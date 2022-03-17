using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Media;
using Onebeld.Extensions;
using PleasantUI.Windows;
using Regul.Base;
using Regul.OlibKey.Enums;
using Regul.OlibKey.General;
using Regul.OlibKey.Structures;

namespace Regul.OlibKey.Views.Pages
{
    public class DataPageViewModel : ViewModelBase
    {
        private DataInformation _dataInformation;
        
        private Database _database;
        private Data _data;
        private readonly PasswordManagerViewModel _viewModel;

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

        private int DataIndex => _viewModel.Database.DataList.IndexOf(_viewModel.SelectedData);

        public bool IsView => _dataInformation == DataInformation.View;
        public bool IsEdit => _dataInformation == DataInformation.Edit;
        public bool IsCreate => _dataInformation == DataInformation.Create;

        public int SelectedTypeIndex
        {
            get
            {
                switch (Data.TypeId)
                {
                    case DataType.BankCard:
                        return 1;
                    case DataType.PersonalData:
                        return 2;
                    case DataType.Notes:
                        return 3;

                    case DataType.Login:
                    default:
                        return 0;
                }
            }
            set
            {
                switch (value)
                {
                    
                    case 1:
                        Data.TypeId = DataType.BankCard;
                        break;
                    case 2:
                        Data.TypeId = DataType.PersonalData;
                        break;
                    case 3:
                        Data.TypeId = DataType.Notes;
                        break;

                    default:
                        Data.TypeId = DataType.Login;
                        break;
                }
            }
        }

        public int SelectedCustomFieldTypeIndex
        {
            get => _selectedCustomFieldTypeIndex;
            set => RaiseAndSetIfChanged(ref _selectedCustomFieldTypeIndex, value);
        }

        public Folder SelectedFolder
        {
            get => Database.Folders.FirstOrDefault(folder => folder.Id == Data.FolderId);
            set => Data.FolderId = value.Id;
        }

        #endregion

        public DataPageViewModel(DataInformation dataInformation, PasswordManagerViewModel viewModel)
        {
            _viewModel = viewModel;
            Database = _viewModel.Database;
            _dataInformation = dataInformation;
            
            switch (dataInformation)
            {
                case DataInformation.Create:
                    Data = new Data();
                    break;
                case DataInformation.View:
                    Data = (Data)_viewModel.SelectedData.Clone();

                    if (Data.Login == null)
                        Data.Login = new Login();
                    if (Data.BankCard == null)
                        Data.BankCard = new BankCard();
                    if (Data.PersonalData == null)
                        Data.PersonalData = new PersonalData();
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
            switch (SelectedTypeIndex)
            {
                case 1:
                    Data.Login = null;
                    Data.PersonalData = null;
                    break;
                case 2:
                    Data.BankCard = null;
                    Data.Login = null;
                    break;
                case 3:
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
                        if (Database.DataList[index].Login.WebSite != Data.Login?.WebSite)
                            Data.IsIconChange = true;
                    }
                    
                    if (Data.TypeId != Database.DataList[index].TypeId)
                        Data.IsIconChange = true;

                    Data.TimeChanged = DateTime.Now.ToString(CultureInfo.CurrentCulture);

                    Database.DataList[index] = Data;

                    _viewModel.SelectedData = _viewModel.Database.DataList[index];
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

            Data = (Data)_viewModel.Database.DataList[DataIndex].Clone();
            
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
                Color color;
                if (Data.Color == 0)
                {
                    color = await WindowColorPicker.SelectColor(WindowsManager.MainWindow, new Color(255, 255, 255, 255).ToString());
                }
                else color =
                    await WindowColorPicker.SelectColor(WindowsManager.MainWindow, Color.FromUInt32(Data.Color).ToString());

                Data.Color = color.ToUint32();
            }
            catch { }
        }

        #region ImportFile

        private async void ImportFile()
        {
            if (Data.ImportedFiles == null)
                Data.ImportedFiles = new AvaloniaList<ImportedFile>();
            
            OpenFileDialog dialog = new OpenFileDialog { AllowMultiple = true };

            string[] files = await dialog.ShowAsync(WindowsManager.MainWindow);

            if (files == null) return;

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
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                InitialFileName = importedFile.Name
            };
            string path = await saveFileDialog.ShowAsync(WindowsManager.MainWindow);

            if (path != null)
            {
                FileInteractions.ExportFile(importedFile.Data, path);
            }
        }

        private void DeleteFile(ImportedFile importedFile) => Data.ImportedFiles.Remove(importedFile);

        #endregion

        #region CustomFields

        private void AddCustomField()
        {
            if (Data.CustomFields == null)
                Data.CustomFields = new AvaloniaList<CustomField>();
            
            CustomFieldType typeId;

            switch (SelectedCustomFieldTypeIndex)
            {
                case 1:
                    typeId = CustomFieldType.Password;
                    break;
                case 2:
                    typeId = CustomFieldType.Check;
                    break;
                
                default:
                    typeId = CustomFieldType.Text;
                    break;
            }
            
            Data.CustomFields.Add(new CustomField
            {
                TypeId = typeId
            });
        }

        private void DeleteCustomField(CustomField customField) => Data.CustomFields.Remove(customField);

        #endregion
    }
}