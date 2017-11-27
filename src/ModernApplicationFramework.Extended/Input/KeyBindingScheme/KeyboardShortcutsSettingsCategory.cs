using System;
using System.Xml.Schema;
using System.Xml.Serialization;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Extended.Input.KeyBindingScheme
{

    [Serializable]
    [XmlType(AnonymousType = true)]
    [XmlRoot(ElementName = "Category")]
    public class KeyBindingsSettingsDataModel
    {
        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string Version { get; set; }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public KeyboardShortcutsObject KeyboardShortcuts { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true)]
    public class KeyboardShortcutsObject
    {

        private string _shortcutsSchemeField;

        private KeyboardShortcutsScope[] _scopeDefinitionsField;

        private KeyboardShortcutsUserShortcuts _userShortcutsField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string ShortcutsScheme
        {
            get => _shortcutsSchemeField;
            set => _shortcutsSchemeField = value;
        }

        /// <remarks/>
        [XmlArray(Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("Scope", Form = XmlSchemaForm.Unqualified,
            IsNullable = false)]
        public KeyboardShortcutsScope[] ScopeDefinitions
        {
            get => _scopeDefinitionsField;
            set => _scopeDefinitionsField = value;
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public KeyboardShortcutsUserShortcuts UserShortcuts
        {
            get => _userShortcutsField;
            set => _userShortcutsField = value;
        }
    }

    [Serializable]
    [XmlType(AnonymousType = true)]
    public class KeyboardShortcutsScope
    {

        private string _nameField;

        private string _idField;

        /// <remarks/>
        [XmlAttribute]
        public string Name
        {
            get => _nameField;
            set => _nameField = value;
        }

        /// <remarks/>
        [XmlAttribute]
        public string ID
        {
            get => _idField;
            set => _idField = value;
        }

        public KeyboardShortcutsScope()
        {
            
        }

        public KeyboardShortcutsScope(GestureScope scope)
        {
            Name = scope.Text;
            ID = scope.Id.ToString("B");
        }
    }

    [Serializable]
    [XmlType(AnonymousType = true)]
    public class KeyboardShortcutsUserShortcuts
    {

        private KeyboardShortcutsUserShortcutsData[] _shortcutField;

        private KeyboardShortcutsUserShortcutsData[] _removeShortcutField;

        /// <remarks/>
        [XmlElement("Shortcut", Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public KeyboardShortcutsUserShortcutsData[] Shortcut
        {
            get => _shortcutField;
            set => _shortcutField = value;
        }

        /// <remarks/>
        [XmlElement("RemoveShortcut", Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public KeyboardShortcutsUserShortcutsData[] RemoveShortcut
        {
            get => _removeShortcutField;
            set => _removeShortcutField = value;
        }
    }

    [Serializable]
    [XmlType(AnonymousType = true)]
    public class KeyboardShortcutsUserShortcutsData
    {
        private string _commandField;

        private string _scopeField;

        private string _valueField;

        /// <remarks/>
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
