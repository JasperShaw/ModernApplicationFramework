using System;
using ModernApplicationFramework.Basics.CommandBar.DataSources;

namespace ModernApplicationFramework.Basics.CommandBar.Elements
{
    public class CommandBarConxtexMenu : CommandBarItem
    {
        public override CommandBarDataSource ItemDataSource { get; }

        public CommandBarConxtexMenu(Guid id, ContextMenuCategory category, string text)
        {
            ItemDataSource = new ContextMenuDataSource(id, category, $"{category.CategoryName} | {text}");
        }
    }
}
