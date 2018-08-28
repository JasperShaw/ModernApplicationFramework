using System;
using ModernApplicationFramework.Basics.Definitions.Command;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    /// <inheritdoc cref="CommandBarDataSource" />
    /// <summary>
    /// A command bar definition that specifies a context menu
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Basics.Definitions.CommandBar.CommandBarDefinitionBase" />
    /// <seealso cref="T:ModernApplicationFramework.Interfaces.IHasInternalName" />
    internal class ContextMenuDataSource : CommandBarDataSource
    {
        /// <summary>
        /// The category of the context menu
        /// </summary>
        public ContextMenuCategory Category { get; }

        public override Guid Id { get; }

        public override CommandControlTypes UiType => CommandControlTypes.ContextMenu;

        public ContextMenuDataSource(Guid id, ContextMenuCategory category, string text) : base(text, false)
        {
            Id = id;
            Category = category;
        }
    }
}