﻿using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Onebeld.Extensions;
using PleasantUI.Controls.Custom;
using Regul.Base;
using Regul.Base.Views.Windows;
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
                UseTrash = viewModel.UseTrash
            }
        };

        IsNotCreatedDatabase = false;
        IsEdited = true;
        Content = new MainContent();
        Page = new StartPage();
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

        Content = new MainContent();
        Page = new StartPage();

        IsEdited = false;
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
    }

    private void OpenDatabaseSettings()
    {
        if (Database is null) return;

        DatabaseSettingsView view = new()
        {
            DataContext = Database.Settings
        };

        view.Show(WindowsManager.MainWindow);
    }

    private async void ChangeMasterPassword()
    {
        ChangingMasterPasswordViewModel viewModel = new(MasterPassword);
        ChangingMasterPassword window = new()
        {
            DataContext = viewModel
        };

        if (await window.Show<bool>(WindowsManager.MainWindow))
        {
            MasterPassword = viewModel.NewMasterPassword;

            WindowsManager.ShowNotification(App.GetResource<string>("SuccessChangedMasterPasswordMessage"));
        }
    }

    public bool Save(string? path)
    {
        _filePath = path;
        PasswordManagerView.FilePath = path;
        
        if (Database is null || _filePath is null)
            return false;

        Database.Save(_filePath, MasterPassword);
        IsEdited = false;

        return true;
    }
}