using System.IO;
using System.Xml;
using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Interfaces.Services
{
    public interface ICommandBarSerializer
    {
        void Serialize(Stream stream);

        void Deserialize(Stream stream);

        void Deserialize(XmlDocument document);

        void Deserialize(XmlNode xmlRootNode);

        bool Validate(Stream stream);

        bool Validate(XmlNode node);

        void ResetFromBackup(XmlDocument backup, CommandBarDefinitionBase item);
    }
}