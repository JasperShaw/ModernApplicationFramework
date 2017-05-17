using System;
using System.IO;
using System.Xml;

namespace ModernApplicationFramework.Basics.SettingsManager
{
    public class SettingsFileDeserializer
    {
        public XmlDocument Desrialize(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException();
            try
            {
                var xmlDocument = new XmlDocument();
                xmlDocument.Load(filePath);
                return xmlDocument;
            }
            catch (Exception e)
            {
                throw new SettingsManagerException("Could not load current settings. File is currupt.", e);
            }
        }

        public GetValueResult Deserialize<T>(string s, out T result)
        {
            result = default(T);

            int num = s.IndexOf('*', 1);

            return GetValueResult.Success;
        }
    }
}
