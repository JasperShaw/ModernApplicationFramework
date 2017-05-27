using System;
using System.Collections.Generic;
using System.Xml;

namespace ModernApplicationFramework.Settings.SettingsManager
{

    public interface IReadableSettingsFile
    {
        string GetPropertyValueData(string path, string propertyName, bool navigateAttributeWise = true);
        string GetPropertyValueData(XmlNode node, string propertyName);
        string GetPropertyValueData(XmlNode node, string path, string propertyName, bool navigateAttributeWise = true);
        string GetAttributeValue(string path, string attribute, bool navigateAttributeWise = true);
        void TryRead(string path);
    }

    public interface IWriteableSettingsFile
    {
        void AddOrChangePropertyValueElement(XmlNode parent, string propertyName, string value, bool changeValueIfExists = true);
        void AddPropertyValueElement(XmlNode parent, string propertyName, string value);
        void SetPropertyValueData(string path, string propertyName, string value, bool navigateAttributeWise = true);

        void AddPropertyValueElement(string path, string propertyName, string value, bool navigateAttributeWise);

        void AddOrChangePropertyValueElement(string path, string propertyName, string value, bool navigateAttributeWise, bool changeValueIfExists);

        void AddToolsOptionsCategoryElement(string name);

        void AddToolsOptionsModelElement(string settingsModelName, string categoryName);

        void Save();
    }


    public interface ISettingsFile : IReadableSettingsFile, IWriteableSettingsFile, IDisposable
    {
        XmlDocument SettingsSotrage { get; set; }     
        XmlDocument CreateNewSettingsStore();    
        IEnumerable<XmlNode> GetChildNodes(string path, bool navigateAttributeWise = true);
        XmlNode GetSingleNode(string path, bool navigateAttributeWise = true);
    }
}