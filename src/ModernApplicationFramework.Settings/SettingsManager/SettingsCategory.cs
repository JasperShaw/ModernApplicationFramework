using System;
using System.Collections.Generic;
using ModernApplicationFramework.Settings.Interfaces;

namespace ModernApplicationFramework.Settings.SettingsManager
{
    /// <inheritdoc />
    /// <summary>
    /// Category informations are required to manage and organize different settings. It has a A hierarchical structure 
    /// </summary>
    /// <seealso cref="ISettingsCategory" />
    public sealed class SettingsCategory : ISettingsCategory
    {
        /// <inheritdoc />
        /// <summary>
        /// The parent category
        /// </summary>
        public ISettingsCategory Parent { get; }

        public IList<ISettingsCategory> Children { get; }

        public Guid Id { get; }
        
        public SettingsCategoryType CategoryType { get; }

        /// <inheritdoc />
        /// <summary>
        /// The name of the category.
        /// </summary>
        public string Name { get; }


        public bool HasChildren => Children.Count > 0;

        public SettingsCategory(Guid id, SettingsCategoryType type, string name, ISettingsCategory parent)
        {
            Children = new List<ISettingsCategory>();
            CategoryType = type;
            Name = name;
            Parent = parent;
           if (parent != null)
               Parent.Children.Add(this);
            Id = id;
        }

        /// <inheritdoc />
        /// <summary>
        /// All parents of this Category in a row
        /// </summary>
        public IEnumerable<ISettingsCategory> Path
        {
            get
            {
                var path = new LinkedList<ISettingsCategory>();
                path.AddFirst(this);
                var c = this as ISettingsCategory;
                while (c.Parent != null)
                {
                    path.AddFirst(c.Parent);
                    c = c.Parent;
                }
                return path;
            }
        }
    }

    public enum SettingsCategoryType
    {
        Normal,
        ToolsOption,
        ToolsOptionSub,
    }
}