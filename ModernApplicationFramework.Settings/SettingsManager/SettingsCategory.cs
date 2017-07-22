using System.Collections.Generic;
using ModernApplicationFramework.Settings.Interfaces;

namespace ModernApplicationFramework.Settings.SettingsManager
{
    /// <inheritdoc />
    /// <summary>
    /// Category informations are required to manage and organize different settings. It has a A hierarchical structure 
    /// </summary>
    /// <seealso cref="ISettingsCategory" />
    public class SettingsCategory : ISettingsCategory
    {
        /// <inheritdoc />
        /// <summary>
        /// The parent category
        /// </summary>
        public ISettingsCategory Parent { get; }

        /// <inheritdoc />
        /// <summary>
        /// The name of the category.
        /// </summary>
        public string Name { get; }

        /// <inheritdoc />
        /// <summary>
        /// The localized, displayed name of this category
        /// </summary>
        public string Text { get; }

        /// <inheritdoc />
        /// <summary>
        /// The sorting order of this category
        /// </summary>
        public uint SortOrder { get; }

        /// <inheritdoc />
        /// <summary>
        /// The root Category. <see langword="null" /> when the current category is root
        /// </summary>
        public ISettingsCategory Root { get; }

        /// <inheritdoc />
        /// <summary>
        /// All Sub categories
        /// </summary>
        public IList<ISettingsCategory> Children { get; }

        /// <inheritdoc />
        /// <summary>
        /// Indicates whether this category will have an <c>ToolsOptionsCategory</c> XML-tag in the settings file
        /// </summary>
        public bool IsToolsOptionsCategory { get; } = true;

        public SettingsCategory(string name, string text, uint sortOrder, ISettingsCategory parent, bool isToolsOptionsCategory = true)
        {
            SortOrder = sortOrder;
            Name = name;
            Text = text;
            Parent = parent;
            Children = new List<ISettingsCategory>();

            if (parent == null)
                return;

            Parent.Children.Add(this);
            Root = parent.Root;
            IsToolsOptionsCategory = isToolsOptionsCategory;
        }

        public SettingsCategory(string name, string text, uint sortOrder, bool isToolsOptionsCategory = true) 
            : this(name, text, sortOrder,null, isToolsOptionsCategory)
        {
            Root = this;
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
}