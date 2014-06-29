using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Wordament_Solver_CV.HelperClasses;
using System.ComponentModel;
using Wordament_Solver_CV.Cloud;
using System.IO.Compression;
using System.Reflection;
using Wordament_Solver_CV.View;
namespace Wordament_Solver_CV
{
  public  class InitailseGame:INotifyPropertyChanged
    {
      
      public  GameSettings gameSettings;

      private string msg;

      public string Msg
      {
          get { return msg; }
          set { msg = value;
          OnPropertyChanged("Msg");
          }
      }

      bool result; 

      private void OnPropertyChanged(string p)
      {
          if (PropertyChanged != null)
              PropertyChanged(this, new PropertyChangedEventArgs(p));
      }
      
      public  void StartUpTask()
      {
          BackgroundWorker bgwrkr = new BackgroundWorker();
          bgwrkr.DoWork += (da, dd) =>
          {
              App.AppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WordamentCV");

              Msg = "Initialising . . .";

              if (!Directory.Exists(App.AppData))
                  Directory.CreateDirectory(App.AppData);



              CloudSettings _cloudSettings = new CloudSettings();

              if (_cloudSettings.IsAvailable == false)
              {
                  Msg = "Service Unavailable, Please check Network settings or Try again!!";
                  return;
              }

              if (File.Exists(Path.Combine(App.AppData, "Setting.xml")))
              {
                  
                  gameSettings = XmlHelper.readXml<GameSettings>(Path.Combine(App.AppData, "Setting.xml"));
              }
              else
              {
                  gameSettings = new GameSettings()
                  {
                      UserId = Guid.NewGuid(),
                      Version = _cloudSettings.Version,
                      AppVersion = _cloudSettings.AppVersion,
                      WordFile = _cloudSettings.FileName,
                  };
                  XmlHelper.writeXml(gameSettings, Path.Combine(App.AppData, "Setting.xml"));
              }

              if (gameSettings.Version != _cloudSettings.Version || !File.Exists(Path.Combine(App.AppData, Path.GetFileNameWithoutExtension(gameSettings.WordFile) + ".txt")))
              {
                  Msg = "Getting Word list...";
                  if (!DownloadWordFile())
                  {
                      Msg = "Unable To Find WordFile, Please restart App";
                      return;
                  }
              }

              App.gameSettings = gameSettings;
              DreamFactory.IncrementCount();

              result = true;
          };

          bgwrkr.RunWorkerCompleted += bgwrkr_RunWorkerCompleted;
          bgwrkr.RunWorkerAsync();
      }

      void bgwrkr_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
      {
          if(result)
          App.RootFrame.Navigate(new GameScreen());

      }

      private bool DownloadWordFile()
      {
          try
          {
              DreamFactory.getWordFile(gameSettings.WordFile,DreamFactory.getSession());

              return true;
          }
          catch (Exception ex)
          {
              ErrorReporting.ReportError(ex);

              return false;
          }
         
      }

      public event PropertyChangedEventHandler PropertyChanged;
    }
}
