using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Regul.OlibKey.Enums;
using Regul.OlibKey.Structures;

namespace Regul.OlibKey.Views.Pages
{
	public class DataPage : UserControl, IPage
	{
		public DataPage()
		{
			AvaloniaXamlLoader.Load(this);
		}

		public DataPage(DataInformation dataInformation, Database database, PasswordManagerViewModel viewModel, Data data = null) : this()
		{
			DataContext = new DataPageViewModel(dataInformation, database, viewModel, data);
		}
	}
}
