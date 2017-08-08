using System.Xml.Serialization;

namespace ModernApplicationFramework.Extended.KeyBindingScheme
{
    [System.Serializable]
    [XmlType(AnonymousType = true)]
    public class KeyBindingSchemeShortcut
    {
        private string _commandField;
        private string _scopeField;
        private string _valueField;

        [XmlAttribute]
        public string Command
        {
            get => _commandField;
            set => _commandField = value;
        }

        /// <remarks/>
        [XmlAttribute]
        public string Scope
        {
            get => _scopeField;
            set => _scopeField = value;
        }

        /// <remarks/>
        [XmlText]
        public string Value
        {
            get => _valueField;
            set => _valueField = value;
        }
    }
}