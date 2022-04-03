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
using Regul.OlibKey.Enums;
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
    private AvaloniaList<Data> _foundedDataList = new();

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
        set
        {
            RaiseAndSetIfChanged(ref _selectedFolder, value);
            DoSearch(SearchText);
        }
    }

    public bool SearchByFavorite
    {
        get => _searchByFavorite;
        set
        {
            RaiseAndSetIfChanged(ref _searchByFavorite, value);
            DoSearch(SearchText);
        }
    }

    public bool AlphabeticalSorting
    {
        get => _alphabeticalSorting;
        set
        {
            RaiseAndSetIfChanged(ref _alphabeticalSorting, value);
            DoSearch(SearchText);
        }
    }

    public AvaloniaList<Data> FoundedDataList
    {
        get => _foundedDataList;
        set => RaiseAndSetIfChanged(ref _foundedDataList, value);
    }

    public bool SearchByAllElements
    {
        get => _searchByAllElements;
        set
        {
            RaiseAndSetIfChanged(ref _searchByAllElements, value);
            if (value) DoSearch(SearchText);
        }
    }

    public bool SearchByLogins
    {
        get => _searchByLogins;
        set
        {
            RaiseAndSetIfChanged(ref _searchByLogins, value);
            if (value) DoSearch(SearchText);
        }
    }

    public bool SearchByBankCard
    {
        get => _searchByBankCard;
        set
        {
            RaiseAndSetIfChanged(ref _searchByBankCard, value);
            if (value) DoSearch(SearchText);
        }
    }

    public bool SearchByPersonalData
    {
        get => _searchByPersonalData;
        set
        {
            RaiseAndSetIfChanged(ref _searchByPersonalData, value);
            if (value) DoSearch(SearchText);
        }
    }

    public bool SearchByNotes
    {
        get => _searchByNotes;
        set
        {
            RaiseAndSetIfChanged(ref _searchByNotes, value);
            if (value) DoSearch(SearchText);
        }
    }

    #endregion

    public SearchViewModel(SearchView view, PasswordManagerViewModel targetViewModel, Database database)
    {
        _view = view;
        _targetViewModel = targetViewModel;
        Database = database;
        
        DoSearch(SearchText);

        this.WhenAnyValue(x => x.SearchText)
            .Skip(1)
            .Throttle(TimeSpan.FromMilliseconds(300))
            .Subscribe(DoSearch);
        this.WhenAnyValue(x => x.SelectedData)
            .Where(x => x is not null)
            .Subscribe(DoSelect!);
    }

    private void AddFolder()
    {
        _targetViewModel.RestartLockerTimer();
        
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
        
        _targetViewModel.RestartLockerTimer();
        
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

        _targetViewModel.LockerTimer?.Stop();

        try
        {
            Color dataColor;
            dataColor = SelectedFolder.Color == 0
                ? new Color(255, 255, 255, 255)
                : Color.FromUInt32(SelectedFolder.Color);

            Color color = await WindowColorPicker.SelectColor(WindowsManager.MainWindow, dataColor.ToString());

            SelectedFolder.Color = color.ToUint32();
            
            _targetViewModel.IsEdited = true;
        }
        finally
        {
            _targetViewModel.LockerTimer?.Start();
        }
    }

    private void DoSelect(Data data)
    {
        _targetViewModel.SelectedData = data;
        _view.Close();
    }

    private void DoSearch(string str)
    {
        _targetViewModel.RestartLockerTimer();

        string lowerStr = str.ToLower();

        List<Data> results = new(Database.DataList);
        
        if (SearchByLogins)
        {
            results = results
                .FindAll(x => x.TypeId == DataType.Login);
        }
        else if (SearchByBankCard)
        {
            results = results
                .FindAll(x => x.TypeId == DataType.BankCard);
        }
        else if (SearchByPersonalData)
        {
            results = results
                .FindAll(x => x.TypeId == DataType.PersonalData);
        }
        else if (SearchByNotes)
        {
            results = results.FindAll(x => x.TypeId == DataType.Notes);
        }

        if (SelectedFolder is not null)
            results = results.FindAll(x => x.FolderId == SelectedFolder.Id);

        if (SearchByFavorite) 
            results = results.FindAll(x => x.Favorite);

        if (!string.IsNullOrWhiteSpace(str))
        {
            results = results.FindAll(x => (x.Name is not null && x.Name.ToLower().Contains(lowerStr))
                                           || (x.Login is { Username: { }, Email: { } } && (x.Login.Username.ToLower().Contains(lowerStr) 
                                               || x.Login.Email.ToLower().Contains(lowerStr)))
                                           || (x.BankCard?.CardNumber is not null && x.BankCard.CardNumber.ToLower().Contains(lowerStr)
                                               || (x.PersonalData?.Fullname is not null && x.PersonalData.Fullname.ToLower().Contains(lowerStr)))).ToList();
        }

        if (AlphabeticalSorting)
            results = results.OrderBy(x => x.Name).ToList();
        
        FoundedDataList = new AvaloniaList<Data>(results);
    }
}