using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;

namespace Wordament_Solver_CV
{
    public class stringToDoc : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;

            FlowDocument myFlowDoc = new FlowDocument();
            myFlowDoc.Blocks.Add(new Paragraph(new Run(value.ToString())));
            return myFlowDoc;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class testBinding : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

   public class TileStringConvertors:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string[,] temp = value as string[,];

            if (temp == null) return null;

            int index;

            int.TryParse(parameter.ToString(), out index);

            switch(index)
            {
                case 0: return temp[0, 0];
                case 1: return temp[0, 1];
                case 2: return temp[0, 2];
                case 3: return temp[0, 3];
                case 4: return temp[1, 0];
                case 5: return temp[1, 1];
                case 6: return temp[1, 2];
                case 7: return temp[1, 3];
                case 8: return temp[2, 0];
                case 9: return temp[2, 1];
                case 10: return temp[2, 2];
                case 11: return temp[2, 3];
                case 12: return temp[3, 0];
                case 13: return temp[3, 1];
                case 14: return temp[3, 2];
                case 15: return temp[3, 3];
                default: return null;

            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
