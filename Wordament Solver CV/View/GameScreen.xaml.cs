﻿using System;
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
using Wordament_Solver_CV.ViewModel;

namespace Wordament_Solver_CV.View
{
    /// <summary>
    /// Interaction logic for GameScreen.xaml
    /// </summary>
    public partial class GameScreen : Page
    {
        public GameScreen()
        {
            InitializeComponent();
            GameService game = ((ObjectDataProvider)this.Resources["Vm"]).ObjectInstance as GameService;
            game.Start();
            

        }
    }
}
