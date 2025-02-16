using System;
using System.Globalization;
using System.Windows.Data;

namespace Battleship_WPF.Converters
{
    class PortConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (int.TryParse((string)value, out int result))
                return result;
            else return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((int)value).ToString();
        }
    }
}
