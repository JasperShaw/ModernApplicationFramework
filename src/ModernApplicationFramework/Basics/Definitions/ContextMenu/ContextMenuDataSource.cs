using System;
using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Basics.Definitions.ContextMenu
{
    /// <inheritdoc cref="CommandBarDataSource" />
    /// <summary>
    /// A command bar definition that specifies a context menu
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Basics.Definitions.CommandBar.CommandBarDefinitionBase" />
    /// <seealso cref="T:ModernApplicationFramework.Interfaces.IHasInternalName" />
    public class ContextMenuDataSource : CommandBarDataSource
    {
        /// <summary>
        /// The category of the context menu
        /// </summary>
        public ContextMenuCategory Category { get; }

        public override Guid Id { get; }

        public ContextMenuDataSource(Guid id, ContextMenuCategory category, string text) : base($"{category.CategoryName} | {text}",
            uint.MinValue, false, false)
        {
            Id = id;
            Category = category;
        }
    }
}