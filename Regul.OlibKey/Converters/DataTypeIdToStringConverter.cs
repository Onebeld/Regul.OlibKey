using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using Regul.Base;
using Regul.OlibKey.Enums;

namespace Regul.OlibKey.Converters;

public class DataTypeIdToStringConverter : IMultiValueConverter
{
    public static readonly DataTypeIdToStringConverter Instance = new();
        
    public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        DataType typeId = values[4] as DataType? ?? DataType.Login;

        switch (typeId)
        {
            case DataType.Login:
                if (!string.IsNullOrEmpty(values[0] as string))
                    return values[0];
                if (!string.IsNullOrEmpty(values[1] as string))
                    return values[1];
                return App.GetResource<string>("NoData");
            case DataType.BankCard:
                return !string.IsNullOrEmpty(values[2] as string) ? values[2] : App.GetResource<string>("NoData");
            case DataType.PersonalData:
                return !string.IsNullOrEmpty(values[3] as string) ? values[3] : App.GetResource<string>("NoData");
                
            default:
                return string.Empty;
        }
    }
}