using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Avalonia.Collections;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Onebeld.Extensions;
using Regul.Base;
using Regul.OlibKey.General;

namespace Regul.OlibKey.Structures
{
    [DataContract]
    public class Data : ViewModelBase, ICloneable
    {
        private string _typeId;
        private string _name;
        private string _timeCreate;
        private string _timeChanged;
        private string _deleteDate;
        private uint _color;
        private bool _useColor;
        private string _note;

        private bool _favorite;
        private string _folderId;

        private Login _login = new Login();
        private BankCard _bankCard = new BankCard();
        private PersonalData _personalData = new PersonalData();

        private AvaloniaList<CustomField> _customFields = new AvaloniaList<CustomField>();
        private AvaloniaList<ImportedFile> _importedFiles = new AvaloniaList<ImportedFile>();

        [XmlAttribute]
        [DataMember]
        public string TypeId
        {
            get => _typeId;
            set => RaiseAndSetIfChanged(ref _typeId, value);
        }

        [XmlAttribute]
        [DataMember]
        public string Name
        {
            get => _name;
            set => RaiseAndSetIfChanged(ref _name, value);
        }

        [DataMember]
        public string TimeCreate
        {
            get => _timeCreate;
            set => RaiseAndSetIfChanged(ref _timeCreate, value);
        }

        [DataMember]
        public string TimeChanged
        {
            get => _timeChanged;
            set => RaiseAndSetIfChanged(ref _timeChanged, value);
        }

        [DataMember]
        public string DeleteDate
        {
            get => _deleteDate;
            set => RaiseAndSetIfChanged(ref _deleteDate, value);
        }

        [DataMember]
        public uint Color
        {
            get => _color;
            set => RaiseAndSetIfChanged(ref _color, value);
        }

        [DataMember]
        public bool UseColor
        {
            get => _useColor;
            set => RaiseAndSetIfChanged(ref _useColor, value);
        }

        [DataMember]
        public string Note
        {
            get => _note;
            set => RaiseAndSetIfChanged(ref _note, value);
        }

        [XmlAttribute]
        [DataMember]
        public bool Favorite
        {
            get => _favorite;
            set => RaiseAndSetIfChanged(ref _favorite, value);
        }

        [DataMember]
        public string FolderId
        {
            get => _folderId;
            set => RaiseAndSetIfChanged(ref _folderId, value);
        }

        [XmlElement(IsNullable = false)]
        [DataMember]
        public Login Login
        {
            get => _login;
            set => RaiseAndSetIfChanged(ref _login, value);
        }

        [XmlElement(IsNullable = false)]
        [DataMember]
        public BankCard BankCard
        {
            get => _bankCard;
            set => RaiseAndSetIfChanged(ref _bankCard, value);
        }

        [XmlElement(IsNullable = false)]
        [DataMember]
        public PersonalData PersonalData
        {
            get => _personalData;
            set => RaiseAndSetIfChanged(ref _personalData, value);
        }

        [DataMember]
        public AvaloniaList<CustomField> CustomFields
        {
            get => _customFields;
            set => RaiseAndSetIfChanged(ref _customFields, value);
        }

        [DataMember]
        public AvaloniaList<ImportedFile> ImportedFiles
        {
            get => _importedFiles;
            set => RaiseAndSetIfChanged(ref _importedFiles, value);
        }

        [XmlIgnore]
        public bool IsIconChange = false;

        [XmlIgnore]
        public Task<IImage> Icon => GetIcon();

        private async Task<IImage> GetIcon()
        {
            if (TypeId == "DT_Login")
            {
                if (string.IsNullOrEmpty(Login.WebSite))
                {
                    Login.Image = null;
                    
                    return App.GetResource<DrawingImage>("GlobeIcon");
                }
                
                try
                {
                    if (IsIconChange || string.IsNullOrEmpty(Login.Image))
                        Login.Image = await ImageInteraction.DownloadImage(Login.WebSite);

                    IsIconChange = false;
                        
                    MemoryStream ms = new MemoryStream(Convert.FromBase64String(Login.Image));
                    return new Bitmap(ms);
                }
                catch
                {
                    Login.Image = null;
                    return App.GetResource<DrawingImage>("GlobeIcon");
                }
            }

            switch (TypeId)
            {
                case "DT_BankCard":
                    return App.GetResource<DrawingImage>("CardIcon");
                case "DT_PersonalData":
                    return App.GetResource<DrawingImage>("PersonalDataIcon");
                case "DT_Notes":
                    return App.GetResource<DrawingImage>("NoteIcon");
                    
                default:
                    return App.GetResource<DrawingImage>("UnknownIcon");
            }
        }

        public object Clone()
        {
            Data data = (Data)MemberwiseClone();
            if (Login != null)
                data.Login = (Login)Login.Clone();
            if (BankCard != null)
                data.BankCard = (BankCard)BankCard.Clone();
            if (PersonalData != null)
                data.PersonalData = (PersonalData)PersonalData.Clone();
            data.CustomFields = new AvaloniaList<CustomField>(CustomFields.Select(item => (CustomField)item.Clone()));
            data.ImportedFiles = new AvaloniaList<ImportedFile>(ImportedFiles.Select(item => (ImportedFile)item.Clone()));
            
            return data;
        }
    }
}