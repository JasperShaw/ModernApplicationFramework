using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ModernApplicationFramework.EditorBase.Settings
{
    [Serializable]
    [XmlType(AnonymousType = true)]
    [XmlRoot(ElementName = "Editor")]
    public class EditorFileAssociation
    {
        private string _idField;
        private List<EditorSupportedFileDefinition> _supportedFileDefinitionField;

        [XmlAttribute]
        public string Id
        {
            get => _idField;
            set => _idField = value;
        }

        [XmlElement("DefaultFileExtension", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
        public List<EditorSupportedFileDefinition> SupportedFileDefinition
        {
            get => _supportedFileDefinitionField;
            set => _supportedFileDefinitionField = value;
        }

        public EditorFileAssociation(string id, IEnumerable<string> extensions)
        {
            Id = id;
            SupportedFileDefinition = new List<EditorSupportedFileDefinition>();
            foreach (var extension in extensions)
                SupportedFileDefinition.Add(new EditorSupportedFileDefinition(extension));
        }

        public EditorFileAssociation(string id)
        {
            Id = id;
            SupportedFileDefinition = new List<EditorSupportedFileDefinition>();
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
}