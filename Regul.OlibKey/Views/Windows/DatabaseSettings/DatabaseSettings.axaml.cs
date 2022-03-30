using Avalonia.Markup.Xaml;
using PleasantUI.Controls.Custom;

namespace Regul.OlibKey.Views.Windows;

public class DatabaseSettingsView : PleasantDialogWindow
{
	public DatabaseSettingsView() => AvaloniaXamlLoader.Load(this);
}