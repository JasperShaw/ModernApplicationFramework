using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Interfaces.Utilities;
using ModernApplicationFramework.Interfaces.ViewModels;
using MenuItem = ModernApplicationFramework.Controls.Menu.MenuItem;

namespace ModernApplicationFramework.Basics.CommandBar.Creators
{
    /// <inheritdoc cref="IMainMenuCreator" />
    /// <summary>
    /// Implementation of <see cref="IMainMenuCreator"/>
    /// </summary>
    /// <seealso cref="T:MenuCreatorBase" />
    /// <seealso cref="T:IMainMenuCreator" />
    [Export(typeof(IMainMenuCreator))]
    public class MenuCreator : MenuCreatorBase, IMainMenuCreator
    {
        public MenuItem CreateMenuItem(CommandBarDefinitionBase contextMenuDefinition, IReadOnlyList<CommandBarGroupDefinition> groups, Func<CommandBarGroupDefinition, IReadOnlyList<CommandBarItemDefinition>> itemsFunc)
        {
            var menuItem = new MenuItem(contextMenuDefinition);
            CreateRecursive(ref menuItem, contextMenuDefinition, groups, itemsFunc);
            return menuItem;
        }

        public void CreateMenuBar(IMenuHostViewModel model)
        {
            var topLevelDefinitions =
                model.TopLevelDefinitions.Where(x => !model.DefinitionHost.ExcludedItemDefinitions.Contains(x));

            foreach (var topLevelDefinition in topLevelDefinitions)
            {
                var groups = model.DefinitionHost.GetSortedGroupsOfDefinition(topLevelDefinition);
                model.BuildLogical(topLevelDefinition, groups, model.DefinitionHost.GetItemsOfGroup);
                var t = GetSingleSubDefinitions(topLevelDefinition, groups, model.DefinitionHost.GetItemsOfGroup);

                foreach (var definition in t)
                {
                    var menuitem = new MenuItem(definition);
                    groups = model.DefinitionHost.GetSortedGroupsOfDefinition(definition);
                    CreateRecursive(ref menuitem, definition, groups, model.DefinitionHost.GetItemsOfGroup);
                    model.Items.Add(menuitem);
                }
            }
        }
    }
}