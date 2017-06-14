using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MARC2.Model;
using System.Windows.Media;

namespace MARC2.Converter
{
    public class ScrollViewHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var input = value as System.Double?;
            if (input == 0)
            {
                return input;
            }
            return (null != input ? input - 70 :0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //Debugger.Break();
            return value;
        }
    }

    public class AltBackgroundConverter : IValueConverter
    {
        private Brush whiteBrush = new SolidColorBrush(Colors.White);
        private Brush grayBrush = new SolidColorBrush(Colors.Gray);

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo language)
        {
            if (!(value is int)) return null;
            int index = (int)value;

            if (index % 2 == 0)
                return whiteBrush;
            else
                return grayBrush;
        }

        // No need to implement converting back on a one-way binding
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo language)
        {
            throw new NotImplementedException();
        }
    }
}
