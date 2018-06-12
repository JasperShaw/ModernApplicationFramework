using System;
using System.IO;
using System.Xml;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Utilities.Xml;

namespace ModernApplicationFramework.Basics.Services
{
    public abstract class LayoutSerializer<T> : ILayoutSerializer<T>
    {
        protected abstract string RootNode { get; }

        protected abstract Stream ValidationScheme { get; }

        public virtual void Deserialize(Stream stream)
        {
            var document = new XmlDocument();
            document.Load(stream);
            Deserialize(document);
        }

        public virtual void Deserialize(XmlDocument document)
        {
            Deserialize(document.DocumentElement);
        }

        public virtual void Deserialize(XmlNode xmlRootNode)
        {
            EnsureInitialized();
            PrepareDeserialize();
            ClearCurrentLayout();
            Deserialize(ref xmlRootNode);
        }

        protected abstract void Deserialize(ref XmlNode xmlRootNode);

        public virtual void ResetFromBackup(XmlDocument backup, T item)
        {
            var currentLayout = new XmlDocument();
            using (var stream = new MemoryStream())
            {
                Serialize(stream);
                stream.Seek(0L, SeekOrigin.Begin);
                currentLayout.Load(stream);
            }

            var backupNode = GetBackupNode(backup, item); 
            var currentNode = GetCurrentNode(currentLayout, item);

            if (currentNode == null)
                throw new ArgumentNullException(nameof(currentNode));

            if (backupNode == null)
            {
                HandleBackupNodeNull(item);
                return;
            }
            var replaceNode = currentLayout.ImportNode(backupNode, true);
            currentNode.ParentNode?.ReplaceChild(replaceNode, currentNode);
            Deserialize(currentLayout);
        }

        public virtual void ResetFromBackup(XmlDocument backup)
        {
            Deserialize(backup);
        }

        protected abstract void HandleBackupNodeNull(T item);
        protected abstract XmlNode GetBackupNode(in XmlDocument backup, T item);
        protected abstract XmlNode GetCurrentNode(in XmlDocument currentLayout, T item);

        public void Serialize(Stream stream)
        {
            EnsureInitialized();
            var xmlDocument = CreateDocument();
            Serialize(ref xmlDocument);
            xmlDocument.Save(stream);
        }

        public virtual bool Validate(Stream stream)
        {
            var validator = new XmlValidator(ValidationScheme);
            if (!validator.Validate(stream))
                return false;
            return true;
        }

        public virtual bool Validate(XmlNode node)
        {
            var validator = new XmlValidator(ValidationScheme);
            if (!validator.Validate(node, ConformanceLevel.Fragment))
                return false;
            return true;
        }

        protected abstract void EnsureInitialized();

        protected abstract void PrepareDeserialize();

        protected abstract void ClearCurrentLayout();

        protected abstract void Serialize(ref XmlDocument xmlDocument);

        protected virtual XmlDocument CreateDocument()
        {
            var xmlDocument = new XmlDocument();
            var xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
            var root = xmlDocument.DocumentElement;
            xmlDocument.InsertBefore(xmlDeclaration, root);
            var rootElement = xmlDocument.CreateElement(string.Empty, RootNode, string.Empty);
            xmlDocument.AppendChild(rootElement);
            return xmlDocument;
        }
    }
}