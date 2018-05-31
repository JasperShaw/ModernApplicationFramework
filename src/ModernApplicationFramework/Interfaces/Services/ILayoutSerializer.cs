using System.IO;
using System.Xml;

namespace ModernApplicationFramework.Interfaces.Services
{
    public interface ILayoutSerializer<in T>
    {
        /// <summary>
        /// Deserializes and applies a layout from a document stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        void Deserialize(Stream stream);

        /// <summary>
        /// Deserializes and applies a layout from a <see cref="XmlDocument"/>.
        /// </summary>
        /// <param name="document">The document.</param>
        void Deserialize(XmlDocument document);

        /// <summary>
        /// Deserializes and applies a layout from a <see cref="XmlNode"/>.
        /// </summary>
        /// <param name="xmlRootNode">The XML root node.</param>
        void Deserialize(XmlNode xmlRootNode);

        /// <summary>
        /// Restores a given <see cref="T"/> from backup.
        /// </summary>
        /// <param name="backup">The backup.</param>
        /// <param name="item">The item.</param>
        void ResetFromBackup(XmlDocument backup, T item);

        /// <summary>
        ///     Serializes the current layout to a stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        void Serialize(Stream stream);

        /// <summary>
        /// Validates whether the given input stream is valid to deserialize.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns><see langword="true"/> when validation was successful with no errors; otherwise <see langword="false"/></returns>
        bool Validate(Stream stream);

        /// <summary>
        /// Validates whether the given <see cref="XmlNode"/> is valid to deserialize.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns><see langword="true"/> when validation was successful with no errors; otherwise <see langword="false"/></returns>
        bool Validate(XmlNode node);
    }
}