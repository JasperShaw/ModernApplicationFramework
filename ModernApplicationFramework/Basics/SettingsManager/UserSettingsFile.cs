using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using ModernApplicationFramework.Native.Platform;

namespace ModernApplicationFramework.Basics.SettingsManager
{
    public abstract class SettingsFile : DisposableObject, ISettingsFile
    {
        protected const string RootElement = "UserSettings";


        protected readonly ISettingsManager SettingsManager;

        public XmlDocument SettingsSotrage { get; set; }

        protected SettingsFile(ISettingsManager settingsManager)
        {
            SettingsManager = settingsManager;
        }

        public abstract XmlNode GetSingleNode(string path, bool navigateAttributeWise = true);

        public abstract IEnumerable<XmlNode> GetChildNodes(string path, bool navigateAttributeWise = true);

        public abstract string GetPropertyValueData(string path, string propertyName,
            bool navigateAttributeWise = true);

        public abstract void AddPropertyValueElement(string path, string propertyName, string value,
            bool navigateAttributeWise = true);

        public abstract void AddOrChangePropertyValueElement(string path, string propertyName, string value,
            bool navigateAttributeWise = true,
            bool changeValueIfExists = true);

        public abstract string GetPropertyValueData(XmlNode node, string path, string propertyName,
            bool navigateAttributeWise = true);

        public abstract string GetPropertyValueData(XmlNode node, string propertyName);

        public abstract string GetAttributeValue(string path, string attribute, bool navigateAttributeWise = true);

        public abstract void SetPropertyValueData(string path, string propertyName, string value,
            bool navigateAttributeWise = true);


        /// <summary>
        ///     Adds a new PropertyValue Element to the given node.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public abstract void AddPropertyValueElement(XmlNode parent, string propertyName, string value);

        /// <summary>
        ///     Adds a new PropertyValue Element to the given node. Overrides the value if property element already exists
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <param name="changeValueIfExists"></param>
        public abstract void AddOrChangePropertyValueElement(XmlNode parent, string propertyName, string value,
            bool changeValueIfExists = true);

        /// <summary>
        ///     Creates and assinges a new SettingsStore
        /// </summary>
        /// <returns>returs the generated SettingsStore</returns>
        public abstract XmlDocument CreateNewSettingsStore();


        /// <summary>
        ///     Reads a settings file on disk and sets the SettingsStorage
        /// </summary>
        /// <param name="path">Path to the file</param>
        public abstract void TryRead(string path);

        public void Save()
        {
            lock (SettingsSotrage)
            {
                var settings =
                    new XmlWriterSettings
                    {
                        Indent = true,
                        IndentChars = @"    ",
                        NewLineChars = Environment.NewLine,
                        NewLineHandling = NewLineHandling.Replace
                    };
                using (var w = XmlWriter.Create(SettingsManager.EnvironmentVarirables.SettingsFilePath, settings))
                {
                    SettingsSotrage.Save(w);
                }
            }
        }

        protected override void DisposeManagedResources()
        {
            try
            {
                Save();
                SettingsSotrage = null;
            }
            catch (Exception)
            {
                //Ignored
            }
        }


        /// <summary>
        ///     Provides a new XML-Document with declaration
        /// </summary>
        /// <returns></returns>
        protected XmlDocument CreateSettingsStoreBase()
        {
            var document = new XmlDocument();
            var xmlDeclaration = document.CreateXmlDeclaration("1.0", "UTF-8", null);
            var root = document.DocumentElement;
            document.InsertBefore(xmlDeclaration, root);
            return document;
        }
    }


    public class UserSettingsFile : SettingsFile
    {
        public UserSettingsFile(ISettingsManager settingsManager) : base(settingsManager)
        {
        }

        public override XmlNode GetSingleNode(string path, bool navigateAttributeWise = true)
        {
            var xPath = new XPath(path, navigateAttributeWise);
            var node = SettingsSotrage.SelectSingleNode(SettingsXPathCreator.CreateXPath(xPath));
            return node;
        }

        public override IEnumerable<XmlNode> GetChildNodes(string path, bool navigateAttributeWise = true)
        {
            var xPath = new XPath(path, navigateAttributeWise);
            var nodes = SettingsSotrage.SelectSingleNode(SettingsXPathCreator.CreateXPath(xPath));
            if (nodes == null || !nodes.HasChildNodes)
                return new List<XmlNode>();
            return nodes.ChildNodes.Cast<XmlNode>().ToList();
        }

        #region PropertyValue

        public override string GetPropertyValueData(string path, string propertyName, bool navigateAttributeWise = true)
        {
            var xPath = new XPath(path, navigateAttributeWise);
            var node = SettingsSotrage.SelectSingleNode(
                SettingsXPathCreator.CreatePropertyValeXPath(xPath, propertyName));
            var value = node?.InnerText;
            return value;
        }

        public override string GetPropertyValueData(XmlNode node, string path, string propertyName,
            bool navigateAttributeWise = true)
        {
            var nodePath = SettingsXPathCreator.CreateNodeXPath(node);
            var xPath = new XPath(path, navigateAttributeWise);
            var additPath =
                nodePath + SettingsXPathCreator.CreatePropertyValeXPath(xPath, propertyName,
                    XPathCreationOptions.AllowEmpty);
            var result = SettingsSotrage.SelectSingleNode(additPath);
            var value = result?.InnerText;
            return value;
        }

        public override string GetPropertyValueData(XmlNode node, string propertyName)
        {
            var nodePath = SettingsXPathCreator.CreateNodeXPath(node) +
                           SettingsXPathCreator.CreatePropertyValeXPath(null, propertyName,
                               XPathCreationOptions.Absolute, false);
            var result = SettingsSotrage.SelectSingleNode(nodePath);
            var value = result?.InnerText;
            return value;
        }

        public override void AddPropertyValueElement(XmlNode parent, string propertyName, string value)
        {
            AddOrChangePropertyValueElement(parent, propertyName, value, false);
        }

        public override void AddPropertyValueElement(string path, string propertyName, string value, bool navigateAttributeWise = true)
        {
            AddOrChangePropertyValueElement(path, propertyName, value, false);
        }

        public override void AddOrChangePropertyValueElement(string path, string propertyName, string value, bool navigateAttributeWise = true,
            bool changeValueIfExists = true)
        {
            var node = GetSingleNode(path, navigateAttributeWise);
            if (node == null || !node.HasChildNodes)
                throw new SettingsManagerException("The given Property Value could not be found");
            AddOrChangePropertyValueElement(node, propertyName, value, changeValueIfExists);
        }

        public override void AddOrChangePropertyValueElement(XmlNode parent, string propertyName, string value,
            bool changeValueIfExists = true)
        {
            if (parent == null)
                throw new SettingsManagerException("Could not add PropertyValue to settings storage");

            foreach (XmlNode node in parent.ChildNodes)
            {
                if (node.Attributes?["name"] == null || !node.Attributes["name"].Value
                        .Equals(propertyName, StringComparison.CurrentCultureIgnoreCase))
                    continue;
                if (changeValueIfExists)
                    node.InnerText = value;
                return;
            }
            var element = SettingsSotrage.CreateElement("PropertyValue", value,
                new KeyValuePair<string, string>("name", propertyName));
            parent.AppendChild(element);
        }

        public override void SetPropertyValueData(string path, string propertyName, string value,
             bool navigateAttributeWise = true)
        {
                var node = GetSingleNode(path, navigateAttributeWise);
                if (node == null || !node.HasChildNodes)
                    throw new SettingsManagerException("The given Property Value could not be found");

                XmlNode element = null;

                foreach (XmlNode child in node.ChildNodes)
                {
                    if (child.Attributes?["name"] == null || !child.Attributes["name"].Value
                            .Equals(propertyName, StringComparison.CurrentCultureIgnoreCase))
                        continue;
                    element = child;
                }
            lock (SettingsSotrage)
            {
                if (element == null)
                    throw new SettingsManagerException("The given Property Value could not be found");
                element.InnerText = value;
            }
        }

        #endregion

        public override string GetAttributeValue(string path, string attribute, bool navigateAttributeWise = true)
        {
            var xPath = new XPath(path, navigateAttributeWise);
            var value = SettingsSotrage
                .SelectSingleNode(SettingsXPathCreator.CreateElementAttributeValueXPath(xPath, attribute))
                ?.Value;
            return value;
        }





        public override XmlDocument CreateNewSettingsStore()
        {
            var document = CreateSettingsStoreBase();
            var rootNode = document.CreateElement(RootElement);
            document.AppendChild(rootNode);

            rootNode.AppendChild(CreateApplicationVersionElement(document));

            SettingsSotrage = document;
            return document;
        }

        public override void TryRead(string path)
        {
            try
            {
                var document = new XmlDocument();
                using (var xmlTextReader = new XmlTextReader(path))
                {
                    xmlTextReader.Normalization = false;
                    document.Load(xmlTextReader);
                    SettingsSotrage = document;
                }
                SettingsSotrage = document;
            }
            catch (XmlException)
            {
                SettingsSotrage = CreateSettingsStoreBase();
            }
        }

        private XmlElement CreateApplicationVersionElement(XmlDocument document)
        {
            return document.CreateElement("ApplicationIdentity", null,
                new KeyValuePair<string, string>("version", SettingsManager.EnvironmentVarirables.ApplicationVersion));
        }
    }
}