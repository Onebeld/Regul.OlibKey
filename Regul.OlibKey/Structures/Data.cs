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
using Regul.OlibKey.Enums;
using Regul.OlibKey.General;

namespace Regul.OlibKey.Structures;

[DataContract]
public class Data : ViewModelBase, ICloneable
{
    private DataType _typeId = DataType.Login;
    private string _name = null!;
    private string _timeCreate = null!;
    private string _timeChanged = null!;
    private string _deleteDate = null!;
    private uint _color;
    private bool _useColor;
    private string _note = null!;

    private bool _favorite;
    private string _folderId = null!;

    private Login? _login = new();
    private BankCard? _bankCard = new();
    private PersonalData? _personalData = new();

    private AvaloniaList<CustomField> _customFields = new();
    private AvaloniaList<ImportedFile> _importedFiles = new();

    [XmlAttribute]
    [DataMember]
    public DataType TypeId
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
    public Login? Login
    {
        get => _login;
        set => RaiseAndSetIfChanged(ref _login, value);
    }

    [XmlElement(IsNullable = false)]
    [DataMember]
    public BankCard? BankCard
    {
        get => _bankCard;
        set => RaiseAndSetIfChanged(ref _bankCard, value);
    }

    [XmlElement(IsNullable = false)]
    [DataMember]
    public PersonalData? PersonalData
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
        if (TypeId == DataType.Login && Login is not null)
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
                        
                MemoryStream ms = new(Convert.FromBase64String(Login.Image ?? throw new NullReferenceException()));
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
            case DataType.BankCard:
                return App.GetResource<DrawingImage>("CardIcon");
            case DataType.PersonalData:
                return App.GetResource<DrawingImage>("PersonalDataIcon");
            case DataType.Notes:
                return App.GetResource<DrawingImage>("NoteIcon");
                    
            default:
                return App.GetResource<DrawingImage>("UnknownIcon");
        }
    }

    public object Clone()
    {
        Data data = (Data)MemberwiseClone();
        if (Login is not null)
            data.Login = (Login)Login.Clone();
        if (BankCard is not null)
            data.BankCard = (BankCard)BankCard.Clone();
        if (PersonalData is not null)
            data.PersonalData = (PersonalData)PersonalData.Clone();
        data.CustomFields = new AvaloniaList<CustomField>(CustomFields.Select(item => (CustomField)item.Clone()));
        data.ImportedFiles = new AvaloniaList<ImportedFile>(ImportedFiles.Select(item => (ImportedFile)item.Clone()));
            
        return data;
    }
}