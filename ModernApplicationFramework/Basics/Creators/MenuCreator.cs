using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
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
        public void CreateMenuBar(IMenuHostViewModel model)
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
                    menuItem.Items.Add(new MenuItem());
                    model.Items.Add(menuItem);
                }
                foreach (var noGroupMenuItem in model.MenuItemGroupDefinitions.Where(x => x.Parent == bar))
                {
                    var menuItems = model.MenuItemDefinitions.Where(x => x.Group == noGroupMenuItem)
                        .Where(x => !model.ExcludedMenuElementDefinitions.Contains(x))
                        .OrderBy(x => x.SortOrder);

                    foreach (var menuItem in menuItems)
                    {
                        var item = new MenuItem(menuItem);
                        model.Items.Add(item);
                    }
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


                if (i > 0 && i <= groups.Count - 1 && menuItems.Any())
                {
                    if (menuItems.Any(menuItemDefinition => menuItemDefinition.IsVisible))
                    {
                        var separator = new MenuItem(CommandBarSeparatorDefinition.MenuSeparatorDefinition);
                        menuItem.Items.Add(separator);
                    }
                }

                foreach (var menuItemDefinition in menuItems)
                {
                    MenuItem menuItemControl;
                    if (menuItemDefinition.CommandDefinition is CommandListDefinition)
                        menuItemControl = new DummyListMenuItem(menuItemDefinition, menuItem);
                    else
                        menuItemControl = new MenuItem(menuItemDefinition);
                    CreateMenuTree(menuItemDefinition, menuItemControl);
                    menuItem.Items.Add(menuItemControl);
                }
            }
        }

        public void CreateMenuBar(IMenuHostViewModel model, MenuItem item)
        {
            CreateMenuBar(model);
            model.Items.Add(item);
        }
    }
}