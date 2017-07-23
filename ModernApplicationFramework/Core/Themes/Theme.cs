using System;

namespace ModernApplicationFramework.Core.Themes
{
    /// <summary>
    /// Abstract definition of a UI theme
    /// </summary>
    public abstract class Theme
    {
        /// <summary>
        /// The unlocalized name
        /// </summary>
        public abstract string Name { get; }
        
        /// <summary>
        /// The localized displayed text
        /// </summary>
        public abstract string Text { get; }

        /// <summary>
        /// Gets the resource URI.
        /// </summary>
        /// <returns>The URI</returns>
        public abstract Uri GetResourceUri();
    }
}