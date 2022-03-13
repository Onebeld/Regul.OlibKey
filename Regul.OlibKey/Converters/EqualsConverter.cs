using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Regul.OlibKey.Converters
{
    public class EqualsConverter : IValueConverter
    {
        public static readonly EqualsConverter Instance = new EqualsConverter();
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter as string == value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}