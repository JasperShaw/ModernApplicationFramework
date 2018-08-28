using System;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar.Elements
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
