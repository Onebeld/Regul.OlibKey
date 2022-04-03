using Avalonia.Markup.Xaml;
using PleasantUI.Controls.Custom;
using Regul.OlibKey.General;

namespace Regul.OlibKey.Views.Windows;

public class SearchView : PleasantDialogWindow, IMustCloseWhenLocked
{
	public SearchView() => AvaloniaXamlLoader.Load(this);
}