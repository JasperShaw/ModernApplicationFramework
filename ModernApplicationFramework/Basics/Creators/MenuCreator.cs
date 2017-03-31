using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Basics.Definitions.Menu.ExcludeDefinitions;
using ModernApplicationFramework.Basics.Definitions.Menu.MenuItems;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Interfaces.Command;
using ModernApplicationFramework.Interfaces.Utilities;
using ModernApplicationFramework.Interfaces.ViewModels;
using MenuItem = ModernApplicationFramework.Controls.MenuItem;

namespace ModernApplicationFramework.Basics.Creators
{
    [Export(typeof(IMenuCreator))]
    public class MenuCreator : IMenuCreator
    {
        private readonly MenuItemDefinition[] _menuItems;
        private readonly MenuDefinition[] _excludeMenus;
        private readonly MenuItemGroupDefinition[] _excludeMenuItemGroups;
        private readonly MenuItemDefinition[] _excludeMenuItems;
        private readonly MenuDefinition[] _menus;
        private readonly MenuItemGroupDefinition[] _menuItemGroups;

        [ImportingConstructor]
        public MenuCreator(
            ICommandService commandService,
            [ImportMany] MenuDefinition[] menus,
            [ImportMany] MenuItemGroupDefinition[] menuItemGroups,
            [ImportMany] MenuItemDefinition[] menuItems,
            [ImportMany] ExcludeMenuDefinition[] excludeMenus,
            [ImportMany] ExcludeMenuItemGroupDefinition[] excludeMenuItemGroups,
            [ImportMany] ExcludeMenuItemDefinition[] excludeMenuItems)
        {
            _menuItems = menuItems;
            _excludeMenus = excludeMenus.Select(x => x.ExludedMenuItemDefinition).ToArray();
            _excludeMenuItemGroups = excludeMenuItemGroups.Select(x => x.MenuItemGroupDefinitionToExclude).ToArray();
            _excludeMenuItems = excludeMenuItems.Select(x => x.MenuItemDefinitionToExclude).ToArray();
            _menus = menus;
            _menuItemGroups = menuItemGroups;

        }


        public void CreateMenu(IMenuHostViewModel model)
        {
            var menus = _menus.Where(x => !_excludeMenus.Contains(x)).OrderBy(x => x.SortOrder);
            foreach (var menuDefinition in menus)
            {
                var menuItem = new MenuItem(menuDefinition);
                AddGroupsRecursive(menuDefinition, menuItem);
                model.Items.Add(menuItem);
            }
            foreach (var noGroupMenuItem in _menuItems.Where(x => x.Group == null).OrderBy(x => x.SortOrder))
            {
                var item = new MenuItem(noGroupMenuItem);
                model.Items.Add(item);
            }
        }

        private void AddGroupsRecursive(CommandBarDefinitionBase menu, MenuItem menuItem)
        {
            var groups = _menuItemGroups.Where(x => x.Parent == menu)
                .Where(x => !_excludeMenuItemGroups.Contains(x))
                .OrderBy(x => x.SortOrder)
                .ToList();

            for (var i = 0; i < groups.Count; i++)
            {
                var group = groups[i];
                var menuItems = _menuItems.Where(x => x.Group == group)
                    .Where(x => !_excludeMenuItems.Contains(x))
                    .OrderBy(x => x.SortOrder);

                foreach (var menuItemDefinition in menuItems)
                {
                    MenuItem menuItemControl;
                    if (menuItemDefinition.CommandDefinition is CommandListDefinition)
                        menuItemControl = new DummyListMenuItem(menuItemDefinition, menuItem);
                    else
                        menuItemControl = new MenuItem(menuItemDefinition);
                    AddGroupsRecursive(menuItemDefinition, menuItemControl);
                    menuItem.Items.Add(menuItemControl);
                }
                if (i >= groups.Count - 1 || !menuItems.Any())
                    continue;
                var separator = new MenuItem(new CommandBarSeparatorDefinition());
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