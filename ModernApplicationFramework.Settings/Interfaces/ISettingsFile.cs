using System;
using System.Collections.Generic;
using System.Xml;

namespace ModernApplicationFramework.Settings.Interfaces
{
    public interface ISettingsFile : IReadableSettingsFile, IWriteableSettingsFile, IDisposable
    {
        XmlDocument SettingsStorage { get; set; }     
        XmlDocument CreateNewSettingsStore();    
        IEnumerable<XmlNode> GetChildNodes(string path, bool navigateAttributeWise = true);
        XmlNode GetSingleNode(string path, bool navigateAttributeWise = true);
    }
}