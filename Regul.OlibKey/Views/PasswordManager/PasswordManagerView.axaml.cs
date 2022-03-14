using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PleasantUI.Controls.Custom;
using Regul.ModuleSystem;

namespace Regul.OlibKey.Views
{
	public class PasswordManagerView : UserControl, IEditor
	{
		private PasswordManagerViewModel _viewModel;

		public PasswordManagerView()
		{
			AvaloniaXamlLoader.Load(this);
		}

		public string FileToPath { get; set; }
		public Editor CurrentEditor { get; set; }
		public Project CurrentProject { get; set; }
		public string Id { get; set; }

		public void Load(string path, Project project, PleasantTabItem pleasantTabItem, Editor editor)
		{
			PasswordManagerViewModel viewModel = new PasswordManagerViewModel(pleasantTabItem, editor, this, path);
			Id = editor.Id;

			DataContext = _viewModel = viewModel;
		}

		public void Release() => _viewModel.Release();

		public bool Save(string path) => _viewModel.Save(path);
	}
}
