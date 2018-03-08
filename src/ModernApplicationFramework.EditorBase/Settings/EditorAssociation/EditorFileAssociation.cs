using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ModernApplicationFramework.EditorBase.Settings.EditorAssociation
{
    [Serializable]
    [XmlType(AnonymousType = true)]
    [XmlRoot(ElementName = "Editor")]
    public class EditorFileAssociation
    {
        private HashSet<EditorSupportedFileDefinition> _createWithDefaultExtension;
        private HashSet<EditorSupportedFileDefinition> _defaultExtension;

        [XmlAttribute]
        public string Id { get; set; }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlElement("CreateWithDefaultExtension", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
        public HashSet<EditorSupportedFileDefinition> CreateWithDefaultExtension
        {
            get => _createWithDefaultExtension;
            set => _createWithDefaultExtension = value;
        }

        [XmlElement("DefaultExtension", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
        public HashSet<EditorSupportedFileDefinition> DefaultExtension
        {
            get => _defaultExtension;
            set => _defaultExtension = value;
        }

        public EditorFileAssociation(string id, string name)
        {
            Id = id;
            Name = name;
            CreateWithDefaultExtension = new HashSet<EditorSupportedFileDefinition>();
            DefaultExtension = new HashSet<EditorSupportedFileDefinition>();
        }

        internal void AddRange(IEnumerable<string> extensions, AddOption option)
        {
            if (option == AddOption.NewFile)
            {
                foreach (var extension in extensions)
                    CreateWithDefaultExtension.Add(new EditorSupportedFileDefinition(extension));
            }
            else if (option == AddOption.OpenFile)
            {
                foreach (var extension in extensions)
                    DefaultExtension.Add(new EditorSupportedFileDefinition(extension));
            }

        }

        public EditorFileAssociation()
        {
            
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as EditorFileAssociation);
        }

        public bool Equals(EditorFileAssociation other)
        {
            if (other == null)
                return false;
            if (!Id.Equals(other.Id))
                return false;
            if (DefaultExtension.Count != other.DefaultExtension.Count &&
                CreateWithDefaultExtension.Count != other.CreateWithDefaultExtension.Count)
                return false;
            if (DefaultExtension.Count == 0 && other.DefaultExtension.Count == 0 && 
                CreateWithDefaultExtension.Count == 0 &&  other.CreateWithDefaultExtension.Count == 0)
                return true;
            if (DefaultExtension.Count != 0 && DefaultExtension.SetEquals(other.DefaultExtension))
                return true;
            if (CreateWithDefaultExtension.Count != 0 && CreateWithDefaultExtension.SetEquals(other.CreateWithDefaultExtension))
                return true;
            return false;
        }


    }

    [Serializable]
    [XmlType(AnonymousType = true)]
    public class EditorSupportedFileDefinition
    {
        private string _extension;

        [XmlText]
        public string Extension
        {
            get => _extension;
            set => _extension = value;
        }

        public EditorSupportedFileDefinition(string extension)
        {
            Extension = extension;
        }

        public EditorSupportedFileDefinition()
        {

        }

        public override bool Equals(object obj)
        {
            return Equals(obj as EditorSupportedFileDefinition);
        }

        public override int GetHashCode()
        {
            return Extension.GetHashCode();
        }

        public bool Equals(EditorSupportedFileDefinition other)
        {
            if (other == null)
                return false;
            if (Extension.Equals(other.Extension, StringComparison.CurrentCultureIgnoreCase))
                return true;
            return false;
        }
    }

    internal enum AddOption
    {
        NewFile,
        OpenFile
    }
}