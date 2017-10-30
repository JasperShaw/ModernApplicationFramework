using System.Collections.Generic;

namespace ModernApplicationFramework.Settings.XPath
{
    internal class XPath {

        /// <summary>
        /// The path. Separation character is either '/' or '.'. See <see cref="NavigateAttributeWise"/> for information
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// <see langword="true"/> when <see cref="Path"/> contains the attribute names separated by '/'; <see langword="false"/> when <see cref="Path"/> contains the element names separated by '.'
        /// </summary>
        public bool NavigateAttributeWise { get; }

        /// <summary>
        /// A List that contains the pats elements split by the rules specified in <see cref="NavigateAttributeWise"/>
        /// </summary>
        public IEnumerable<string> Segments => Path.Split(NavigateAttributeWise ? '/' : '.');

        public XPath(string path, bool navigateAttributeWise)
        {
            Path = path;
            NavigateAttributeWise = navigateAttributeWise;
        }
    }
}