using Avalonia.Markup.Xaml;
using PleasantUI.Controls.Custom;

namespace Regul.OlibKey.Views.Windows;

public class SearchView : PleasantDialogWindow
{
	public SearchView() => AvaloniaXamlLoader.Load(this);
}