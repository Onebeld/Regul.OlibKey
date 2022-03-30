using System.Runtime.Serialization;
using System.Xml.Serialization;
using Onebeld.Extensions;

namespace Regul.OlibKey.Structures;

[DataContract]
public class Folder : ViewModelBase
{
    private string _name = null!;
    private string _id = null!;
    private string? _deleteDate;
    private uint _color = 0;
    private bool _useColor;

    [XmlAttribute]
    [DataMember]
    public string Name
    {
        get => _name;
        set => RaiseAndSetIfChanged(ref _name, value);
    }

    [XmlAttribute]
    [DataMember]
    public string Id
    {
        get => _id;
        set => RaiseAndSetIfChanged(ref _id, value);
    }

    [DataMember]
    public string? DeleteDate
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
}