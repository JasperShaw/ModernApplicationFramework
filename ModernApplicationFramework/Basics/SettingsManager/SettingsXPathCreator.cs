using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace ModernApplicationFramework.Basics.SettingsManager
{
    public static class SettingsXPathCreator
    {
        private const string PropertyValueElementName = "PropertyValue";
        private const string PropertyAttributeName = "name";
        private const string PathAttributeName = "name";


        /// <summary>
        /// Creates a specific XPath query that will find the InnerText of a PropertyValue XML-Element. The Element must look like this:
        /// <![CDATA[<PropertyValue name="propertyName">value</PropertyValue>]]>
        /// </summary>
        /// <param name="xPath">The Container that holds the path to the element containing the PropertyValue-Element</param>
        /// <param name="propertyName">The name of the property to query</param>
        /// <param name="options">Decides whether the path is Absolute or relative</param>
        /// <param name="createCompletePath">Decides whether there will be a full path creation. Default is true</param>
        /// <returns>returns the query</returns>
        public static string CreatePropertyValeXPath(XPath xPath, string propertyName, XPathCreationOptions options = XPathCreationOptions.FirstRelative, bool createCompletePath = true)
        {
            var path = string.Empty;
            if (createCompletePath)
            {
                path = CreateXPath(xPath, options);
                path += $"//{PropertyValueElementName}";
            }

            if (string.IsNullOrEmpty(propertyName))
                path += $"[@{PropertyAttributeName}]";
            else
                path += $"[@{PropertyAttributeName}='{propertyName}']";
            return path;
        }

        /// <summary>
        /// Creates an XPath query that will find the Attribute of an XML-Element.
        /// </summary>
        /// <param name="xPath">The Container that holds the path to the Element</param>
        /// <param name="attributeName">The name of attribute</param>
        /// <param name="options">Decides whether the path is Absolute or relative</param>
        /// <returns>returns the query</returns>
        public static string CreateElementAttributeValueXPath(XPath xPath, string attributeName, XPathCreationOptions options = XPathCreationOptions.FirstRelative)
        {
            var path = CreateXPath(xPath);
            if (string.IsNullOrEmpty(attributeName))
                path += "/@*";
            else
                path += $"/@{attributeName}";
            return path;
        }

        /// <summary>
        /// Creates an XPath query that will navigate to the given Element
        /// </summary>
        /// <param name="xPath">The Container that holds the path to the Element</param>
        /// <param name="options">Decides whether the path is Absolute or relative</param>
        /// <returns>the query to the Element</returns>
        public static string CreateXPath(XPath xPath, XPathCreationOptions options = XPathCreationOptions.FirstRelative)
        {
            return xPath.NavigateAttributeWise
                ? CreateBaseXPath(xPath, options, s => $"*[@{PathAttributeName}='{s}']")
                : CreateBaseXPath(xPath, options, s => $"{s}");
        }

        public static string CreateNodeXPath(XmlNode element)
        {
            var path = "/" + element.Name;

            var parentElement = element.ParentNode as XmlElement;
            if (parentElement != null)
            {
                var siblings = parentElement.SelectNodes(element.Name);
                if (siblings != null && siblings.Count > 1) 
                {
                    var position = 1 + siblings.Cast<XmlElement>().TakeWhile(sibling => sibling != element).Count();
                    path = path + "[" + position + "]";
                }
                path = CreateNodeXPath(parentElement) + path;
            }

            return path;
        }


        private static string CreateBaseXPath(XPath path, XPathCreationOptions options, Func<string, string> pathPattern)
        {

            if (string.IsNullOrEmpty(path.Path) && !options.HasFlag(XPathCreationOptions.AllowEmpty))
                return options == XPathCreationOptions.Absolute ? "/*" : "//*";
            if (string.IsNullOrEmpty(path.Path) && options.HasFlag(XPathCreationOptions.AllowEmpty))
                return string.Empty;

            var firstElementNavigator = options == XPathCreationOptions.Absolute ? "/" : "//";

            var xPath = firstElementNavigator;

            var elementNavigator = options == XPathCreationOptions.Relative ? "//" : "/";
            var first = true;
            foreach (var segment in path.Segments)
            {
                if (segment.Length == 0)
                    continue;
                if (first)
                    first = false;
                else
                    xPath += elementNavigator;      
                xPath += pathPattern(segment);
            }

            return xPath;
        }
    }


    public class XPath {
        public string Path { get; }
        public bool NavigateAttributeWise { get; }

        public IEnumerable<string> Segments => Path.Split(NavigateAttributeWise ? '/' : '.');

        public XPath(string path, bool navigateAttributeWise)
        {
            Path = path;
            NavigateAttributeWise = navigateAttributeWise;
        }
    }

    [Flags]
    public enum XPathCreationOptions
    {
        Relative = 0,
        Absolute = 1,
        FirstRelative = 2,
        AllowEmpty = 4
    }
}
