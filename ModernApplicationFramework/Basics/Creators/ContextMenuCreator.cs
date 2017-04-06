using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
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
            var contextMenu = new ContextMenu();
            AddGroupsRecursive(model, contextMenuDefinition, contextMenu);
            return contextMenu;
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
                if (i >= groups.Count - 1 || !menuItems.Any())
                    continue;
                var separator = new MenuItem(CommandBarSeparatorDefinition.MenuSeparatorDefinition);
                contextMenu.Items.Add(separator);
            }
        }
    }

    public interface IContextMenuCreator
    {
        ContextMenu CreateContextMenu(IContextMenuHost model, CommandBarDefinitionBase contextMenuDefinition);
    }
}