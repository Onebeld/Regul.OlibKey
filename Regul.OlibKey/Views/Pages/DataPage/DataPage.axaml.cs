using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Regul.OlibKey.Enums;
using Regul.OlibKey.General;

namespace Regul.OlibKey.Views.Pages;

public class DataPage : UserControl, IPage
{
	public DataPage() => AvaloniaXamlLoader.Load(this);

	public DataPage(DataInformation dataInformation, PasswordManagerViewModel viewModel) : this()
	{
		DataContext = new DataPageViewModel(dataInformation, viewModel);
	}
}