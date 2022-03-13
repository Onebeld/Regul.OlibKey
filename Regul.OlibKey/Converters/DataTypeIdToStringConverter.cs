using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using Regul.Base;

namespace Regul.OlibKey.Converters
{
    public class DataTypeIdToStringConverter : IMultiValueConverter
    {
        public static readonly DataTypeIdToStringConverter Instance = new DataTypeIdToStringConverter();
        
        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            string typeId = values[4] as string;

            switch (typeId)
            {
                case "DT_Login":
                    if (!string.IsNullOrEmpty(values[0] as string))
                        return values[0];
                    if (!string.IsNullOrEmpty(values[1] as string))
                        return values[1];
                    return App.GetResource<string>("NoData");
                case "DT_BankCard":
                    return !string.IsNullOrEmpty(values[2] as string) ? values[2] : App.GetResource<string>("NoData");
                case "DT_PersonalData":
                    return !string.IsNullOrEmpty(values[3] as string) ? values[3] : App.GetResource<string>("NoData");
                default:
                    return string.Empty;
            }
        }
    }
}