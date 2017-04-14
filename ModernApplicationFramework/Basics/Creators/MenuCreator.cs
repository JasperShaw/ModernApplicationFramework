using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Interfaces.Utilities;
using ModernApplicationFramework.Interfaces.ViewModels;
using MenuItem = ModernApplicationFramework.Controls.MenuItem;

namespace ModernApplicationFramework.Basics.Creators
{
    [Export(typeof(IMenuCreator))]
    public class MenuCreator : IMenuCreator
    {
        public void CreateMenuBar(IMenuHostViewModel model)
        {
            model.Items.Clear();

            var bars = model.MenuBars.OrderBy(x => x.SortOrder);

            foreach (var bar in bars)
            {
                var group = model.MenuItemGroupDefinitions.FirstOrDefault(x => x.Parent == bar);

                var topLevelMenus = model.MenuItemDefinitions.Where(x => !model.ExcludedMenuElementDefinitions.Contains(x))
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
                }
            }
        }

        public void CreateMenuTree(CommandBarDefinitionBase definition, MenuItem menuItem)
        {
            var host = IoC.Get<IMenuHostViewModel>();
            menuItem.Items.Clear();

            var groups = host.MenuItemGroupDefinitions.Where(x => x.Parent == definition)
                .Where(x => !host.ExcludedMenuElementDefinitions.Contains(x))
                .OrderBy(x => x.SortOrder)
                .ToList();

            for (var i = 0; i < groups.Count; i++)
            {
                var group = groups[i];
                var menuItems = host.MenuItemDefinitions.Where(x => x.Group == group)
                    .Where(x => !host.ExcludedMenuElementDefinitions.Contains(x))
                    .OrderBy(x => x.SortOrder);


                var firstItem = false;
                if (i > 0 && i <= groups.Count - 1 && menuItems.Any())
                {
                    if (menuItems.Any(menuItemDefinition => menuItemDefinition.IsVisible))
                    {
                        var separator = new MenuItem(CommandBarSeparatorDefinition.SeparatorDefinition);
                        menuItem.Items.Add(separator);
                        firstItem = true;
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
                    menuItemDefinition.PrecededBySeparator = firstItem;
                    firstItem = false;
                    CreateMenuTree(menuItemDefinition, menuItemControl);
                    menuItem.Items.Add(menuItemControl);
                    menuItemDefinition.SortOrder = newSortOrder++;
                }
            }
        }


        public IEnumerable<CommandBarItemDefinition> GetSingleSubDefinitions(CommandBarDefinitionBase definition)
        {
            var list = new List<CommandBarItemDefinition>();
            var host = IoC.Get<IMenuHostViewModel>();

            if (definition is MenuBarDefinition barDefinition)
            {
                var group = host.MenuItemGroupDefinitions.FirstOrDefault(x => x.Parent == barDefinition);

                var menus = host.MenuItemDefinitions
                    .Where(x => x.Group == group)
                    .OrderBy(x => x.SortOrder);
                list.AddRange(menus);
            }
            else if (definition is MenuDefinition || definition is CommandBarItemDefinition)
            {
                var groups = host.MenuItemGroupDefinitions.Where(x => x.Parent == definition)
                    .OrderBy(x => x.SortOrder)
                    .ToList();

                uint newSortOrder = 0;  //As Menus are created each click we need to to this also in this methods

                for (var i = 0; i < groups.Count; i++)
                {
                    var group = groups[i];
                    var menuItems = host.MenuItemDefinitions.Where(x => x.Group == group)
                        .OrderBy(x => x.SortOrder);

                    bool firstItem = false; //As Menus are created each click we need to to this also in this methods
                    if (i > 0 && i <= groups.Count - 1 && menuItems.Any())
                        if (menuItems.Any(menuItemDefinition => menuItemDefinition.IsVisible))
                        {
                            list.Add(CommandBarSeparatorDefinition.SeparatorDefinition);
                            firstItem = true;
                        }


                    foreach (var itemDefinition in menuItems)
                    {
                        itemDefinition.PrecededBySeparator = firstItem;
                        list.Add(itemDefinition);
                        itemDefinition.SortOrder = newSortOrder++;
                        firstItem = false;
                    }
                }
            }
            return list;
        }
    }
}