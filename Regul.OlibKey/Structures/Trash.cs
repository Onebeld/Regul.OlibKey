using System.Runtime.Serialization;
using Avalonia.Collections;
using Onebeld.Extensions;

namespace Regul.OlibKey.Structures;

[DataContract]
public class Trashcan : ViewModelBase
{
    private AvaloniaList<Data> _dataList = new();
    private AvaloniaList<Folder> _folders = new();

    [DataMember]
    public AvaloniaList<Data> DataList
    {
        get => _dataList;
        set => RaiseAndSetIfChanged(ref _dataList, value);
    }

    [DataMember]
    public AvaloniaList<Folder> Folders
    {
        get => _folders;
        set => RaiseAndSetIfChanged(ref _folders, value);
    }
}