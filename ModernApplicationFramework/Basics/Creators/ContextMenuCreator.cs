using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.ContextMenu;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Interfaces;
using ContextMenu = ModernApplicationFramework.Controls.ContextMenu;
using MenuItem = ModernApplicationFramework.Controls.MenuItem;

namespace ModernApplicationFramework.Basics.Creators
{
    [Export(typeof(IContextMenuCreator))]
    public class ContextMenuCreator : IContextMenuCreator
    {
        public ContextMenu CreateContextMenu(IContextMenuHost model, CommandBarDefinitionBase contextMenuDefinition)
        {
            var contextMenu = new ContextMenu(contextMenuDefinition);
            AddGroupsRecursive(model, contextMenuDefinition, contextMenu);
            return contextMenu;
        }

        public void CreateContextMenuTree(CommandBarDefinitionBase definition, ItemsControl contextMenu)
        {
            var host = IoC.Get<IContextMenuHost>();
            contextMenu.Items.Clear();

            var groups = host.MenuItemGroupDefinitions.Where(x => x.Parent == definition)
                .Where(x => !host.ExcludedContextMenuElementDefinitions.Contains(x))
                .OrderBy(x => x.SortOrder)
                .ToList();
            for (var i = 0; i < groups.Count; i++)
            {
                var group = groups[i];
                var menuItems = host.MenuItemDefinitions.Where(x => x.Group == group)
                    .Where(x => !host.ExcludedContextMenuElementDefinitions.Contains(x))
                    .OrderBy(x => x.SortOrder);


                if (i > 0 && i <= groups.Count - 1 && menuItems.Any())
                {
                    if (menuItems.Any(menuItemDefinition => menuItemDefinition.IsVisible))
                    {
                        var separator = new MenuItem(CommandBarSeparatorDefinition.MenuSeparatorDefinition);
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
                    CreateContextMenuTree(menuItemDefinition, menuItemControl);
                    contextMenu.Items.Add(menuItemControl);
                }
            }
        }

        public IEnumerable GetContextMenuItemDefinitions(CommandBarDefinitionBase contextMenuDefinition)
        {
            var list = new List<CommandBarDefinitionBase>();
            var model = IoC.Get<IContextMenuHost>();

            if (contextMenuDefinition is ContextMenuDefinition)
            {
                var groups = model.MenuItemGroupDefinitions.Where(x => x.Parent == contextMenuDefinition)
                    .OrderBy(x => x.SortOrder)
                    .ToList();

                for (var i = 0; i < groups.Count; i++)
                {
                    var group = groups[i];
                    var menuItems = model.MenuItemDefinitions.Where(x => x.Group == group)
                        .OrderBy(x => x.SortOrder);
                    if (i > 0 && i <= groups.Count - 1 && menuItems.Any())
                        if (menuItems.Any(menuItemDefinition => menuItemDefinition.IsVisible))
                            list.Add(CommandBarSeparatorDefinition.MenuSeparatorDefinition);
                    list.AddRange(menuItems);
                }
            }
            else
            {
                
            }
            return list;
        }

        private void AddGroupsRecursive(IContextMenuHost model, CommandBarDefinitionBase contextMenuDefinition, ItemsControl contextMenu)
        {
            var groups = model.MenuItemGroupDefinitions.Where(x => x.Parent == contextMenuDefinition)
                .Where(x => !model.ExcludedContextMenuElementDefinitions.Contains(x))
                .OrderBy(x => x.SortOrder)
                .ToList();
            for (var i = 0; i < groups.Count; i++)
            {
                var group = groups[i];
                var menuItems = model.MenuItemDefinitions.Where(x => x.Group == group)
                    .Where(x => !model.ExcludedContextMenuElementDefinitions.Contains(x))
                    .OrderBy(x => x.SortOrder);

                if (i > 0 && i <= groups.Count - 1 && menuItems.Any())
                {
                    if (menuItems.Any(menuItemDefinition => menuItemDefinition.IsVisible))
                    {
                        var separator = new MenuItem(CommandBarSeparatorDefinition.MenuSeparatorDefinition);
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
                    AddGroupsRecursive(model, menuItemDefinition, menuItemControl);
                    contextMenu.Items.Add(menuItemControl);
                }
            }
        }
    }

    public interface IContextMenuCreator
    {
        ContextMenu CreateContextMenu(IContextMenuHost model, CommandBarDefinitionBase contextMenuDefinition);
        void CreateContextMenuTree(CommandBarDefinitionBase definition, ItemsControl contextMenu);
        IEnumerable GetContextMenuItemDefinitions(CommandBarDefinitionBase contextMenuDefinition);
    }
}