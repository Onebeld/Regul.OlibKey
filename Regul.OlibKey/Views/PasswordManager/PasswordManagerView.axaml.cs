using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PleasantUI.Controls.Custom;
using Regul.ModuleSystem;

namespace Regul.OlibKey.Views;

public class PasswordManagerView : UserControl, IEditor
{
	private PasswordManagerViewModel _viewModel;
	
	public string Id { get; set; }
	public Project? Project { get; set; }
	public string? FilePath { get; set; }
	public Editor Editor { get; set; }

	public PasswordManagerView()
	{
		AvaloniaXamlLoader.Load(this);
	}

	public void Load(string? path, Project? project, PleasantTabItem pleasantTabItem, Editor editor)
	{
		PasswordManagerViewModel viewModel = new(pleasantTabItem, editor, this, path);
		Id = editor.Id;

		DataContext = _viewModel = viewModel;
	}

	public void Release()
	{
	}

	public bool Save(string? path) => _viewModel.Save(path);
}