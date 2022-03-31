using Avalonia.Markup.Xaml;
using PleasantUI.Controls.Custom;

namespace Regul.OlibKey.Views.Windows;

public class TrashcanView : PleasantDialogWindow
{
	public TrashcanView() => AvaloniaXamlLoader.Load(this);
}