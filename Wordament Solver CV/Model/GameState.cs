using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Wordament_Solver_CV.ViewModel;

namespace Wordament_Solver_CV.Model
{
    public enum GameState
    {
        InGame,
        ValidWord,
        InvalidWord,
        GuessingWarning,
        TimesUp,
        Unknown
    }
   internal static class GameStates
    {
       public static GameService owner;

        public static Color GetPixelAt(int screenX, int screenY)
        {
            using (var bmp = new Bitmap(1, 1))
            {
                using (var graphics = Graphics.FromImage(bmp))
                {
                    graphics.CopyFromScreen(screenX, screenY, 0, 0, new Size(1, 1));
                    return bmp.GetPixel(0, 0);
                }
            }
        }

        public static bool AccusedOfGuessing(Rectangle windowBounds, Rectangle[] tileRects)
        {
            if (tileRects.Length != 16)
                throw new ArgumentException("TileRects argument must be an ordered array of 16 tiles");

            var tileRect = tileRects[4];

            var testPoint = new Point(
                windowBounds.Left + tileRect.Left + 2,
                windowBounds.Top + tileRect.Bottom + 2);

            var color = GetPixelAt(testPoint.X, testPoint.Y);

            return (color == Color.Red);
        }

        public static int GetcolorDifference(Color color1, Color color2)
        {
            return
                Math.Abs(color1.R - color2.R) +
                Math.Abs(color1.G - color2.G) +
                Math.Abs(color1.B - color2.B);
        }

     

        public static GameState GetGameState(
            Rectangle[] tileRects,
            int lastPlayedTileIndex)
        {
            if (tileRects.Length != 16)
                return GameState.Unknown;

            var testPoint = new Point(
                 tileRects[5].Right + 4,
                tileRects[5].Bottom + 2);

            var color = GetPixelAt(testPoint.X, testPoint.Y);

            int colorDiff;

            colorDiff = GetcolorDifference(color, Settings.GuessColor);

            if (colorDiff < 10)
            {
                testPoint = new Point(
                    tileRects[lastPlayedTileIndex].Left + 1,
                   tileRects[lastPlayedTileIndex].Top + 1);

                color = GetPixelAt(testPoint.X, testPoint.Y);
                return GameState.GuessingWarning;
            }

            colorDiff = GetcolorDifference(color,Settings.currentTheme);
            if (colorDiff < 10)
                return GameState.TimesUp;


            color = GetPixelAt(tileRects[15].Right-10, tileRects[15].Bottom-10);

            if (GetcolorDifference(color, Settings.currentTheme) > 10)
            if (GetcolorDifference(color, Settings.WordRepeat)>10)
                if (GetcolorDifference(color, Settings.InvalidWordTileColor)>10)
                if (GetcolorDifference(color, Settings.ValidWordTileColor) > 10)
                    if (GetcolorDifference(color, Color.White) > 10)
                {
                //displayImage();
                return GameState.Unknown;
                }
            if (lastPlayedTileIndex > -1)
            {
                testPoint = new Point(
                    tileRects[lastPlayedTileIndex].Left + 1,
                     tileRects[lastPlayedTileIndex].Top + 1);

                color = GetPixelAt(testPoint.X, testPoint.Y);

                if (color != Settings.currentTheme)
                {
                    var validcolorDiff = GetcolorDifference(color, Settings.ValidWordTileColor);
                    var invalidcolorDiff = GetcolorDifference(color, Settings.InvalidWordTileColor);

                    const int validWordTollerance = 175;

                   

                    if (validcolorDiff < validWordTollerance)
                    {
                       
                        return GameState.ValidWord;
                    }

                    if (invalidcolorDiff < validWordTollerance)
                    {
                      
                        return GameState.InvalidWord;
                    }
                }
               
            }

            return GameState.InGame;
        }

        public static void displayImage()
        {
            App.Current.Dispatcher.BeginInvoke((Action)delegate()
            {
                string temp = GetTempPath();
                string tempBmp = GetTempPath();
                using (var bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height))
                {
                    using (var graphics = Graphics.FromImage(bmp))
                    {
                        graphics.CopyFromScreen(0, 0, 0, 0, new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height));

                    }
                    bmp.Save(tempBmp);
                }




                BitmapImage bmp1 = new BitmapImage(new Uri(tempBmp));
                preview pv = new preview(bmp1);

                pv.Show();
            });
        }

        private static string GetTempPath()
        {
         string tempPath = Path.GetTempPath() + "\\Wordament";
         if (!Directory.Exists(tempPath)) Directory.CreateDirectory(tempPath);


         return tempPath + "\\" + Guid.NewGuid() + ".bmp";

        }

       
    }
}
