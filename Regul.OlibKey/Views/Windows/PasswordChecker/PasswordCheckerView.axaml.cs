using Avalonia.Markup.Xaml;
using PleasantUI.Controls.Custom;

namespace Regul.OlibKey.Views.Windows;

public class PasswordCheckerView : PleasantDialogWindow
{
	public PasswordCheckerView() => AvaloniaXamlLoader.Load(this);
}