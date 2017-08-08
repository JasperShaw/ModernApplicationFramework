using System.Xml.Schema;
using System.Xml.Serialization;

namespace ModernApplicationFramework.Extended.KeyBindingScheme
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
}