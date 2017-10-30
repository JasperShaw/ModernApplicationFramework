using System.Collections.Generic;
using System.Xml;

namespace ModernApplicationFramework.Utilities.Xml
{
    public static class XmlUtilities
    {
        public static XmlElement CreateElement(this XmlDocument document, string name, string innerText, params KeyValuePair<string, string>[] attributes)
        {
            var element = document.CreateElement(name);
            if (!string.IsNullOrEmpty(innerText))
                element.InnerText = innerText;
            foreach (var attribute in attributes)
                element.SetAttribute(attribute.Key, attribute.Value);
            return element;
        }
    }
}
