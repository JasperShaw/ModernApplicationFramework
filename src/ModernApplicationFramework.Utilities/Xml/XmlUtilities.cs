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

        public static GetValueResult TryGetValueResult<T>(this XmlNode node, string attributeName, out T result, T defaultValue = default(T))
        {
            result = defaultValue;

            string value;
            try
            {
                value = node.Attributes[attributeName].Value;
            }
            catch
            {
                return GetValueResult.Missing;
            }
            var serializer = new XmlValueSerializer();
            return serializer.Deserialize(value, out result);
        }

        public static string GetAttributeValue(this XmlNode node, string attributeName)
        {
            return node.GetAttributeValue<string>(attributeName);
        }

        public static T GetAttributeValue<T>(this XmlNode node, string attributeName)
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
