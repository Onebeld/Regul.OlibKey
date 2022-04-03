using Avalonia.Markup.Xaml;
using PleasantUI.Controls.Custom;
using Regul.OlibKey.General;

namespace Regul.OlibKey.Views.Windows;

public class TrashcanView : PleasantDialogWindow, IMustCloseWhenLocked
{
	public TrashcanView() => AvaloniaXamlLoader.Load(this);
}