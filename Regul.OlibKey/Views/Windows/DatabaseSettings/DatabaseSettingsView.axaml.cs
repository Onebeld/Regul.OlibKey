using Avalonia.Markup.Xaml;
using PleasantUI.Controls.Custom;
using Regul.OlibKey.General;

namespace Regul.OlibKey.Views.Windows;

public class DatabaseSettingsView : PleasantDialogWindow, IMustCloseWhenLocked
{
	public DatabaseSettingsView() => AvaloniaXamlLoader.Load(this);
}