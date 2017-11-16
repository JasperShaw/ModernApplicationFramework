using System;
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

        public static T TryGetAttributeValue<T>(this XmlNode node, string attributeName)
        {
            string value;
            try
            {
                value = node.Attributes[attributeName].Value;
            }
            catch
            {
                throw new ArgumentException("Could get attribute: ", attributeName);
            }
           
            var serializer = new XmlValueSerializer();

            if (serializer.Deserialize(value, out var result, default(T)) != GetValueResult.Success)
                throw new ArgumentException($"Could get parse attribute to type {typeof(T).FullName}: ", attributeName);

            return result;
        }
    }
}
