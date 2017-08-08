using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ModernApplicationFramework.Utilities.Xml
{
    public class XmlObjectParser<T> where T : class 
    {
        private Stream FileStream { get; }

        public XmlObjectParser(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException(nameof(filePath));
            FileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public XmlObjectParser(Stream fileStream)
        {
            FileStream = fileStream;
        }

        public T Parse()
        {
            FileStream.Position = 0;
            var reader = XmlReader.Create(FileStream,
                new XmlReaderSettings {ConformanceLevel = ConformanceLevel.Document});
            return new XmlSerializer(typeof(T)).Deserialize(reader) as T;
        }
    }
}
