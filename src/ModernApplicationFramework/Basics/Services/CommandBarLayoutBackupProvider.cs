using System.ComponentModel.Composition;
using System.IO;
using System.Xml;

namespace ModernApplicationFramework.Basics.Services
{
    [Export(typeof(ICommandBarLayoutBackupProvider))]
    public class CommandBarLayoutBackupProvider : ICommandBarLayoutBackupProvider
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

    public interface ICommandBarLayoutBackupProvider
    {
        XmlDocument Backup { get; }

        void CreateBackupFromStream(Stream stream);
    }
}
