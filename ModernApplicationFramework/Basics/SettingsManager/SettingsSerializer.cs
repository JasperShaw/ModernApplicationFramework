using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ModernApplicationFramework.Basics.SettingsManager
{
    public class SettingsSerializer
    {
        private ISettingsManager SettingsManager { get; }

        public SettingsSerializer(ISettingsManager settingsManager)
        {
            SettingsManager = settingsManager;
        }

        public void Serialize(XmlWriter writer)
        {
            var serializer = new XmlSerializer(typeof(ISettingsManager));
            serializer.Serialize(writer, SettingsManager);
        }

        public void Serialize(TextWriter writer)
        {
            var serializer = new XmlSerializer(typeof(ISettingsManager));
            serializer.Serialize(writer, SettingsManager);
        }

        public void Serialize(Stream stream)
        {
            var serializer = new XmlSerializer(typeof(ISettingsManager));
            serializer.Serialize(stream, SettingsManager);
        }

        public void Serialize(string filepath)
        {
            using (var stream = new StreamWriter(filepath))
                Serialize(stream);
        }

    }
}
