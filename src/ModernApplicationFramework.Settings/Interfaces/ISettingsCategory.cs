using System;
using System.Collections.Generic;
using ModernApplicationFramework.Settings.SettingsManager;

namespace ModernApplicationFramework.Settings.Interfaces
{
    /// <summary>
    /// An <see cref="ISettingsCategory"/> provides information for a settings category.
    /// </summary>
    public interface ISettingsCategory
    {
        Guid Id { get; }
        
        SettingsCategoryType CategoryType { get; }

        /// <summary>
        /// The name of the category.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The parent category
        /// </summary>
        ISettingsCategory Parent { get; }

        IList<ISettingsCategory> Children { get; }

        /// <summary>
        /// All parents of this Category in a row
        /// </summary>
        IEnumerable<ISettingsCategory> Path { get; }
        
        bool HasChildren { get; }
    }
}
