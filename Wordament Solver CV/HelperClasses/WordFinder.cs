using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.ComponentModel;

namespace Wordament_Solver_CV.HelperClasses
{
    public class WordSequence:INotifyPropertyChanged
    {
        public WordSequence(Point[] tiles, string word)
        {
            Tiles = tiles;
            Word = word;
            
        }

        public Point[] Tiles { get; private set; }

        public string Word { get; private set; }

        private int isMarked = 0;
        /*
         * 0 - nothing 
         * 1- started marking
         * 2- marked successfully
         * 3- marking failed
         */

        public int IsMarked
        {
            get { return isMarked; }
            set { isMarked = value;
            OnPropertyChanged("IsMarked");
            }
        }

        private void OnPropertyChanged(string p)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(p));
        }
        


        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class WordFinder
    {
        private IWordList dict;
        private bool[,] used;
        private string[,] board;
        private int w;
        private int h;
        private int depth;
        private string[] letters;
        private Point[] places;

        public int CombinationCount { get; private set; }

        public IEnumerable<string> FindWords(IWordList dictionary, string[,] board)
        {
            foreach (var seq in FindWordSequences(dictionary, board))
            {
                yield return seq.Word;
            }
        }

        public IEnumerable<WordSequence> FindWordSequences(IWordList dictionary, string[,] board)
        {
            dict = dictionary;

            w = board.GetUpperBound(0) + 1;
            h = board.GetUpperBound(1) + 1;

            letters = new string[w * h];
            places = new Point[w * h];

            this.board = board;

            used = new bool[w, h];
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    used[x, y] = false;
                    letters[x + (w * y)] = "";
                }

            depth = 0;

            CombinationCount = 0;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    string currentLetter = board[x, y];
                    if (currentLetter.Contains("/"))
                    {
                        foreach (var letter in currentLetter.Split('/'))
                            foreach (var seq in FindWordSequences(x, y, letter))
                                yield return seq;
                    }
                    else
                    {
                        foreach (var seq in FindWordSequences(x, y, currentLetter))
                            yield return seq;
                    }
                }
            }
        }

        private IEnumerable<WordSequence> FindWordSequences(int currentX, int currentY, string currentLetter)
        {
            letters[depth] = currentLetter;
            used[currentX, currentY] = true;
            places[depth].X = currentX;
            places[depth].Y = currentY;
            CombinationCount++;

            depth++;

            bool noWordsWithPrefix = false;

            if (depth >= 3)
            {
                string currentWord = string.Join("", letters, 0, depth);
                if (dict.Contains(currentWord))
                {
                    var tiles = new Point[depth];
                    for (var tileIndex = 0; tileIndex < depth; tileIndex++)
                        tiles[tileIndex] = places[tileIndex];

                    yield return new WordSequence(tiles, currentWord);
                }

                noWordsWithPrefix = !dict.ContainsStartingWith(currentWord);
            }

            if (!noWordsWithPrefix)
            {
                for (int plusY = -1; plusY <= 1; plusY++)
                {
                    for (int plusX = -1; plusX <= 1; plusX++)
                    {
                        if (plusX == 0 && plusY == 0)
                            continue;

                        int newX = currentX + plusX;
                        int newY = currentY + plusY;

                        if ((newX < 0) || (newX >= w) || (newY < 0) || (newY >= h))
                            continue;

                        if (used[newX, newY])
                            continue;

                        string nextLetter = board[newX, newY];
                        if (nextLetter.Contains("/"))
                        {
                            foreach (var letter in currentLetter.Split('/'))
                                foreach (var seq in FindWordSequences(newX, newY, letter))
                                    yield return seq;
                        }
                        else
                        {
                            foreach (var seq in FindWordSequences(newX, newY, nextLetter))
                                yield return seq;
                        }
                    }
                }
            }

            depth--;
            letters[depth] = "";
            used[currentX, currentY] = false;
        }
    }

    public interface IWordList
    {
        void LoadFromFile(string fileName, bool autoAddPlurals);
        bool Contains(string word);
        int Count { get; }
        bool ContainsStartingWith(string text);
    }

    public class WordList : IWordList
    {
        private HashSet<string> hash = new HashSet<string>();
        private List<string> list = new List<string>();

        public WordList()
        {
        }

        public WordList(IEnumerable<string> words)
        {
            foreach (var word in words.OrderBy(w => w))
                AddWord(word);
        }

        public void LoadFromFile(string fileName, bool autoAddPlurals)
        {
            hash = new HashSet<string>();
            list = new List<string>();

            using (var reader = new StreamReader(fileName))
            {
                while (!reader.EndOfStream)
                {
                    string word = reader.ReadLine().Trim();

                    if (AddWord(word) && autoAddPlurals)
                    {
                        if (word.Length > 1)
                            if (word[word.Length - 1] != 's')
                            {
                                if (word.Substring(word.Length - 2, 2) == "ty")
                                    AddWord(word.Substring(0, word.Length - 2) + "ties");
                                else if (word[word.Length - 1] == 'x')
                                    AddWord(word + "es");
                                else
                                    AddWord(word + 's');
                            }
                    }
                }
            }
        }

        private bool AddWord(string word)
        {
            word = word.Replace("-", "").Replace("'", "").Trim().ToLower();
            if (word == "")
                return false;
            if (hash.Add(word))
            {
                list.Add(word);
                return true;
            }
            return false;
        }

        public bool Contains(string word)
        {
            return hash.Contains(word.ToLower());
        }

        public int Count
        {
            get { return hash.Count; }
        }

        public bool ContainsStartingWith(string text)
        {
            return BinarySearch(text, false, true) > -1;
        }

        internal int BinarySearch(string word, bool closestMatch, bool startingWith)
        {
            if (list.Count == 0)
                return -1;

            word = word.ToLower();

            int minIndex = 0;
            int maxIndex = list.Count - 1;
            int midIndex = -1;

            while (maxIndex >= minIndex)
            {
                midIndex = minIndex + (int)Math.Truncate((maxIndex - minIndex) / 2f);

                string midWord = list[midIndex];

                if (startingWith && (midWord.Length > word.Length))
                    midWord = midWord.Substring(0, word.Length);

                int compare = word.CompareTo(midWord);

                if (compare < 0)
                    maxIndex = midIndex - 1;
                else if (compare > 0)
                    minIndex = midIndex + 1;
                else
                {
                    if (!startingWith)
                        return midIndex;

                    while ((midIndex > minIndex) && (list[midIndex - 1].StartsWith(word)))
                    {
                        midIndex--;
                    }
                    return midIndex;
                }
            };

            if (!closestMatch)
                return -1;

            return minIndex;
        }
    }
    public class DistinctWordComparer : IEqualityComparer<WordSequence>
    {
        public bool Equals(WordSequence x, WordSequence y)
        {
            return x.Word.ToLower() == x.Word.ToLower();
        }

        public int GetHashCode(WordSequence obj)
        {
            return obj.Word.ToLower().GetHashCode();
        }
    }
}
