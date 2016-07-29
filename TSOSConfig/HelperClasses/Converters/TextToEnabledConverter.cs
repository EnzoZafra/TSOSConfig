using System;
using System.Globalization;
using System.Windows.Data;

namespace TSOSConfig.HelperClasses.Converters
{
    class TextToEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace((string) value))
            {
                return false;
            }
            else if (string.IsNullOrEmpty((string)value))
            {
                return false;
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
