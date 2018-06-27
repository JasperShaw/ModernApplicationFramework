using System;
using System.Xml.Serialization;

namespace ModernApplicationFramework.EditorBase.Settings.MruFiles
{
    [Serializable]
    [XmlType(AnonymousType = true)]
    [XmlRoot(ElementName = "MruFile")]
    public class StorableMruFile
    {
        [XmlAttribute]
        public string PersistenceData { get; set; }

        public StorableMruFile(string data)
        {
            PersistenceData = data;
        }

        public StorableMruFile()
        {
            
        }
    }
}