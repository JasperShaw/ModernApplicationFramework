using System.IO;
using System.Xml;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Basics.Services
{
    public class LayoutBackupProvider : ILayoutBackupProvider
    {
        public XmlDocument Backup { get; private set; }

        public void CreateBackupFromStream(Stream stream)
        {
            stream.Seek(0L, SeekOrigin.Begin);
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(stream);
            Backup = xmlDoc;
        }
    }
}