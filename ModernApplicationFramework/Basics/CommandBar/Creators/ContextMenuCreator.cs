using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.CommandBar.Hosts;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Controls;
using ContextMenu = ModernApplicationFramework.Controls.ContextMenu;
using MenuItem = ModernApplicationFramework.Controls.MenuItem;

namespace ModernApplicationFramework.Basics.CommandBar.Creators
{
    [Export(typeof(IContextMenuCreator))]
    public class ContextMenuCreator : IContextMenuCreator
    {
        public ContextMenu CreateContextMenu(CommandBarDefinitionBase contextMenuDefinition)
        {
            var contextMenu = new ContextMenu(contextMenuDefinition);
            AddGroupsRecursive(contextMenuDefinition, contextMenu);
            return contextMenu;
        }

        public void CreateContextMenuTree(CommandBarDefinitionBase definition, ItemsControl contextMenu)
        {
            var host = IoC.Get<ICommandBarDefinitionHost>();
            contextMenu.Items.Clear();

            var groups = host.ItemGroupDefinitions.Where(x => x.Parent == definition)
                .Where(x => !host.ExcludedItemDefinitions.Contains(x))
                .OrderBy(x => x.SortOrder)
                .ToList();
            for (var i = 0; i < groups.Count; i++)
            {
                var group = groups[i];
                var menuItems = host.ItemDefinitions.Where(x => x.Group == group)
                    .Where(x => !host.ExcludedItemDefinitions.Contains(x))
                    .OrderBy(x => x.SortOrder);

                var firstItem = false;
                if (i > 0 && i <= groups.Count - 1 && menuItems.Any())
                {
                    if (menuItems.Any(menuItemDefinition => menuItemDefinition.IsVisible))
                    {
                        var separator = new MenuItem(CommandBarSeparatorDefinition.SeparatorDefinition);
                        contextMenu.Items.Add(separator);
                        firstItem = true;
                    }
                }

                uint newSortOrder = 0;
                foreach (var menuItemDefinition in menuItems)
                {
                    MenuItem menuItemControl;
                    if (menuItemDefinition.CommandDefinition is CommandListDefinition)
                        menuItemControl = new DummyListMenuItem(menuItemDefinition, contextMenu);
                    else
                        menuItemControl = new MenuItem(menuItemDefinition);
                    menuItemDefinition.PrecededBySeparator = firstItem;
                    firstItem = false;
                    CreateContextMenuTree(menuItemDefinition, menuItemControl);
                    contextMenu.Items.Add(menuItemControl);
                    menuItemDefinition.SortOrder = newSortOrder++;
                }
            }
        }

        public IEnumerable<CommandBarItemDefinition> GetContextMenuItemDefinitions(CommandBarDefinitionBase contextMenuDefinition)
        {
            var list = new List<CommandBarItemDefinition>();
            var host = IoC.Get<ICommandBarDefinitionHost>();

            if (contextMenuDefinition is Definitions.ContextMenu.ContextMenuDefinition)
            {
                var groups = host.ItemGroupDefinitions.Where(x => x.Parent == contextMenuDefinition)
                    .OrderBy(x => x.SortOrder)
                    .ToList();

                for (var i = 0; i < groups.Count; i++)
                {
                    var group = groups[i];
                    var menuItems = host.ItemDefinitions.Where(x => x.Group == group)
                        .OrderBy(x => x.SortOrder);
                    if (i > 0 && i <= groups.Count - 1 && menuItems.Any())
                        if (menuItems.Any(menuItemDefinition => menuItemDefinition.IsVisible))
                            list.Add(CommandBarSeparatorDefinition.SeparatorDefinition);
                    list.AddRange(menuItems);
                }
            }
            else if (contextMenuDefinition is MenuDefinition || contextMenuDefinition is CommandBarItemDefinition)
            {
                var groups = host.ItemGroupDefinitions.Where(x => x.Parent == contextMenuDefinition)
                    .OrderBy(x => x.SortOrder)
                    .ToList();

                uint newSortOrder = 0;  //As Menus are created each click we need to to this also in this methods

                for (var i = 0; i < groups.Count; i++)
                {
                    var group = groups[i];
                    var menuItems = host.ItemDefinitions.Where(x => x.Group == group)
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

        private void AddGroupsRecursive(CommandBarDefinitionBase contextMenuDefinition, ItemsControl contextMenu)
        {
            var host = IoC.Get<ICommandBarDefinitionHost>();

            var groups = host.ItemGroupDefinitions.Where(x => x.Parent == contextMenuDefinition)
                .Where(x => !host.ExcludedItemDefinitions.Contains(x))
                .OrderBy(x => x.SortOrder)
                .ToList();
            for (var i = 0; i < groups.Count; i++)
            {
                var group = groups[i];
                var menuItems = host.ItemDefinitions.Where(x => x.Group == group)
                    .Where(x => !host.ExcludedItemDefinitions.Contains(x))
                    .OrderBy(x => x.SortOrder);

                if (i > 0 && i <= groups.Count - 1 && menuItems.Any())
                {
                    if (menuItems.Any(menuItemDefinition => menuItemDefinition.IsVisible))
                    {
                        var separator = new MenuItem(CommandBarSeparatorDefinition.SeparatorDefinition);
                        contextMenu.Items.Add(separator);
                    }
                }

                foreach (var menuItemDefinition in menuItems)
                {
                    MenuItem menuItemControl;
                    if (menuItemDefinition.CommandDefinition is CommandListDefinition)
                        menuItemControl = new DummyListMenuItem(menuItemDefinition, contextMenu);
                    else
                        menuItemControl = new MenuItem(menuItemDefinition);
                    AddGroupsRecursive(menuItemDefinition, menuItemControl);
                    contextMenu.Items.Add(menuItemControl);
                }
            }
        }
    }

    public interface IContextMenuCreator
    {
        ContextMenu CreateContextMenu(CommandBarDefinitionBase contextMenuDefinition);
        void CreateContextMenuTree(CommandBarDefinitionBase definition, ItemsControl contextMenu);
        IEnumerable<CommandBarItemDefinition> GetContextMenuItemDefinitions(CommandBarDefinitionBase contextMenuDefinition);
    }
}