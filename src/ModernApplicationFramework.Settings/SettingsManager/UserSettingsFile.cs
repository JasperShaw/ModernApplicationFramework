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

        private readonly object _lockObk = new object();

        public UserSettingsFile(ISettingsManager settingsManager) : base(settingsManager)
        {
        }

        public override XmlNode GetSingleNode(string path, bool navigateAttributeWise = true)
        {
            var xPath = new XPath.XPath(path, navigateAttributeWise);
            var node = SettingsStorage.SelectSingleNode(SettingsXPathCreator.CreateXPath(xPath));
            return node;
        }

        public override IEnumerable<XmlNode> GetChildNodes(string path, bool navigateAttributeWise = true)
        {
            var xPath = new XPath.XPath(path, navigateAttributeWise);
            var nodes = SettingsStorage.SelectSingleNode(SettingsXPathCreator.CreateXPath(xPath));
            if (nodes == null || !nodes.HasChildNodes)
                return new List<XmlNode>();
            return nodes.ChildNodes.Cast<XmlNode>().ToList();
        }

        #region PropertyValue

        public override string GetPropertyValueData(string path, string propertyName, bool navigateAttributeWise = true)
        {
            var xPath = new XPath.XPath(path, navigateAttributeWise);
            var node = SettingsStorage.SelectSingleNode(
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
            var result = SettingsStorage.SelectSingleNode(additPath);
            var value = result?.InnerText;
            return value;
        }

        public override string GetPropertyValueData(XmlNode node, string propertyName)
        {
            var nodePath = SettingsXPathCreator.CreateNodeXPath(node) +
                           SettingsXPathCreator.CreatePropertyValeXPath(null, propertyName,
                               XPathCreationOptions.Absolute, false);
            var result = SettingsStorage.SelectSingleNode(nodePath);
            var value = result?.InnerText;
            return value;
        }

        public override void RemoveNodeContent(string settingsFilePath)
        {
            var node = GetSingleNode(settingsFilePath);
            var xmlNodeList = node?.ChildNodes[0]?.ChildNodes;
            if (xmlNodeList == null)
                return;
            if (node is XmlElement e)
                e.IsEmpty = true;
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
            var element = SettingsStorage.CreateElement("PropertyValue", value,
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
            lock (_lockObk)
            {
                if (element == null)
                    throw new SettingsManagerException("The given Property Value could not be found");
                element.InnerText = value;
            }
        }

        #endregion

        public override  void AddCategoryElement(ISettingsCategory category)
        {
            foreach (var settingsCategory in category.Path)
            {
                if (GetSingleNode(settingsCategory.Name) != null)
                    continue;
                XmlNode parentNode;
                if (settingsCategory.Parent == null)
                {
                    if (settingsCategory.CategoryType == SettingsCategoryType.ToolsOption)
                        parentNode = GetSingleNode("ToolsOptions", false) ??
                                     SettingsStorage.DocumentElement?.AppendChild(
                                         CreateToolsOptionsElement(SettingsStorage));
                    else
                        parentNode = GetSingleNode(string.Empty, false);
                }
                else
                {
                    AddCategoryElement(settingsCategory.Parent);
                    parentNode = GetSingleNode(settingsCategory.Parent.Name);
                }

                XmlElement element;
                switch (settingsCategory.CategoryType)
                {
                    case SettingsCategoryType.ToolsOption:
                        element = SettingsStorage.CreateElement("ToolsOptionsCategory", string.Empty,
                            new KeyValuePair<string, string>("name", settingsCategory.Name));
                        break;
                    case SettingsCategoryType.ToolsOptionSub:
                        element = SettingsStorage.CreateElement("ToolsOptionsSubCategory", string.Empty,
                            new KeyValuePair<string, string>("name", settingsCategory.Name));
                        break;
                    case SettingsCategoryType.Normal:
                        element = SettingsStorage.CreateElement("Category", null,
                            new KeyValuePair<string, string>("name", settingsCategory.Name),
                            new KeyValuePair<string, string>("RegisteredName", settingsCategory.Name));
                        if (!settingsCategory.HasChildren)
                            element.SetAttribute("Category", settingsCategory.Id.ToString("B"));
                        break;
                    default:
                        throw new NotSupportedException();
                }
                lock (_lockObk)
                    parentNode?.AppendChild(element);
            }
        }
        
        public override void AddToolsOptionsModelElement(string settingsModelName, string categoryName)
        {
            var node = GetSingleNode(categoryName);

            if (node == null)
                throw new ArgumentNullException(nameof(node));

            lock (_lockObk)
            {
                var element = SettingsStorage.CreateElement("ToolsOptionsCategory", string.Empty,
                    new KeyValuePair<string, string>("name", settingsModelName));
                node.AppendChild(element);
            }
        }

        public override string GetAttributeValue(string path, string attribute, bool navigateAttributeWise = true)
        {
            var xPath = new XPath.XPath(path, navigateAttributeWise);
            var value = SettingsStorage
                .SelectSingleNode(SettingsXPathCreator.CreateElementAttributeValueXPath(xPath, attribute))
                ?.Value;
            return value;
        }


        public override void InsertDocument(string path, XmlDocument document, bool insertRootNode)
        {
            var node = GetSingleNode(path);
            if (node == null)
                return;

            var childs = !insertRootNode ? document.ChildNodes[0]?.ChildNodes : document.ChildNodes;
            if (childs == null)
                return;

            foreach (XmlNode childNode in childs)
            {
                var newNode = node.OwnerDocument?.ImportNode(childNode, true);
                if (newNode == null)
                    continue;
                node.AppendChild(newNode);
            }
        }

        public override XmlDocument CreateNewSettingsStore()
        {
            var document = CreateSettingsStoreBase();
            var rootNode = document.CreateElement(RootElement);
            document.AppendChild(rootNode);
            rootNode.AppendChild(CreateApplicationVersionElement(document));
            rootNode.AppendChild(document.CreateElement("ToolsOptions"));
            SettingsStorage = document;
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
                    SettingsStorage = document;
                }
                SettingsStorage = document;
            }
            catch (XmlException)
            {
                SettingsStorage = CreateSettingsStoreBase();
            }
        }
        
        private XmlElement CreateApplicationVersionElement(XmlDocument document)
        {
            lock (_lockObk)
            {
                return document.CreateElement("ApplicationIdentity", null,
                    new KeyValuePair<string, string>("version", SettingsManager.EnvironmentVariables.ApplicationVersion));
            }
        }

        private XmlElement CreateToolsOptionsElement(XmlDocument document)
        {
            return document.CreateElement("ToolsOptions");
        }
    }
}