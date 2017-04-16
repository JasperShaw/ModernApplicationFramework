using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Utilities;
using ModernApplicationFramework.Interfaces.ViewModels;
using MenuItem = ModernApplicationFramework.Controls.MenuItem;

namespace ModernApplicationFramework.Basics.CommandBar.Creators
{
    [Export(typeof(IMenuCreator))]
    public class MenuCreator : IMenuCreator
    {
        public void CreateMenuBar(IMenuHostViewModel model)
        {
            model.Items.Clear();
            var host = IoC.Get<ICommandBarDefinitionHost>();

            var bars = model.MenuBars.OrderBy(x => x.SortOrder);

            foreach (var bar in bars)
            {
                var groups = host.ItemGroupDefinitions.Where(x => x.Parent == bar)
                    .Where(x => !host.ExcludedItemDefinitions.Contains(x))
                    .OrderBy(x => x.SortOrder)
                    .ToList();

                var veryFirstItem = true;
                foreach (var group in groups)
                {
                    var topLevelMenus = host.ItemDefinitions.Where(x => !host.ExcludedItemDefinitions.Contains(x))
                        .Where(x => x.Group == group)
                        .OrderBy(x => x.SortOrder);

                    uint newSortOrder = 0;
                    foreach (var menuDefinition in topLevelMenus)
                    {
                        var menuItem = new MenuItem(menuDefinition);

                        //Required so we can call CreateMenuTree() with each submenu open. Do not do this for command items however
                        if (menuDefinition is MenuDefinition)
                            menuItem.Items.Add(new MenuItem());

                        model.Items.Add(menuItem);
                        menuDefinition.SortOrder = newSortOrder++;
                        menuDefinition.IsVeryFirst = veryFirstItem;
                        veryFirstItem = false;
                    }
                }
            }
        }

        public void CreateMenuTree(CommandBarDefinitionBase definition, MenuItem menuItem)
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


        public IEnumerable<CommandBarItemDefinition> GetSingleSubDefinitions(CommandBarDefinitionBase definition)
        {
            var list = new List<CommandBarItemDefinition>();
            var host = IoC.Get<ICommandBarDefinitionHost>();

            if (definition is MenuBarDefinition barDefinition)
            {
                var groups = host.ItemGroupDefinitions.Where(x => x.Parent == barDefinition)
                    .Where(x => !host.ExcludedItemDefinitions.Contains(x))
                    .OrderBy(x => x.SortOrder)
                    .ToList();
                var veryFirstItem = true;
                foreach (var group in groups)
                {
                    var menus = host.ItemDefinitions
                        .Where(x => x.Group == group)
                        .OrderBy(x => x.SortOrder);
                    foreach (var menu in menus)
                    {
                        list.Add(menu);
                        menu.IsVeryFirst = veryFirstItem;
                        veryFirstItem = false;
                    }
                }
            }
            else if (definition is MenuDefinition || definition is CommandBarItemDefinition)
            {

                var groups = host.ItemGroupDefinitions.Where(x => x.Parent == definition)
                    .OrderBy(x => x.SortOrder)
                    .ToList();
                var veryFirstItem = true;
                for (var i = 0; i < groups.Count; i++)
                {
                    var group = groups[i];
                    var menuItems = host.ItemDefinitions.Where(x => x.Group == group)
                        .OrderBy(x => x.SortOrder);

                    bool precededBySeparator = false; //As Menus are created each click we need to to this also in this methods
                    if (i > 0 && i <= groups.Count - 1 && menuItems.Any())
                        if (menuItems.Any(menuItemDefinition => menuItemDefinition.IsVisible))
                        {
                            var separatorDefinition = CommandBarSeparatorDefinition.SeparatorDefinition;
                            separatorDefinition.Group = groups[i-1];
                            list.Add(separatorDefinition);
                            precededBySeparator = true;
                        }

                    uint newSortOrder = 0;  //As Menus are created each click we need to to this also in this methods
                    foreach (var itemDefinition in menuItems)
                    {
                        itemDefinition.PrecededBySeparator = precededBySeparator;
                        list.Add(itemDefinition);
                        itemDefinition.SortOrder = newSortOrder++;
                        precededBySeparator = false;
                        itemDefinition.IsVeryFirst = veryFirstItem;
                        veryFirstItem = false;
                    }
                }
            }
            return list;
        }
    }
}