using System;
using System.Globalization;
using System.Windows.Data;

namespace Battleship_WPF.Converters
{
    public class IntStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (int.TryParse((string)value, out int result))
            {
                int? returnable = result;
                return returnable;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int? val = value as int?;
            return val.ToString();
        }
    }
}
