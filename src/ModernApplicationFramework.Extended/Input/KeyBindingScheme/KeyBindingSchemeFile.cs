using System.Xml.Schema;
using System.Xml.Serialization;

namespace ModernApplicationFramework.Extended.Input.KeyBindingScheme
{
    [System.Serializable]
    [XmlType(AnonymousType = true)]
    [XmlRoot("KeyBindingScheme")]
    public class KeyBindingSchemeFile
    {
        private string _nameField;
        private KeyBindingSchemeShortcut[] _shortcutsField;

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string Name
        {
            get => _nameField;
            set => _nameField = value;
        }

        [XmlArray(Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("Shortcut", Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public KeyBindingSchemeShortcut[] Shortcuts
        {
            get => _shortcutsField;
            set => _shortcutsField = value;
        }    
    }

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