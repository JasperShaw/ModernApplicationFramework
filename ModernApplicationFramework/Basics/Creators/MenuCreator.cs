using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Interfaces.Utilities;
using ModernApplicationFramework.Interfaces.ViewModels;
using MenuItem = ModernApplicationFramework.Controls.MenuItem;

namespace ModernApplicationFramework.Basics.Creators
{
    [Export(typeof(IMenuCreator))]
    public class MenuCreator : IMenuCreator
    {
        public void CreateMenu(IMenuHostViewModel model)
        {
            model.Items.Clear();

            var bars = model.MenuBars.OrderBy(x => x.SortOrder);
            foreach (var bar in bars)
            {
                var menus = model.MenuDefinitions.Where(x => !model.ExcludedMenuElementDefinitions.Contains(x))
                    .Where(x => x.MenuBar == bar)
                    .OrderBy(x => x.SortOrder);

                foreach (var menuDefinition in menus)
                {
                    var menuItem = new MenuItem(menuDefinition);
                    AddGroupsRecursive(model, menuDefinition, menuItem);
                    model.Items.Add(menuItem);
                }
                foreach (var noGroupMenuItem in model.MenuItemDefinitions.Where(x => x.Group == null).OrderBy(x => x.SortOrder))
                {
                    var item = new MenuItem(noGroupMenuItem);
                    model.Items.Add(item);
                }
            }
        }

        private void AddGroupsRecursive(IMenuHostViewModel model,CommandBarDefinitionBase menu, MenuItem menuItem)
        {
            var groups = model.MenuItemGroupDefinitions.Where(x => x.Parent == menu)
                .Where(x => !model.ExcludedMenuElementDefinitions.Contains(x))
                .OrderBy(x => x.SortOrder)
                .ToList();

            for (var i = 0; i < groups.Count; i++)
            {
                var group = groups[i];
                var menuItems = model.MenuItemDefinitions.Where(x => x.Group == group)
                    .Where(x => !model.ExcludedMenuElementDefinitions.Contains(x))
                    .OrderBy(x => x.SortOrder);

                foreach (var menuItemDefinition in menuItems)
                {
                    MenuItem menuItemControl;
                    if (menuItemDefinition.CommandDefinition is CommandListDefinition)
                        menuItemControl = new DummyListMenuItem(menuItemDefinition, menuItem);
                    else
                        menuItemControl = new MenuItem(menuItemDefinition);
                    AddGroupsRecursive(model, menuItemDefinition, menuItemControl);
                    menuItem.Items.Add(menuItemControl);
                }
                if (i >= groups.Count - 1 || !menuItems.Any())
                    continue;
                var separator = new MenuItem(CommandBarSeparatorDefinition.MenuSeparatorDefinition);
                menuItem.Items.Add(separator);
            }
        }

        public void CreateMenu(IMenuHostViewModel model, MenuItem item)
        {
            CreateMenu(model);
            model.Items.Add(item);
        }
    }
}