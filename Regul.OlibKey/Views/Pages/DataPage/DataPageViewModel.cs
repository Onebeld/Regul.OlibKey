using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using Onebeld.Extensions;
using OtpNet;
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
    private readonly PasswordManagerViewModel _targetViewModel;
    private DataInformation _dataInformation;
    private readonly DataType[] _dataTypes = (DataType[])Enum.GetValues(typeof(DataType));
    private readonly CustomFieldType[] _customFieldTypes = (CustomFieldType[])Enum.GetValues(typeof(CustomFieldType));
    private Database _database = null!;
    private Data _data = null!;
    private int _selectedCustomFieldTypeIndex;

    private Totp? _totp;
    private DispatcherTimer? _totpTimer;

    #region Properties

    public Database Database
    {
        get => _database;
        private set => RaiseAndSetIfChanged(ref _database, value);
    }

    public Data Data
    {
        get => _data;
        private set => RaiseAndSetIfChanged(ref _data, value);
    }

    private int DataIndex
    {
        get
        {
            if (_targetViewModel.Database is null || _targetViewModel.SelectedData is null)
                throw new NullReferenceException();
            
            return _targetViewModel.Database.DataList.IndexOf(_targetViewModel.SelectedData);
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

    public Folder? SelectedFolder
    {
        get => Database.Folders.FirstOrDefault(folder => folder.Id == Data.FolderId);
        set
        {
            Data.FolderId = value?.Id;
            RaisePropertyChanged(nameof(SelectedFolder));
        }
    }

    public string GeneratedCode { get; set; } = string.Empty;
    public int TimeLeft { get; set; }
    public bool IsActivatedTotp { get; set; }

    #endregion

    public DataPageViewModel(DataInformation dataInformation, PasswordManagerViewModel targetViewModel)
    {
        _targetViewModel = targetViewModel;
        Database = _targetViewModel.Database ?? throw new NullReferenceException();
        _dataInformation = dataInformation;
            
        switch (dataInformation)
        {
            case DataInformation.Create:
                Data = new Data();
                break;
            case DataInformation.View:
                Data = (Data)_targetViewModel.SelectedData!.Clone();
                    
                Data.Login ??= new Login();
                Data.BankCard ??= new BankCard();
                Data.PersonalData ??= new PersonalData();
                
                if (Data.Login.IsActivatedTotp) ActivateTotp();
                break;
                
            case DataInformation.Edit:
            default:
                throw new ArgumentOutOfRangeException(nameof(dataInformation), dataInformation, null);
        }
        
        _targetViewModel.RestartLockerTimer();
            
        RaisePropertyChanged(nameof(IsView));
        RaisePropertyChanged(nameof(IsEdit));
        RaisePropertyChanged(nameof(IsCreate));
    }

    private void SaveData()
    {
        _targetViewModel.RestartLockerTimer();
        
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

                _targetViewModel.Page = new StartPage();
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
                _targetViewModel.SelectedData = Database.DataList[index];
                break;
                
            case DataInformation.View:
            default:
                throw new ArgumentOutOfRangeException(nameof(_dataInformation), _dataInformation, null);
        }
        
        _targetViewModel.IsEdited = true;
    }

    private void ChangeData()
    {
        _targetViewModel.RestartLockerTimer();
        
        _dataInformation = DataInformation.Edit;
            
        RaisePropertyChanged(nameof(IsView));
        RaisePropertyChanged(nameof(IsEdit));
        RaisePropertyChanged(nameof(IsCreate));
    }

    private void DeleteData()
    {
        _targetViewModel.RestartLockerTimer();
        
        int index = DataIndex;
        
        if (Database.Settings.UseTrashcan)
        {
            Data data = Database.DataList[index];
            data.DeleteDate = DateTime.Now.ToString(CultureInfo.CurrentCulture);
            Database.Trashcan.DataList.Add(data);
        }

        Database.DataList.RemoveAt(index);

        _targetViewModel.IsEdited = true;
    }

    private void Cancel()
    {
        _targetViewModel.RestartLockerTimer();
        
        _dataInformation = DataInformation.View;

        Data = (Data)Database.DataList[DataIndex].Clone();
            
        RaisePropertyChanged(nameof(IsView));
        RaisePropertyChanged(nameof(IsEdit));
        RaisePropertyChanged(nameof(IsCreate));
        RaisePropertyChanged(nameof(SelectedFolder));
    }

    private void Back()
    {
        _targetViewModel.RestartLockerTimer();
        
        if (_targetViewModel.SelectedData is null) 
            _targetViewModel.Page = new StartPage();
        
        _targetViewModel.SelectedData = null;
    }

    private async void CopyString(string s)
    {
        _targetViewModel.RestartLockerTimer();
        
        Application.Current?.Clipboard?.SetTextAsync(s);

        if (OlibKeySettings.Instance.ClearClipboard)
        {
            await Task.Delay(TimeSpan.FromSeconds(OlibKeySettings.Instance.ClearingTime));
            Application.Current?.Clipboard?.ClearAsync();
            
            WindowsManager.ShowNotification(App.GetResource<string>("ClipboardCleared"), NotificationType.Information);
        }
    }

    private async void ChangeColor()
    {
        _targetViewModel.LockerTimer?.Stop();

        try
        {
            Color dataColor;
            dataColor = Data.Color == 0
                ? new Color(255, 255, 255, 255)
                : Color.FromUInt32(Data.Color);

            Color color = await WindowColorPicker.SelectColor(WindowsManager.MainWindow, dataColor.ToString());

            Data.Color = color.ToUint32();
        }
        finally
        {
            _targetViewModel.LockerTimer?.Start();
        }
    }

    public async void GeneratePassword()
    {
        _targetViewModel.LockerTimer?.Stop();
        
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
        
        _targetViewModel.LockerTimer?.Start();
    }

    private void SelectNullFolder()
    {
        _targetViewModel.RestartLockerTimer();
        SelectedFolder = null;
    }

    #region ImportFile

    private async void ImportFile()
    {
        _targetViewModel.LockerTimer?.Stop();
        
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
        
        _targetViewModel.LockerTimer?.Start();
    }

    private async void ExportFile(ImportedFile importedFile)
    {
        _targetViewModel.LockerTimer?.Stop();
        
        SaveFileDialog saveFileDialog = new()
        {
            InitialFileName = importedFile.Name
        };
        string? path = await saveFileDialog.ShowAsync(WindowsManager.MainWindow);

        if (path is null) return;
            
        FileInteractions.ExportFile(importedFile.Data, path);
        
        _targetViewModel.LockerTimer?.Start();
    }

    private void DeleteFile(ImportedFile importedFile)
    {
        _targetViewModel.RestartLockerTimer();
        Data.ImportedFiles.Remove(importedFile);
    }

    #endregion

    #region CustomFields

    private void AddCustomField()
    {
        _targetViewModel.RestartLockerTimer();
        
        CustomFieldType typeId = _customFieldTypes[SelectedCustomFieldTypeIndex];

        Data.CustomFields.Add(new CustomField
        {
            TypeId = typeId
        });
    }

    private void DeleteCustomField(CustomField customField)
    {
        _targetViewModel.RestartLockerTimer();
        Data.CustomFields.Remove(customField);
    }

    #endregion

    #region TOTP

    private void ActivateTotp()
    {
        if (Data.Login is null || string.IsNullOrWhiteSpace(Data.Login.SecretKey)) 
            return;
        
        if (_totpTimer is not null && _totpTimer.IsEnabled)
            _totpTimer.Stop();

        _totp = new Totp(Base32Encoding.ToBytes(Data.Login.SecretKey!.Replace(" ", "")), 
            step: 30,
            timeCorrection: new TimeCorrection(DateTime.UtcNow));

        GeneratedCode = _totp.ComputeTotp();
        TimeLeft = _totp.RemainingSeconds();
        IsActivatedTotp = true;
        Data.Login.IsActivatedTotp = true;
        
        RaisePropertyChanged(nameof(GeneratedCode));
        RaisePropertyChanged(nameof(TimeLeft));
        RaisePropertyChanged(nameof(IsActivatedTotp));

        _totpTimer = new DispatcherTimer
        {
            Interval = new TimeSpan(0, 0, 0, 0, 200)
        };
        _totpTimer.Tick += OnTickTotpTimer;
        _totpTimer.Start();
    }

    public void DeactivateTotp()
    {
        if (_totpTimer is null || _totp is null || Data.Login is null)
            return;
        
        _totpTimer.Stop();
        _totp = null;
        
        GeneratedCode = string.Empty;
        TimeLeft = 0;
        IsActivatedTotp = false;
        Data.Login.IsActivatedTotp = true;

        RaisePropertyChanged(nameof(GeneratedCode));
        RaisePropertyChanged(nameof(TimeLeft));
        RaisePropertyChanged(nameof(IsActivatedTotp));
    }

    private void OnTickTotpTimer(object sender, EventArgs e)
    {
        if (_totp is null)
            return;
        
        GeneratedCode = _totp.ComputeTotp();
        TimeLeft = _totp.RemainingSeconds();
        
        RaisePropertyChanged(nameof(GeneratedCode));
        RaisePropertyChanged(nameof(TimeLeft));
    }

    #endregion
}