using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Wordament_Solver_CV.HelperClasses;
using Wordament_Solver_CV.Model;
using System.Collections.ObjectModel;
using System.Threading;
using System.IO;
using Wordament_Solver_CV.Cloud;
namespace Wordament_Solver_CV.ViewModel
{
   public class GameService:INotifyPropertyChanged
    {
       internal BackgroundWorker worker = new BackgroundWorker();

       DictionaryService DicService = new DictionaryService();
       public GameService()
       {
           GameStates.owner = this;
           Wordament.owner = this;
           Wordament.dictionary = new WordList();
           Wordament.dictionary.LoadFromFile(Path.Combine(App.AppData, Path.GetFileNameWithoutExtension(App.gameSettings.WordFile) + ".txt"), false);

           ValidWords = new ObservableCollection<WordSequence>();


           globalKeyboardHook keyboardHook = new globalKeyboardHook();
           keyboardHook.HookedKeys.Add(Keys.Escape);
           keyboardHook.KeyDown += keyboardHook_KeyDown;

           worker.DoWork += worker_DoWork;
           worker.WorkerReportsProgress = true;
           worker.WorkerSupportsCancellation = true;
           worker.ProgressChanged += worker_ProgressChanged;
           worker.RunWorkerCompleted += worker_RunWorkerCompleted;
       
       }

       void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
       {

           if (restartFlag)
           {
               Status = "Restarting ..";
              
               worker.RunWorkerAsync();
           }
           else
           Status = "Completed";

           restartFlag = false;
       }


       void keyboardHook_KeyDown(object sender, KeyEventArgs e)
       {
           Stop();
       }
        
       void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
       {
           throw new NotImplementedException();
       }

       public ObservableCollection<WordSequence> ValidWords
       {
           set
           {
               DicService.RequestList = value.Select((p) => p.Word).ToList();
           }
       }

       private string currentWord;

       public string CurrentWord
       {
           get { return currentWord; }
           set { currentWord = value;
           string temp = DicService.GetMeaning(value);
           if (!string.IsNullOrEmpty(temp))
               Meaning = temp;
                 OnPropertyChanged("CurrentWord");
                }
       }
       
      
       private string currentGameState;

       public string CurrentGameState
       {
           get { return currentGameState; }
           set { currentGameState = value;
           OnPropertyChanged("CurrentGameState");
           }
       }

       private string status;

       public string Status
       {
           get { return status; }
           set { status = value; OnPropertyChanged("Status"); }
       }

       private double speed=5;

       public double Speed
       {
           get { return speed; }
           set { speed = value;
           OnPropertyChanged("Speed");
           }
       }

       private string meaning;

       public string Meaning
       {
           get { return meaning; }
           set { meaning = value;
           OnPropertyChanged("Meaning");
           }
       }
       

       private string[,] tileString;

       public string[,] TileString
       {
           get { return tileString; }
           set { tileString = value;
           OnPropertyChanged("TileString");
           }
       }

       public void OnPropertyChanged(string p)
       {
           if (PropertyChanged != null)
               PropertyChanged(this, new PropertyChangedEventArgs(p));
       }

       private System.Windows.Media.SolidColorBrush currentTheme= new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Gray);

       public System.Windows.Media.SolidColorBrush CurrentTheme
       {
           get { return currentTheme; }
           set { currentTheme = value;
           OnPropertyChanged("CurrentTheme");
           }
       }
       
       //string[,] tileString = new string[4, 4];

       void worker_DoWork(object sender, DoWorkEventArgs e)
       {


           Status = "..........";
           
           TileString = new string[4, 4];
           while (!worker.CancellationPending)
           {
               if  (Wordament.rects.Count == 16)
               {
                   if (Wordament.IsTilesValid())
                   {
                       Wordament.GetGameBoard();
                       Wordament.MarkWords();
                   }
                   else
                   {
                       Wordament.rects.Clear();
                   }
               }
               else
               {
                   App.Current.Dispatcher.BeginInvoke((Action)delegate()
                   {
                       CurrentTheme = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.LightGray);
                   });
                 Wordament.FindRects();
               }
           }
           Status = "Cancelled";
       }

       bool restartFlag;
       public void SearchAgain()
       {
           Thread.Sleep(3000);

           Status = "Searching..";
           Wordament.rects.Clear();
           worker.CancelAsync();
           restartFlag = true;
                    
       }

       public void WaitForGameToResume()
       {
           Status = "Waiting...";
           Thread.Sleep(45000);
           Wordament.rects.Clear();
           restartFlag = true;
           worker.CancelAsync();

           Thread t = new Thread(new ThreadStart(() =>
           {
               DreamFactory.IncrementGameCount();
           }));

           t.Start();
       }

       public void Start()
       {
           Status = "Started";

           worker.RunWorkerAsync();
       }

       public void Stop()
       {
           Status = "Stopped";
           worker.CancelAsync();
       }

       public event PropertyChangedEventHandler PropertyChanged;
    }
}
