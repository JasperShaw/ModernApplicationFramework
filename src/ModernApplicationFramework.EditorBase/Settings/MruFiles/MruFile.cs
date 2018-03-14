using System;
using System.Xml.Serialization;

namespace ModernApplicationFramework.EditorBase.Settings.MruFiles
{
    [Serializable]
    [XmlType(AnonymousType = true)]
    [XmlRoot(ElementName = "MruFile")]
    public class MruFile
    {
        [XmlAttribute]
        public string PersistenceData { get; set; }

        public MruFile(string data)
        {
            PersistenceData = data;
        }

        public MruFile()
        {
            
        }
    }
}