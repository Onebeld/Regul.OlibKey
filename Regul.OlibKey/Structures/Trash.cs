using System.Runtime.Serialization;
using Avalonia.Collections;
using Onebeld.Extensions;

namespace Regul.OlibKey.Structures
{
    [DataContract]
    public class Trash : ViewModelBase
    {
        private AvaloniaList<Data> _dataList = new AvaloniaList<Data>();
        private AvaloniaList<Folder> _folders = new AvaloniaList<Folder>();

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
}