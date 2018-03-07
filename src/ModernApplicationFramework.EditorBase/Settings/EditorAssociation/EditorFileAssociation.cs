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
        private List<EditorSupportedFileDefinition> _createWithDefaultExtension;
        private List<EditorSupportedFileDefinition> _defaultExtension;

        [XmlAttribute]
        public string Id { get; set; }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlElement("CreateWithDefaultExtension", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
        public List<EditorSupportedFileDefinition> CreateWithDefaultExtension
        {
            get => _createWithDefaultExtension;
            set => _createWithDefaultExtension = value;
        }

        [XmlElement("DefaultExtension", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
        public List<EditorSupportedFileDefinition> DefaultExtension
        {
            get => _defaultExtension;
            set => _defaultExtension = value;
        }

        public EditorFileAssociation(string id, string name)
        {
            Id = id;
            Name = name;
            CreateWithDefaultExtension = new List<EditorSupportedFileDefinition>();
            DefaultExtension = new List<EditorSupportedFileDefinition>();
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
    }

    internal enum AddOption
    {
        NewFile,
        OpenFile
    }
}