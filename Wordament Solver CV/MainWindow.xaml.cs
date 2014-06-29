using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wordament_Solver_CV.View;

namespace Wordament_Solver_CV
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            App.RootFrame = rootFrame;
            Loaded += MainWindow_Loaded;

            
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            App.RootFrame.Navigate(new InitialisingScreen());
        }
    }
}
