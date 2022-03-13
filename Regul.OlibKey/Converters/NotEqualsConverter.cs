using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Regul.OlibKey.Converters
{
    public class NotEqualsConverter : IValueConverter
    {
        public static readonly NotEqualsConverter Instance = new NotEqualsConverter();
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter as string != value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}