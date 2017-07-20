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
}