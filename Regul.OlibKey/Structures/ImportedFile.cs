using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Onebeld.Extensions;

namespace Regul.OlibKey.Structures
{
    [DataContract]
    public class ImportedFile : ViewModelBase, ICloneable
    {
        private string _name;
        private string _data;

        [XmlAttribute]
        [DataMember]
        public string Name
        {
            get => _name;
            set => RaiseAndSetIfChanged(ref _name, value);
        }

        [DataMember]
        public string Data
        {
            get => _data;
            set => RaiseAndSetIfChanged(ref _data, value);
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}