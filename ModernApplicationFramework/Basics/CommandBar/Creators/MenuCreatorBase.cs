using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Interfaces;
using MenuItem = ModernApplicationFramework.Controls.MenuItem;

namespace ModernApplicationFramework.Basics.CommandBar.Creators
{
    public abstract class MenuCreatorBase
    {
        public void CreateMenuTree(CommandBarDefinitionBase definition, ItemsControl menuItem)
        {
            var host = IoC.Get<ICommandBarDefinitionHost>();
            menuItem.Items.Clear();

            var groups = host.ItemGroupDefinitions.Where(x => x.Parent == definition)
                .Where(x => !host.ExcludedItemDefinitions.Contains(x))
                .OrderBy(x => x.SortOrder)
                .ToList();

            var veryFirstItem = true;
            for (var i = 0; i < groups.Count; i++)
            {
                var group = groups[i];
                var menuItems = host.ItemDefinitions.Where(x => x.Group == group)
                    .Where(x => !host.ExcludedItemDefinitions.Contains(x))
                    .OrderBy(x => x.SortOrder);


                var precededBySeparator = false;
                if (i > 0 && i <= groups.Count - 1 && menuItems.Any())
                {
                    if (menuItems.Any(menuItemDefinition => menuItemDefinition.IsVisible))
                    {
                        var separatorDefinition = CommandBarSeparatorDefinition.SeparatorDefinition;
                        separatorDefinition.Group = groups[i - 1];
                        var separator = new MenuItem(separatorDefinition);
                        menuItem.Items.Add(separator);
                        precededBySeparator = true;
                    }
                }
                uint newSortOrder = 0;

                foreach (var menuItemDefinition in menuItems)
                {
                    MenuItem menuItemControl;
                    if (menuItemDefinition.CommandDefinition is CommandListDefinition)
                        menuItemControl = new DummyListMenuItem(menuItemDefinition, menuItem);
                    else
                        menuItemControl = new MenuItem(menuItemDefinition);
                    menuItemDefinition.PrecededBySeparator = precededBySeparator;
                    precededBySeparator = false;
                    CreateMenuTree(menuItemDefinition, menuItemControl);
                    menuItem.Items.Add(menuItemControl);
                    menuItemDefinition.SortOrder = newSortOrder++;
                    menuItemDefinition.IsVeryFirst = veryFirstItem;
                    veryFirstItem = false;
                }
            }
        }

        public IEnumerable<CommandBarItemDefinition> GetSingleSubDefinitions(CommandBarDefinitionBase contextMenuDefinition)
        {
            var list = new List<CommandBarItemDefinition>();
            var host = IoC.Get<ICommandBarDefinitionHost>();

            var groups = host.ItemGroupDefinitions.Where(x => x.Parent == contextMenuDefinition)
                .OrderBy(x => x.SortOrder)
                .ToList();
            for (var i = 0; i < groups.Count; i++)
            {
                var group = groups[i];
                var menuItems = host.ItemDefinitions.Where(x => x.Group == group).OrderBy(x => x.SortOrder);

                if (i > 0 && i <= groups.Count - 1 && menuItems.Any())
                    if (menuItems.Any())
                    {
                        var separatorDefinition = CommandBarSeparatorDefinition.SeparatorDefinition;
                        separatorDefinition.Group = groups[i - 1];
                        list.Add(separatorDefinition);
                    }
                list.AddRange(menuItems);
            }
            return list;
        }
    }
}
