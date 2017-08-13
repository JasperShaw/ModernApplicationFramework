using System;
using System.Xml.Serialization;
using System.Xml.Schema;

namespace ModernApplicationFramework.Extended.KeyBindingScheme
{

    [Serializable]
    [XmlType(AnonymousType = true)]
    [XmlRoot(ElementName = "Category")]
    public abstract class CategorySettingsDocument
    {
        private string _nameField;
        private string _registeredNameField;
        private string _categoryFiled;

        [XmlAttribute]
        public string Name
        {
            get => _nameField;
            set => _nameField = value;
        }

        [XmlAttribute]
        public string RegisteredName
        {
            get => _registeredNameField;
            set => _registeredNameField = value;
        }

        [XmlAttribute]
        public string Category
        {
            get => _categoryFiled;
            set => _categoryFiled = value;
        }
    }

    [XmlRoot(ElementName = "Category")]
    public class KeyboardShortcutsSettingsCategory : CategorySettingsDocument
    {
        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string Version { get; set; }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public TestKeyboardShortcuts KeyboardShortcuts { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true)]
    public class TestKeyboardShortcuts
    {

        private string _shortcutsSchemeField;

        private TestKeyboardShortcutsScope[] _scopeDefinitionsField;

        private TestKeyboardShortcutsUserShortcuts _userShortcutsField;

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
        public TestKeyboardShortcutsScope[] ScopeDefinitions
        {
            get => _scopeDefinitionsField;
            set => _scopeDefinitionsField = value;
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public TestKeyboardShortcutsUserShortcuts UserShortcuts
        {
            get => _userShortcutsField;
            set => _userShortcutsField = value;
        }
    }

    [Serializable]
    [XmlType(AnonymousType = true)]
    public class TestKeyboardShortcutsScope
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
    }

    [Serializable]
    [XmlType(AnonymousType = true)]
    public class TestKeyboardShortcutsUserShortcuts
    {

        private TestKeyboardShortcutsUserShortcutsShortcut[] _shortcutField;

        private TestKeyboardShortcutsUserShortcutsRemoveShortcut[] _removeShortcutField;

        /// <remarks/>
        [XmlElement("Shortcut", Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public TestKeyboardShortcutsUserShortcutsShortcut[] Shortcut
        {
            get => _shortcutField;
            set => _shortcutField = value;
        }

        /// <remarks/>
        [XmlElement("RemoveShortcut", Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public TestKeyboardShortcutsUserShortcutsRemoveShortcut[] RemoveShortcut
        {
            get => _removeShortcutField;
            set => _removeShortcutField = value;
        }
    }

    [Serializable]
    [XmlType(AnonymousType = true)]
    public class TestKeyboardShortcutsUserShortcutsShortcut
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

    [Serializable]
    [XmlType(AnonymousType = true)]
    public class TestKeyboardShortcutsUserShortcutsRemoveShortcut
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
