using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using ModernApplicationFramework.Settings.Interfaces;
using ModernApplicationFramework.Settings.XPath;
using ModernApplicationFramework.Utilities.Interfaces.Settings;
using ModernApplicationFramework.Utilities.Xml;

namespace ModernApplicationFramework.Settings.SettingsManager
{
    internal class UserSettingsFile : SettingsFile
    {
        public UserSettingsFile(ISettingsManager settingsManager) : base(settingsManager)
        {
        }

        public override XmlNode GetSingleNode(string path, bool navigateAttributeWise = true)
        {
            var xPath = new XPath.XPath(path, navigateAttributeWise);
            var node = SettingsSotrage.SelectSingleNode(SettingsXPathCreator.CreateXPath(xPath));
            return node;
        }

        public override IEnumerable<XmlNode> GetChildNodes(string path, bool navigateAttributeWise = true)
        {
            var xPath = new XPath.XPath(path, navigateAttributeWise);
            var nodes = SettingsSotrage.SelectSingleNode(SettingsXPathCreator.CreateXPath(xPath));
            if (nodes == null || !nodes.HasChildNodes)
                return new List<XmlNode>();
            return nodes.ChildNodes.Cast<XmlNode>().ToList();
        }

        #region PropertyValue

        public override string GetPropertyValueData(string path, string propertyName, bool navigateAttributeWise = true)
        {
            var xPath = new XPath.XPath(path, navigateAttributeWise);
            var node = SettingsSotrage.SelectSingleNode(
                SettingsXPathCreator.CreatePropertyValeXPath(xPath, propertyName));
            var value = node?.InnerText;
            return value;
        }

        public override string GetPropertyValueData(XmlNode node, string path, string propertyName,
            bool navigateAttributeWise = true)
        {
            var nodePath = SettingsXPathCreator.CreateNodeXPath(node);
            var xPath = new XPath.XPath(path, navigateAttributeWise);
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

        public override void AddToolsOptionsCategoryElement(string name)
        {
            var toolsOptionsNode = GetSingleNode("ToolsOptions", false);
            if (toolsOptionsNode == null)
            {
                var root = SettingsSotrage.DocumentElement;
                toolsOptionsNode = root?.AppendChild(CreateToolsOptionsElement(SettingsSotrage)); 
            }
            lock (SettingsSotrage)
            {
                var element = SettingsSotrage.CreateElement("ToolsOptionsCategory", string.Empty,
                    new KeyValuePair<string, string>("name", name));
                //If it is still null we want to throw an exception
                if (toolsOptionsNode == null)
                    throw new NullReferenceException(nameof(toolsOptionsNode));
                toolsOptionsNode.AppendChild(element);
            }
        }

        public override void AddCategoryElement(IEnumerable<ISettingsCategory> path, string name, bool navigateAttributeWise = true)
        {
            foreach (var category in path)
            {
                if (GetSingleNode(category.Name) != null)
                    continue;
                var parentNode = category.Parent == null ? GetSingleNode(string.Empty, false) : GetSingleNode(category.Parent.Name);
                parentNode.AppendChild(SettingsSotrage.CreateElement("Category", null,
                    new KeyValuePair<string, string>("name", category.Name),
                    new KeyValuePair<string, string>("RegisteredName", category.Name)));
            }        
        }

        public override void AddToolsOptionsModelElement(string settingsModelName, string categoryName)
        {
            var node = GetSingleNode(categoryName);

            if (node == null)
                throw new ArgumentNullException(nameof(node));

            lock (SettingsSotrage)
            {
                var element = SettingsSotrage.CreateElement("ToolsOptionsCategory", string.Empty,
                    new KeyValuePair<string, string>("name", settingsModelName));
                node.AppendChild(element);
            }
        }

        public override string GetAttributeValue(string path, string attribute, bool navigateAttributeWise = true)
        {
            var xPath = new XPath.XPath(path, navigateAttributeWise);
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
            rootNode.AppendChild(document.CreateElement("ToolsOptions"));
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
            lock (SettingsSotrage)
            {
                return document.CreateElement("ApplicationIdentity", null,
                    new KeyValuePair<string, string>("version", SettingsManager.EnvironmentVarirables.ApplicationVersion));
            }
        }

        private XmlElement CreateToolsOptionsElement(XmlDocument document)
        {
            return document.CreateElement("ToolsOptions");
        }
    }
}