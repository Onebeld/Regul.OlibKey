using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Threading;
using Onebeld.Extensions;
using PleasantUI.Controls.Custom;
using Regul.Base;
using Regul.ModuleSystem;
using Regul.OlibKey.Enums;
using Regul.OlibKey.General;
using Regul.OlibKey.Structures;
using Regul.OlibKey.Views.Pages;
using Regul.OlibKey.Views.PasswordManager.Content;
using Regul.OlibKey.Views.Windows;

namespace Regul.OlibKey.Views;

public class PasswordManagerViewModel : ViewModelBase
{
    private Database? _database;
    private IPage? _page;
    private Control _content = null!;
    private Data? _selectedData;
    private bool _isNotCreatedDatabase;
    private string _masterPassword = string.Empty;
    private string? _filePath;

    public DispatcherTimer? LockerTimer;

    #region Properties

    public bool IsEdited
    {
        get => PleasantTabItem.IsEditedIndicator;
        set
        {
            PleasantTabItem.IsEditedIndicator = value;
            RaisePropertyChanged(nameof(IsEdited));
        }
    }

    public Database? Database
    {
        get => _database;
        private set => RaiseAndSetIfChanged(ref _database, value);
    }

    public Data? SelectedData
    {
        get => _selectedData;
        set => RaiseAndSetIfChanged(ref _selectedData, value);
    }

    public IPage? Page
    {
        get => _page;
        set => RaiseAndSetIfChanged(ref _page, value);
    }

    public Control Content
    {
        get => _content;
        set => RaiseAndSetIfChanged(ref _content, value);
    }

    public bool IsNotCreatedDatabase
    {
        get => _isNotCreatedDatabase;
        set => RaiseAndSetIfChanged(ref _isNotCreatedDatabase, value);
    }

    public string MasterPassword
    {
        get => _masterPassword;
        set => RaiseAndSetIfChanged(ref _masterPassword, value);
    }
    
    #endregion

    private PleasantTabItem PleasantTabItem { get; } = null!;
    private PasswordManagerView PasswordManagerView { get; } = null!;

    public PasswordManagerViewModel()
    {
        this.WhenAnyValue(x => x.SelectedData)
            .Subscribe(DoSelectData);
    }

    public PasswordManagerViewModel(
        PleasantTabItem pleasantTabItem,
        Editor editor,
        PasswordManagerView passwordManagerView,
        string? filePath = null) : this()
    {
        PleasantTabItem = pleasantTabItem;
        PasswordManagerView = passwordManagerView;
        PasswordManagerView.Editor = editor;

        _filePath = filePath;

        if (string.IsNullOrEmpty(filePath))
            IsNotCreatedDatabase = true;

        Content = new CreateUnblockDatabaseContent();

        IsEdited = false;
    }

    private void DoSelectData(Data? data)
    {
        if (Page is DataPage dataPage)
        {
            DataPageViewModel viewModel = dataPage.GetDataContext<DataPageViewModel>();
            if (viewModel.IsActivatedTotp)
                viewModel.DeactivateTotp();
        }
        
        if (data is null) 
            Page = new StartPage();
        else Page = new DataPage(DataInformation.View, this);
    }

    private void AddData()
    {
        SelectedData = null;

        Page = new DataPage(DataInformation.Create, this);
    }

    private async void CreateDatabase()
    {
        CreateDatabase createDatabase = new();

        if (!await createDatabase.Show<bool>(WindowsManager.MainWindow)) return;

        CreateDatabaseViewModel viewModel =
            createDatabase.GetDataContext<CreateDatabaseViewModel>();

        MasterPassword = viewModel.MasterPassword;

        Database = new Database
        {
            Settings = new DatabaseSettings
            {
                Iterations = viewModel.Iteration,
                NumberOfEncryptionProcedures = viewModel.NumberOfEncryptionProcedures,
                UseCompress = viewModel.UseCompress,
                UseTrashcan = viewModel.UseTrashcan
            }
        };

        IsNotCreatedDatabase = false;
        IsEdited = true;
        Content = new MainContent();
        Page = new StartPage();

        if (OlibKeySettings.Instance.LockStorage) 
            ActivateLockerTimer();
    }

    private void OpenDatabase()
    {
        if (_filePath is null || !File.Exists(_filePath)) return;

        try
        {
            Database = Database.Load(_filePath, MasterPassword);
        }
        catch
        {
            WindowsManager.ShowNotification(App.GetResource<string>("IncorrectMasterPasswordMessage"), NotificationType.Error);

            return;
        }

        if (Database.Settings.UseTrashcan)
        {
            for (int index = Database.Trashcan.DataList.Count - 1; index >= 0; index--)
            {
                Data data = Database.Trashcan.DataList[index];
                
                if (OlibKeySettings.Instance.CleanTrashcan)
                {
                    DateTime cleaningTime = DateTime.Parse(data.DeleteDate)
                        .AddDays(OlibKeySettings.Instance.ClearingTrashcanTime);
                
                    if (DateTime.Now > cleaningTime)
                    {
                        Database.Trashcan.DataList.RemoveAt(index);
                        continue;
                    }
                }

                data.IsDeleted = true;
            }
            
            for (int index = Database.Trashcan.Folders.Count - 1; index >= 0; index--)
            {
                Folder folder = Database.Trashcan.Folders[index];

                if (OlibKeySettings.Instance.CleanTrashcan)
                {
                    DateTime cleaningTime = DateTime.Parse(folder.DeleteDate)
                        .AddDays(OlibKeySettings.Instance.ClearingTrashcanTime);
                
                    if (DateTime.Now > cleaningTime)
                    {
                        Database.Trashcan.Folders.RemoveAt(index);
                        continue;
                    }
                }

                folder.IsDeleted = true;
            }
        }

        Content = new MainContent();
        Page = new StartPage();

        IsEdited = false;
        
        if (OlibKeySettings.Instance.LockStorage)
            ActivateLockerTimer();
    }

    private void LockDatabase()
    {
        if (Database is null || _filePath is null)
            throw new NullReferenceException();
        
        Database.Save(_filePath, MasterPassword);
        Database = null;
        MasterPassword = string.Empty;

        IsEdited = false;

        Content = new CreateUnblockDatabaseContent();
        Page = null;
        
        if (OlibKeySettings.Instance.LockStorage) 
            LockerTimer?.Stop();
    }

    private void OpenDatabaseSettings()
    {
        RestartLockerTimer();
        
        if (Database is null) return;
        
        DatabaseSettingsView? window = WindowsManager.CreateModalWindow<DatabaseSettingsView>();
        
        if (window is null) return;

        window.DataContext = Database.Settings;
        window.Show(WindowsManager.MainWindow);
    }

    private void OpenTrashcan()
    {
        RestartLockerTimer();
        
        if (Database is null) return;
        
        TrashcanView? window = WindowsManager.CreateModalWindow<TrashcanView>();

        if (window is null) return;

        window.DataContext = new TrashcanViewModel(this, Database);
        window.Show(WindowsManager.MainWindow);
    }

    private void OpenSearch()
    {
        RestartLockerTimer();
        
        if (Database is null) return;

        SearchView? window = WindowsManager.CreateModalWindow<SearchView>();
        
        if (window is null) return;
        
        SearchViewModel viewModel = new(window, this, Database);
        window.DataContext = viewModel;
        window.Show(WindowsManager.MainWindow);
    }

    private void OpenPasswordChecker()
    {
        RestartLockerTimer();
        
        if (Database is null) return;

        PasswordCheckerView? window = WindowsManager.CreateModalWindow<PasswordCheckerView>();
        
        if (window is null) return;
        
        window.DataContext = new PasswordCheckerViewModel(this, Database);
        window.Show(WindowsManager.MainWindow);
    }

    private async void ChangeMasterPassword()
    {
        RestartLockerTimer();
        
        ChangingMasterPassword? window = WindowsManager.CreateModalWindow<ChangingMasterPassword>();
        
        if (window is null) return;
        
        ChangingMasterPasswordViewModel viewModel = new(MasterPassword);
        window.DataContext = viewModel;

        if (!await window.Show<bool>(WindowsManager.MainWindow)) return;
        
        MasterPassword = viewModel.NewMasterPassword;

        WindowsManager.ShowNotification(App.GetResource<string>("SuccessChangedMasterPasswordMessage"));
    }

    public void ActivateLockerTimer()
    {
        LockerTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMinutes(OlibKeySettings.Instance.LockoutTime)
        };
        LockerTimer.Tick += (_, _) =>
        {
            for (int index = WindowsManager.OtherModalWindows.Count - 1; index >= 0; index--)
            {
                PleasantModalWindow? pleasantModalWindow = WindowsManager.OtherModalWindows[index];
                
                if (pleasantModalWindow is IMustCloseWhenLocked)
                    pleasantModalWindow.Close();
            }

            LockDatabase();
                
            WindowsManager.ShowNotification(
                App.GetResource<string>("StorageLocked") + " " + Path.GetFileName(((PasswordManagerView)PleasantTabItem.Content).Project.Path),
                NotificationType.Information);
        };
            
        LockerTimer.Start();
    }

    public void RestartLockerTimer()
    {
        if (LockerTimer is null || !LockerTimer.IsEnabled) return;
        
        LockerTimer.Stop();
        LockerTimer.Start();
    }

    public bool Save(string? path)
    {
        _filePath = path;
        PasswordManagerView.FilePath = path;
        
        if (Database is null || _filePath is null)
            return false;

        if (!Database.Settings.UseTrashcan)
        {
            Database.Trashcan.DataList.Clear();
            Database.Trashcan.Folders.Clear();
        }

        Database.Save(_filePath, MasterPassword);
        IsEdited = false;

        return true;
    }
}