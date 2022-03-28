using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Onebeld.Extensions;
using PleasantUI.Controls.Custom;
using PleasantUI.Windows;
using Regul.Base;
using Regul.OlibKey.Enums;
using Regul.OlibKey.General;
using Regul.OlibKey.Structures;
using Regul.OlibKey.Views.Windows;

namespace Regul.OlibKey.Views.Pages;

public class DataPageViewModel : ViewModelBase
{
    private readonly PasswordManagerViewModel _viewModel;

    private DataInformation _dataInformation;
    private readonly DataType[] _dataTypes = (DataType[])Enum.GetValues(typeof(DataType));
    private readonly CustomFieldType[] _customFieldTypes = (CustomFieldType[])Enum.GetValues(typeof(CustomFieldType));
        
    private Database _database;
    private Data _data;

    private int _selectedCustomFieldTypeIndex = 0;

    #region Properties

    public Database Database
    {
        get => _database;
        set => RaiseAndSetIfChanged(ref _database, value);
    }

    public Data Data
    {
        get => _data;
        set => RaiseAndSetIfChanged(ref _data, value);
    }

    private int DataIndex
    {
        get
        {
            if (_viewModel.Database is null || _viewModel.SelectedData is null)
                throw new NullReferenceException();
            
            return _viewModel.Database.DataList.IndexOf(_viewModel.SelectedData);
        }
    }

    public bool IsView => _dataInformation == DataInformation.View;
    public bool IsEdit => _dataInformation == DataInformation.Edit;
    public bool IsCreate => _dataInformation == DataInformation.Create;

    public int SelectedTypeIndex
    {
        get => Array.IndexOf(_dataTypes, Data.TypeId);
        set => Data.TypeId = _dataTypes[value];
    }

    public int SelectedCustomFieldTypeIndex
    {
        get => _selectedCustomFieldTypeIndex;
        set => RaiseAndSetIfChanged(ref _selectedCustomFieldTypeIndex, value);
    }

    public Folder SelectedFolder
    {
        get => Database.Folders.FirstOrDefault(folder => folder.Id == Data.FolderId) ?? throw new NullReferenceException();
        set => Data.FolderId = value.Id;
    }

    #endregion

    public DataPageViewModel(DataInformation dataInformation, PasswordManagerViewModel viewModel)
    {
        _viewModel = viewModel;
        Database = _viewModel.Database ?? throw new NullReferenceException();
        _dataInformation = dataInformation;
            
        switch (dataInformation)
        {
            case DataInformation.Create:
                Data = new Data();
                break;
            case DataInformation.View:
                Data = (Data)_viewModel.SelectedData!.Clone();
                    
                Data.Login ??= new Login();
                Data.BankCard ??= new BankCard();
                Data.PersonalData ??= new PersonalData();
                break;
                
            case DataInformation.Edit:
            default:
                throw new ArgumentOutOfRangeException(nameof(dataInformation), dataInformation, null);
        }
            
        RaisePropertyChanged(nameof(IsView));
        RaisePropertyChanged(nameof(IsEdit));
        RaisePropertyChanged(nameof(IsCreate));
    }

    private void SaveData()
    {
        switch (Data.TypeId)
        {
            case DataType.BankCard:
                Data.Login = null;
                Data.PersonalData = null;
                break;
                
            case DataType.PersonalData:
                Data.BankCard = null;
                Data.Login = null;
                break;
                
            case DataType.Notes:
                Data.BankCard = null;
                Data.PersonalData = null;
                Data.Login = null;
                break;
                    
            default:
                Data.BankCard = null;
                Data.PersonalData = null;
                break;
        }

        switch (_dataInformation)
        {
            case DataInformation.Create:
                Data.TimeCreate = DateTime.Now.ToString(CultureInfo.CurrentCulture);
                Database.DataList.Add(Data);

                _viewModel.Page = new StartPage();
                break;
            case DataInformation.Edit:
                int index = DataIndex;
                    
                if (Data.TypeId == DataType.Login && Database.DataList[index].TypeId == DataType.Login)
                {
                    if (Database.DataList[index].Login?.WebSite != Data.Login?.WebSite)
                        Data.IsIconChange = true;
                }
                    
                if (Data.TypeId != Database.DataList[index].TypeId)
                    Data.IsIconChange = true;

                Data.TimeChanged = DateTime.Now.ToString(CultureInfo.CurrentCulture);

                Database.DataList[index] = Data;

                _viewModel.SelectedData = _viewModel.Database?.DataList[index];
                break;
                
            case DataInformation.View:
            default:
                throw new ArgumentOutOfRangeException();
        }

        _viewModel.IsEdited = true;
    }

    private void ChangeData()
    {
        _dataInformation = DataInformation.Edit;
            
        RaisePropertyChanged(nameof(IsView));
        RaisePropertyChanged(nameof(IsEdit));
        RaisePropertyChanged(nameof(IsCreate));
    }

    private void DeleteData() => Database.DataList.RemoveAt(DataIndex);

    private void Cancel()
    {
        _dataInformation = DataInformation.View;

        Data = (Data)_viewModel.Database?.DataList[DataIndex].Clone()! ?? throw new NullReferenceException();
            
        RaisePropertyChanged(nameof(IsView));
        RaisePropertyChanged(nameof(IsEdit));
        RaisePropertyChanged(nameof(IsCreate));
    }

    private void Back() => _viewModel.SelectedData = null;

    private void CopyString(string s) => Application.Current?.Clipboard?.SetTextAsync(s);

    private async void ChangeColor()
    {
        try
        {
            Color dataColor;
            dataColor = Data.Color == 0 
                ? new Color(255, 255, 255, 255) 
                : Color.FromUInt32(Data.Color);
                
            Color color = await WindowColorPicker.SelectColor(WindowsManager.MainWindow, dataColor.ToString());

            Data.Color = color.ToUint32();
        }
        catch
        {
            // ignored
        }
    }

    public async void GeneratePassword()
    {
        PleasantDialogWindow window = new()
        {
            Content = new PasswordGeneratorView(),
            Icon = new Bitmap(AvaloniaLocator.Current.GetService<IAssetLoader>()
                ?.Open(new Uri("avares://Regul.Assets/icon.ico")))
        };
        PasswordGeneratorViewModel viewModel = new(window, true);
        window.DataContext = viewModel;
        window.Bind(PleasantDialogWindow.TitleProperty, new DynamicResourceExtension("PasswordGenerator"));

        string password = await window.Show<string>(WindowsManager.MainWindow);

        if (Data.Login is not null && !string.IsNullOrEmpty(password))
            Data.Login.Password = password;
    }

    #region ImportFile

    private async void ImportFile()
    {
        OpenFileDialog dialog = new() { AllowMultiple = true };

        string[]? files = await dialog.ShowAsync(WindowsManager.MainWindow);

        if (files is null) return;

        foreach (string file in files)
        {
            Data.ImportedFiles.Add(new ImportedFile
            {
                Name = Path.GetFileName(file),
                Data = FileInteractions.ImportFile(file)
            });
        }
    }

    private async void ExportFile(ImportedFile importedFile)
    {
        SaveFileDialog saveFileDialog = new()
        {
            InitialFileName = importedFile.Name
        };
        string? path = await saveFileDialog.ShowAsync(WindowsManager.MainWindow);

        if (path is null) return;
            
        FileInteractions.ExportFile(importedFile.Data, path);
    }

    private void DeleteFile(ImportedFile importedFile) => Data.ImportedFiles.Remove(importedFile);

    #endregion

    #region CustomFields

    private void AddCustomField()
    {
        CustomFieldType typeId = _customFieldTypes[SelectedCustomFieldTypeIndex];

        Data.CustomFields.Add(new CustomField
        {
            TypeId = typeId
        });
    }

    private void DeleteCustomField(CustomField customField) => Data.CustomFields.Remove(customField);

    #endregion
}