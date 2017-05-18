using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ModernApplicationFramework.Basics.SettingsManager
{
    [XmlRoot(RootElement)]
    public class UserSettingsFile : XmlDocument, IXmlSerializable
    {

        public const string RootElement = "UserSettings";

        private readonly ISettingsManager _settingsManager;

        public UserSettingsFile(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
        }

        private UserSettingsFile()
        {
            
        }


        public XmlNode GetSingleNode(string path, bool navigateAttributeWise = true)
        {
            var xPath = new XPath(path, navigateAttributeWise);
            var node = SelectSingleNode(SettingsXPathCreator.CreateXPath(xPath));

            return node;
        }

        public IEnumerable<XmlNode> GetChildNodes(string path, bool navigateAttributeWise = true)
        {
            var xPath = new XPath(path, navigateAttributeWise);
            var nodes = SelectSingleNode(SettingsXPathCreator.CreateXPath(xPath));
            if (nodes == null || !nodes.HasChildNodes)
                return new List<XmlNode>();
            return nodes.ChildNodes.Cast<XmlNode>().ToList();
        }


        public string GetPropertyValueData(string path, string propertyName, bool navigateAttributeWise = true)
        {
            var xPath = new XPath(path, navigateAttributeWise);
            var node = SelectSingleNode(SettingsXPathCreator.CreatePropertyValeXPath(xPath, propertyName));
            var value = node?.InnerText;
            return value;
        }

        public string GetPropertyValueData(XmlNode node, string path, string propertyName, bool navigateAttributeWise = true)
        {
            var nodePath = SettingsXPathCreator.CreateNodeXPath(node);
            var xPath = new XPath(path, navigateAttributeWise);
            var additPath = nodePath + SettingsXPathCreator.CreatePropertyValeXPath(xPath, propertyName, XPathCreationOptions.AllowEmpty);
            var result = SelectSingleNode(additPath);
            var value = result?.InnerText;
            return value;
        }

        public string GetPropertyValueData(XmlNode node, string propertyName)
        {
            var nodePath = SettingsXPathCreator.CreateNodeXPath(node) + SettingsXPathCreator.CreatePropertyValeXPath(null, propertyName, XPathCreationOptions.Absolute, false);
            var result = SelectSingleNode(nodePath);
            var value = result?.InnerText;
            return value;
        }


        public string GetAttributeValue(string path, string attribute, bool navigateAttributeWise = true)
        {
            var xPath = new XPath(path, navigateAttributeWise);
            var value = SelectSingleNode(SettingsXPathCreator.CreateElementAttributeValueXPath(xPath, attribute))?.Value;
            return value;
        }






        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            throw new NotSupportedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            if (!string.IsNullOrWhiteSpace(_settingsManager.EnvironmentVarirables.ApplicationVersion))
            {
                writer.WriteStartElement("ApplicationIdentity");
                writer.WriteAttributeString("version", _settingsManager.EnvironmentVarirables.ApplicationVersion);
                writer.WriteEndElement();
            }
        }
    }
}
