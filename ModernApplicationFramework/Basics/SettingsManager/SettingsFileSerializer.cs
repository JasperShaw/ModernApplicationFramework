using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ModernApplicationFramework.Basics.SettingsManager
{
    public class SettingsFileSerializer
    {
        private ISettingsManager SettingsManager { get; }

        protected XmlSerializerNamespaces Namespaces { get; set; }

        public SettingsFileSerializer(ISettingsManager settingsManager)
        {
            SettingsManager = settingsManager;
        }

        protected virtual void PrepareNamespaces()
        {
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            Namespaces = ns;
        }


        public UserSettingsFile Desrialize(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException();
            try
            {
                var xmlDocument = new UserSettingsFile(SettingsManager);
                xmlDocument.Load(filePath);
                return xmlDocument;
            }
            catch (Exception e)
            {
                throw new SettingsManagerException("Could not load current settings. File is currupt.", e);
            }
        }

        public void Serialize(XmlWriter writer)
        {
            PrepareNamespaces();
            var serializer = new XmlSerializer(typeof(UserSettingsFile));
            serializer.Serialize(writer, new UserSettingsFile(SettingsManager), Namespaces);
        }

        public void Serialize(TextWriter writer)
        {
            PrepareNamespaces();
            var serializer = new XmlSerializer(typeof(UserSettingsFile));
            serializer.Serialize(writer, new UserSettingsFile(SettingsManager), Namespaces);
        }

        public void Serialize(Stream stream)
        {
            PrepareNamespaces();
            var serializer = new XmlSerializer(typeof(UserSettingsFile));
            serializer.Serialize(stream, new UserSettingsFile(SettingsManager), Namespaces);
        }

        public void Serialize(string filepath)
        {
            using (var stream = new StreamWriter(filepath))
                Serialize(stream);
        }

    }
}
