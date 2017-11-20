using System.IO;
using System.Xml;

namespace ModernApplicationFramework.Interfaces.Services
{
    public interface ICommandBarLayoutBackupProvider
    {
        /// <summary>
        /// Gets the backup document.
        /// </summary>
        XmlDocument Backup { get; }

        /// <summary>
        /// Creates a layout backup from document stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        void CreateBackupFromStream(Stream stream);
    }
}