using System.Collections.Generic;
using System.Xml;

namespace ModernApplicationFramework.Settings.Interfaces
{
    public interface IWriteableSettingsFile
    {
        void AddOrChangePropertyValueElement(XmlNode parent, string propertyName, string value, bool changeValueIfExists = true);
        void AddPropertyValueElement(XmlNode parent, string propertyName, string value);
        void SetPropertyValueData(string path, string propertyName, string value, bool navigateAttributeWise = true);

        void AddPropertyValueElement(string path, string propertyName, string value, bool navigateAttributeWise);

        void AddOrChangePropertyValueElement(string path, string propertyName, string value, bool navigateAttributeWise, bool changeValueIfExists);

        void AddToolsOptionsCategoryElement(string name);

        void AddToolsOptionsModelElement(string settingsModelName, string categoryName);

        void AddCategoryElement(IEnumerable<ISettingsCategory> path, string name, bool navigateAttributeWise = true);

        void Save();
    }
}