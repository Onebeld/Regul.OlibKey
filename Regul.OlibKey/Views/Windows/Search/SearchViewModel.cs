using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reactive.Linq;
using Avalonia.Collections;
using Avalonia.Media;
using Onebeld.Extensions;
using PleasantUI.Windows;
using System.Linq;
using Regul.Base;
using Regul.OlibKey.Structures;

namespace Regul.OlibKey.Views.Windows;

public class SearchViewModel : ViewModelBase
{
    private readonly PasswordManagerViewModel _targetViewModel;
    private readonly SearchView _view;
    
    private string _searchText = string.Empty;
    private Database _database = null!;
    private Data? _selectedData;
    private Folder? _selectedFolder;

    private bool _searchByAllElements = true;
    private bool _searchByLogins;
    private bool _searchByBankCard;
    private bool _searchByPersonalData;
    private bool _searchByNotes;

    private bool _searchByFavorite;
    private bool _alphabeticalSorting;

    #region Properties

    public Database Database
    {
        get => _database;
        set => RaiseAndSetIfChanged(ref _database, value);
    }
    public string SearchText
    {
        get => _searchText;
        set => RaiseAndSetIfChanged(ref _searchText, value);
    }
    public Data? SelectedData
    {
        get => _selectedData;
        set => RaiseAndSetIfChanged(ref _selectedData, value);
    }

    public Folder? SelectedFolder
    {
        get => _selectedFolder;
        set => RaiseAndSetIfChanged(ref _selectedFolder, value);
    }

    public bool SearchByFavorite
    {
        get => _searchByFavorite;
        set => RaiseAndSetIfChanged(ref _searchByFavorite, value);
    }

    public bool AlphabeticalSorting
    {
        get => _alphabeticalSorting;
        set => RaiseAndSetIfChanged(ref _alphabeticalSorting, value);
    }

    public AvaloniaList<Data> FoundedDataList { get; } = new();

    #endregion

    public SearchViewModel(SearchView view, PasswordManagerViewModel targetViewModel, Database database)
    {
        _view = view;
        _targetViewModel = targetViewModel;
        Database = database;

        this.WhenAnyValue(x => x.SearchText)
            .Throttle(TimeSpan.FromMilliseconds(1000))
            .Subscribe(DoSearch);
        this.WhenAnyValue(x => x.SelectedData)
            .Where(x => x is not null)
            .Subscribe(DoSelect!);
    }

    private void AddFolder()
    {
        Folder folder = new()
        {
            Id = Guid.NewGuid().ToString("D"),
            Name = App.GetResource<string>("NoName")
        };
        
        Database.Folders.Add(folder);
        
        _targetViewModel.IsEdited = true;
    }

    private void DeleteFolder()
    {
        if (SelectedFolder is null) return;
        
        if (Database.Settings.UseTrashcan)
        {
            SelectedFolder.IsDeleted = true;
            SelectedFolder.DeleteDate = DateTime.Now.ToString(CultureInfo.CurrentCulture);
            Database.Trashcan.Folders.Add(SelectedFolder);
        }

        Database.Folders.Remove(SelectedFolder);

        _targetViewModel.IsEdited = true;
    }

    private async void ChangeColorForFolder()
    {
        if (SelectedFolder is null) return;
        
        try
        {
            Color dataColor;
            dataColor = SelectedFolder.Color == 0 
                ? new Color(255, 255, 255, 255) 
                : Color.FromUInt32(SelectedFolder.Color);
                
            Color color = await WindowColorPicker.SelectColor(WindowsManager.MainWindow, dataColor.ToString());

            SelectedFolder.Color = color.ToUint32();
        }
        catch
        {
            // ignored
        }
    }

    private void DoSelect(Data data)
    {
        _targetViewModel.SelectedData = data;
        _view.Close();
    }

    private void DoSearch(string str)
    {
        FoundedDataList.Clear();

        if (string.IsNullOrEmpty(str))
        {
            FoundedDataList.AddRange(new List<Data>(Database.DataList));
            return;
        }

        string lowerStr = str.ToLower();

        List<Data> results = Database.DataList
            .Where(x => (x.Name is not null && x.Name.ToLower().Contains(lowerStr))
            || (x.Login is { Username: { }, Email: { } } && (x.Login.Username.ToLower().Contains(lowerStr) 
                                                             || x.Login.Email.ToLower().Contains(lowerStr)))
            || (x.BankCard?.CardNumber is not null && x.BankCard.CardNumber.ToLower().Contains(lowerStr)
            || (x.PersonalData?.Fullname is not null && x.PersonalData.Fullname.ToLower().Contains(lowerStr)))).ToList();

        if (SearchByFavorite) 
            results = results.FindAll(x => x.Favorite);

        if (AlphabeticalSorting)
            results = results.OrderBy(x => x.Name).ToList();
        
        FoundedDataList.AddRange(results);
    }
}