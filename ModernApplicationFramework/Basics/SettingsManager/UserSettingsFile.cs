using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ModernApplicationFramework.Basics.SettingsManager
{
    [XmlRoot(RootElement)]
    public class UserSettingsFile : IXmlSerializable
    {

        public const string RootElement = "UserSettings";

        private readonly ISettingsManager _settingsManager;

        public UserSettingsFile(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
        }

        private UserSettingsFile()
        {
            
        }


        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            throw new NotSupportedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            if (!string.IsNullOrWhiteSpace(_settingsManager.EnvironmentVarirables.ApplicationVersion))
            {
                writer.WriteStartElement("ApplicationIdentity");
                writer.WriteAttributeString("version", _settingsManager.EnvironmentVarirables.ApplicationVersion);
                writer.WriteEndElement();
            }
        }
    }
}
