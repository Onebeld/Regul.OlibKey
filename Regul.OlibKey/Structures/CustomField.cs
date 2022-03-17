using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Onebeld.Extensions;
using Regul.OlibKey.Enums;

namespace Regul.OlibKey.Structures
{
    [DataContract]
    public class CustomField : ViewModelBase, ICloneable
    {
        private string _name;
        private CustomFieldType _typeId;
        private string _textElement;
        private bool _checkedElement;

        [XmlAttribute]
        [DataMember]
        public string Name
        {
            get => _name;
            set => RaiseAndSetIfChanged(ref _name, value);
        }

        [XmlAttribute]
        [DataMember]
        public CustomFieldType TypeId
        {
            get => _typeId;
            set => RaiseAndSetIfChanged(ref _typeId, value);
        }

        [DataMember]
        public string TextElement
        {
            get => _textElement;
            set => RaiseAndSetIfChanged(ref _textElement, value);
        }

        [DataMember]
        public bool CheckedElement
        {
            get => _checkedElement;
            set => RaiseAndSetIfChanged(ref _checkedElement, value);
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}