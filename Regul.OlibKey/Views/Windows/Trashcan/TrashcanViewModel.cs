using Avalonia.Collections;
using Onebeld.Extensions;
using Regul.OlibKey.Structures;

namespace Regul.OlibKey.Views.Windows;

public class TrashcanViewModel : ViewModelBase
{
    private Database _database = null!;
    private readonly PasswordManagerViewModel _targetViewModel;
    
    #region Properties

    public Database Database
    {
        get => _database;
        private set => RaiseAndSetIfChanged(ref _database, value);
    }

    public AvaloniaList<Folder> SelectedFolder { get; } = new();
    public AvaloniaList<Data> SelectedData { get; } = new();

    #endregion

    public TrashcanViewModel(PasswordManagerViewModel viewModel, Database database)
    {
        _targetViewModel = viewModel;
        Database = database;
    }

    private void DeleteFolder()
    {
        for (int index = SelectedFolder.Count - 1; index >= 0; index--)
            Database.Trashcan.Folders.Remove(SelectedFolder[index]);

        _targetViewModel.IsEdited = true;
    }

    private void DeleteData()
    {
        for (int index = SelectedData.Count - 1; index >= 0; index--)
            Database.Trashcan.DataList.Remove(SelectedData[index]);
        
        _targetViewModel.IsEdited = true;
    }

    private void RestoreData()
    {
        for (int index = SelectedData.Count - 1; index >= 0; index--)
        {
            Data data = SelectedData[index];

            data.DeleteDate = null;
            data.IsDeleted = false;
            
            Database.DataList.Add(data);
            Database.Trashcan.DataList.Remove(data);
        }
        
        _targetViewModel.IsEdited = true;
    }

    private void RestoreFolder()
    {
        for (int index = SelectedFolder.Count - 1; index >= 0; index--)
        {
            Folder folder = SelectedFolder[index];

            folder.DeleteDate = null;
            folder.IsDeleted = false;
            
            Database.Folders.Add(folder);
            Database.Trashcan.Folders.Remove(folder);
        }
        _targetViewModel.IsEdited = true;
    }
}