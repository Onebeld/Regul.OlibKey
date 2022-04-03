using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Regul.ModuleSystem.Models;

namespace Regul.OlibKey.Views;

public partial class OlibKeySettingsView : UserControl, IModuleSettings
{
    public OlibKeySettingsView() => AvaloniaXamlLoader.Load(this);
}