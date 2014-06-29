using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Wordament_Solver_CV.HelperClasses;

namespace Wordament_Solver_CV
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Frame RootFrame;
        public static string AppData;
        public static GameSettings gameSettings;
       
         public App()
         {
             Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
         }

         void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
         {
             ErrorReporting.ReportError(e.Exception);
         }
       
    }
}
