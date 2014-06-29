using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wordament_Solver_CV.Model
{
  public static  class Settings
    {
      public static List<Color> ThemeColors = new List<Color>() 
      {
      Color.FromArgb(240,150,9),
      Color.FromArgb(27,161,226),
      Color.FromArgb(160,80,0),
      Color.FromArgb(51,153,51),
      Color.FromArgb(162,193,57),
      Color.FromArgb(216,0,115),
      Color.FromArgb(230,113,184),
      Color.FromArgb(162,0,255),
      Color.FromArgb(229,20,0),
      Color.FromArgb(0,171,169)
      };

      public static Color GuessColor = Color.FromArgb(220, 0, 0);

      public static Color ValidWordTileColor = Color.FromArgb(51, 153, 53);
      public static Color InvalidWordTileColor = Color.FromArgb(229, 20, 0);
      public static Color currentTheme = Color.FromArgb(240, 150, 9);
      public static Color WordRepeat = Color.FromArgb(245, 220, 94);


    }
}
