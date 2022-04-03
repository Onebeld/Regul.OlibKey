using Avalonia.Markup.Xaml;
using PleasantUI.Controls.Custom;
using Regul.OlibKey.General;

namespace Regul.OlibKey.Views.Windows;

public class ChangingMasterPassword : PleasantDialogWindow, IMustCloseWhenLocked
{
	public ChangingMasterPassword() => AvaloniaXamlLoader.Load(this);
}