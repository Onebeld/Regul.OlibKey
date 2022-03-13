using Avalonia.Data.Converters;
using Regul.Base;
using Regul.OlibKey.General;

namespace Regul.OlibKey.Converters
{
    public static class OlibKeyConverters
    {
        public static readonly IValueConverter EmptyToNotDataConverter =
            new FuncValueConverter<string, string>(str => 
                string.IsNullOrEmpty(str) ? App.GetResource<string>("NoName") : str);

        public static readonly IValueConverter CheckComplexityPassword =
            new FuncValueConverter<string, double>(PasswordUtils.GetPasswordComplexity);
    }
}