using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Wordament_Solver_CV.HelperClasses;
using Wordament_Solver_CV.ViewModel;

namespace Wordament_Solver_CV.Model
{
    public static class Wordament
    {

        internal static GameService owner;
        public static List<Rectangle> rects = new List<Rectangle>();
        public static Bitmap screenShot;
        public static void FindRects()
        {

            screenShot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            using (var graphics = System.Drawing.Graphics.FromImage(screenShot))
            {
                // Copy from screen into our bmp
                graphics.CopyFromScreen(
                    new System.Drawing.Point(0, 0),
                    new System.Drawing.Point(0, 0),
                    new System.Drawing.Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height));
            }

            rects = GetTileRects(screenShot).ToList();
            var temp = (from a in rects group a by a.Width into g select new { width = g.Key, count = g.Count() });
            int TileWidth = (from a in temp where a.count == temp.Max(s => s.count) select a.width).FirstOrDefault();

            rects = rects.Where(r => (r.Width == TileWidth)).OrderBy(r => r.Top).ThenBy(r => r.Left).ToList();

            if (rects.Count != 16)
            {


                owner.Status = "Game Not Found";
                Thread.Sleep(5000);

                owner.Status = "searching Again";
                FindRects();
            }


        }

        private delegate bool InRectFunction(int x, int y);
        private delegate bool FindNextPixelFunction(
            int prevX, int prevY,
            int currX, int currY,
            out int nextX, out int nextY);

        public static Rectangle[] GetTileRects(Bitmap screenShot)
        {
            var screen = new BitmapWrapper(screenShot);

            var rects = new List<Rectangle>();

            InRectFunction inRect = delegate(int x, int y)
            {
                foreach (var rect in rects)
                {
                    if ((x >= rect.Left) && (x <= rect.Right) &&
                        (y >= rect.Top) && (y <= rect.Bottom))
                        return true;
                }
                return false;
            };

            var searchOffsets = new List<Point> {
				new Point(-1, -1),
				new Point(0, -1),
				new Point(1, -1),
				new Point(1, 0),
				new Point(1, 1),
				new Point(0, 1),
				new Point(-1, 1),
				new Point(-1, 0)
			};

            FindNextPixelFunction findNextPixel = delegate(
                int prevX, int prevY,
                int currX, int currY,
                out int nextX, out int nextY)
            {
                nextX = -1;
                nextY = -1;

                Point prevOffset = new Point(prevX - currX, prevY - currY);
                int prevOffsetIndex = searchOffsets.IndexOf(prevOffset);

                List<Point> offsets = new List<Point>(searchOffsets);

                while (prevOffsetIndex >= 0)
                {
                    var p = offsets[0];
                    offsets.RemoveAt(0);
                    offsets.Add(p);
                    prevOffsetIndex--;
                }

                for (var index = 0; index < offsets.Count; index++)
                {
                    var offset = offsets[index];

                    nextX = currX + offset.X;
                    nextY = currY + offset.Y;

                    if ((nextX < 0) || (nextX >= screen.Width) || (nextY < 0) || (nextY >= screen.Height))
                        continue;

                    if (Settings.ThemeColors.Contains(screen.GetPixel(nextX, nextY)))
                        return true;
                }

                return false;
            };

            for (var y = 0; y < screen.Height; y++)
            {
                for (var x = 0; x < screen.Width; x++)
                {
                    if (inRect(x, y))
                        continue;

                    if (Settings.ThemeColors.Contains(screen.GetPixel(x, y)))
                    {
                        int tileLeft = x;
                        int tileRight = x;
                        int tileTop = y;
                        int tileBottom = y;

                        int prevX = x - 1;
                        int prevY = y;
                        int currX = x;
                        int currY = y;
                        int nextX;
                        int nextY;

                        while (findNextPixel(prevX, prevY, currX, currY, out nextX, out nextY))
                        {
                            if ((nextX == x) & (nextY == y))
                            {
                                break;
                            }

                            tileLeft = Math.Min(tileLeft, nextX);
                            tileRight = Math.Max(tileRight, nextX);
                            tileTop = Math.Min(tileTop, nextY);
                            tileBottom = Math.Max(tileBottom, nextY);

                            prevX = currX;
                            prevY = currY;
                            currX = nextX;
                            currY = nextY;
                        }

                        Rectangle tileRect = new Rectangle(tileLeft, tileTop, tileRight - tileLeft, tileBottom - tileTop);

                        if ((tileRect.Width > 0) && (tileRect.Height > 0))
                            rects.Add(tileRect);
                    }
                }
            }

            return rects.ToArray();
        }


        internal static bool IsTilesValid()
        {

            var lefts = new HashSet<int>();
            var tops = new HashSet<int>();
            var widths = new HashSet<int>();
            var heights = new HashSet<int>();

            foreach (var rect in rects)
            {
                lefts.Add(rect.Left);
                tops.Add(rect.Top);
                widths.Add(rect.Width);
                heights.Add(rect.Height);
            }

            return
            ((lefts.Count == 4) && (tops.Count == 4) &&
                (widths.Count == 1) && (heights.Count == 1));

        }

        internal static void GetGameBoard()
        {
            for (int i = 0; i < 16; i++)
            {
                using (var tileBitmap = new Bitmap(Wordament.rects[i].Width, Wordament.rects[i].Height))
                {
                    using (var graphics = Graphics.FromImage(tileBitmap))
                    {
                        graphics.DrawImage(Wordament.screenShot, 0, 0, Wordament.rects[i], GraphicsUnit.Pixel);

                        var bmpWrapper = new BitmapWrapper(tileBitmap);
                        Color currentColor = bmpWrapper.GetPixel(0, 0);
                        Settings.currentTheme = currentColor;
                        App.Current.Dispatcher.BeginInvoke((Action)delegate()
                        {
                            owner.CurrentTheme = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(currentColor.R, currentColor.G, currentColor.B));
                        });
                        Brush tileBrush = new SolidBrush(currentColor);
                        graphics.FillRectangle(tileBrush, 0, 0, Wordament.rects[i].Width / 4, Wordament.rects[i].Height / 4);


                        using (var ocr = new Tesseract.TesseractEngine(Environment.CurrentDirectory, "Eng"))
                        {
                            string text = ocr.Process(tileBitmap, Tesseract.PageSegMode.SingleBlock).GetText();
                            text = text.Trim(' ', '-', '/', '\\','\n');
                            
                            if (text == "") text = "I";

                            int tileX = i % 4;
                            int tileY = (int)Math.Truncate(i / 4.0f);
                            owner.TileString[tileX, tileY] = text;
                            owner.OnPropertyChanged("TileString");
                        }

                    }
                }
            }
        }

        internal static WordList dictionary;

      static  string[,] cachedString ;
     static   int CachedIndex = 0;
        internal static void MarkWords()
        {
            var finder = new WordFinder();
            var words = finder.FindWordSequences(dictionary, owner.TileString).
                Distinct(new DistinctWordComparer()).
                OrderByDescending(w => w.Word.Length).ToArray();

            App.Current.Dispatcher.BeginInvoke((Action)delegate()
            {
                owner.ValidWords = new System.Collections.ObjectModel.ObservableCollection<WordSequence>(words);
            });

            if (cachedString == null)
                CachedIndex = 0;
            else if (!compare( cachedString , owner.TileString))
            {
                
                cachedString = null;
                CachedIndex = 0;
            }


            for (int i = CachedIndex; i < words.Count(); i++)
            {
                var item = words[i];
                owner.CurrentWord = item.Word;

                if (owner.worker.CancellationPending)
                    break;

                owner.Status = "Marking " + item.Word; 

                item.IsMarked = 1;
                 GameState currentState= MarkWord(item);

                 if (currentState != GameState.ValidWord && currentState != GameState.InvalidWord)
                     owner.CurrentGameState = currentState.ToString();

                 if (currentState == GameState.TimesUp)
                     owner.WaitForGameToResume();

                 if (currentState == GameState.Unknown)
                 {
                     cachedString = owner.TileString;
                     CachedIndex = i;
                     owner.SearchAgain();
                 }
               
                 Thread.Sleep((int)(3000/owner.Speed));
            }

        }

        private static bool compare(string[,] cachedString, string[,] p)
        {
            
            for (int i = 0; i < cachedString.GetUpperBound(0); i++)
            {
                for (int j = 0; j < cachedString.GetUpperBound(1); j++)
                {
                    if (string.Compare(cachedString[i, j], p[i, j]) != 0)
                        return false;
                }
            }

            return true;
                 
        }

        private static GameState MarkWord(WordSequence seq)
        {
            const int pathSmoothingPointCount = 4;

            int previousTileCenterX = -1;
            int previousTileCenterY = -1;
            GameState currentState = GameState.InGame;
            for (var tileIndex = 0; tileIndex < seq.Tiles.Length; tileIndex++)
            {
                var tileLocation = seq.Tiles[tileIndex];
                int gridTileIndex = (int)(tileLocation.X + (tileLocation.Y * 4));
                var tileRect = rects[gridTileIndex];
                int tileCenterX = tileRect.Left + (tileRect.Width / 2);
                int tileCenterY = tileRect.Top + (tileRect.Height / 2);


                if ((pathSmoothingPointCount > 0) && (tileIndex > 0))
                {
                    float stepX = (tileCenterX - previousTileCenterX) / (pathSmoothingPointCount + 1);
                    float stepY = (tileCenterY - previousTileCenterY) / (pathSmoothingPointCount + 1);

                    for (var pathSmoothingIndex = 0; pathSmoothingIndex < pathSmoothingPointCount; pathSmoothingIndex++)
                    {
                        int x = (int)(previousTileCenterX + (stepX * (pathSmoothingIndex + 1)));
                        int y = (int)(previousTileCenterY + (stepY * (pathSmoothingIndex + 1)));
                        MouseControl.SetMousePos(x, y);
                        MouseControl.LeftButtonDown(x, y);
                        Color color = GameStates.GetPixelAt(tileRect.Right - 5, tileRect.Bottom - 5);

                        if (pathSmoothingIndex ==3 && GameStates.GetcolorDifference(color, Color.White) > 10)
                        {
                          GameState tempstate = GameStates.GetGameState(rects.ToArray(),-1);
                          if (tempstate != GameState.TimesUp)
                          {
                              MouseControl.LeftButtonUp(tileCenterX, tileCenterY);
                              return GameState.Unknown;

                          }
                        }

                    }
                }
               
                MouseControl.SetMousePos(tileCenterX, tileCenterY);

               
                if (tileIndex == seq.Tiles.Length - 1)
                    MouseControl.LeftButtonUp(tileCenterX, tileCenterY);

                previousTileCenterX = tileCenterX;
                previousTileCenterY = tileCenterY;
                
                 currentState= GameStates.GetGameState(rects.ToArray(),tileIndex);

                 if (currentState == GameState.ValidWord)
                     seq.IsMarked = 2;
                 else
                     seq.IsMarked = 3;

                System.Threading.Thread.Sleep((int)(1000/owner.Speed));
            }
            return currentState;
        }
    }
}
