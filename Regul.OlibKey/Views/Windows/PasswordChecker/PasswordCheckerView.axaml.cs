using Avalonia.Markup.Xaml;
using PleasantUI.Controls.Custom;
using Regul.OlibKey.General;

namespace Regul.OlibKey.Views.Windows;

public class PasswordCheckerView : PleasantDialogWindow, IMustCloseWhenLocked
{
	public PasswordCheckerView() => AvaloniaXamlLoader.Load(this);
}