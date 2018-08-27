using System;
using System.Collections.ObjectModel;
using ModernApplicationFramework.Basics.Definitions.Command;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    /// <inheritdoc />
    /// <summary>
    /// A command bar definition that specifies a menu item
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Basics.Definitions.CommandBar.CommandBarItemDefinition" />
    internal class MenuDataSource : CommandBarItemDataSource
    {
        public override Guid Id { get; }

        public MenuDataSource(Guid id, string text, CommandBarGroup group, uint sortOrder, bool isCustom, CommandBarFlags flags)
            : base(text, sortOrder, group, null, isCustom, flags)
        {
            Id = id;
        }

        public override CommandControlTypes UiType => CommandControlTypes.Menu;

        public ObservableCollection<CommandBarItemDataSource> Items { get; set; }
    }
}