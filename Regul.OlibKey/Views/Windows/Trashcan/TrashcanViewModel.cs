using System.Collections.Generic;
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
        set => RaiseAndSetIfChanged(ref _database, value);
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
        Database.Trashcan.Folders.RemoveAll(new List<Folder>(SelectedFolder));

        _targetViewModel.IsEdited = true;
    }

    private void DeleteData()
    {
        Database.Trashcan.DataList.RemoveAll(new List<Data>(SelectedData));
        
        _targetViewModel.IsEdited = true;
    }

    private void RestoreData()
    {
        foreach (Data data in SelectedData)
        {
            data.DeleteDate = null;
            data.IsDeleted = false;
        }

        Database.DataList.AddRange(SelectedData);
        Database.Trashcan.DataList.RemoveAll(new List<Data>(SelectedData));
        
        _targetViewModel.IsEdited = true;
    }

    private void RestoreFolder()
    {
        foreach (Folder folder in SelectedFolder)
        {
            folder.DeleteDate = null;
            folder.IsDeleted = false;
        }

        Database.Folders.AddRange(SelectedFolder);
        Database.Trashcan.Folders.RemoveAll(new List<Folder>(SelectedFolder));
        
        _targetViewModel.IsEdited = true;
    }
}