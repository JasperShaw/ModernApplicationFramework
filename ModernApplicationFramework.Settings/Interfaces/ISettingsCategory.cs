using System.Collections.Generic;

namespace ModernApplicationFramework.Settings.Interfaces
{
    /// <summary>
    /// An <see cref="ISettingsCategory"/> provides information for a settings category.
    /// </summary>
    public interface ISettingsCategory
    {
        /// <summary>
        /// All Sub categories
        /// </summary>
        IList<ISettingsCategory> Children { get; }

        /// <summary>
        /// Indicates whether this category will have an <c>ToolsOptionsCategory</c> XML-tag in the settings file
        /// </summary>
        bool IsToolsOptionsCategory { get; }

        /// <summary>
        /// The name of the category.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The parent category
        /// </summary>
        ISettingsCategory Parent { get; }

        /// <summary>
        /// All parents of this Category in a row
        /// </summary>
        IEnumerable<ISettingsCategory> Path { get; }

        /// <summary>
        /// The root Category. <see langword="null"/> when the current category is root
        /// </summary>
        ISettingsCategory Root { get; }

        /// <summary>
        /// The sorting order of this cateogry
        /// </summary>
        uint SortOrder { get; }

        /// <summary>
        /// The localized, displayed name of this category
        /// </summary>
        string Text { get; }
    }
}
