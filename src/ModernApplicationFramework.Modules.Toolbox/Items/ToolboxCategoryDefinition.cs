using System;

namespace ModernApplicationFramework.Modules.Toolbox.Items
{
    /// <summary>
    /// Definition of a toolbox category.
    /// </summary>
    public class ToolboxCategoryDefinition
    {
        /// <summary>
        /// The ID of the definition.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Gets or sets a value indicating whether nodes with this definition are visible.
        /// </summary>
        public bool IsVisible { get; set; } = true;

        /// <summary>
        /// The localized name of the definition.
        /// </summary>
        public string Name { get; }

        public ToolboxCategoryDefinition(Guid id, string name)
        {
            Name = name;
            Id = id;
        }
    }
}