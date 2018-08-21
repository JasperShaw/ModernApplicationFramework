using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Controls.Menu;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Basics.CommandBar.Creators
{
    /// <inheritdoc />
    /// <summary>
    /// Basic implementation to create menus
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Basics.CommandBar.Creators.CreatorBase" />
    public abstract class MenuCreatorBase : CreatorBase
    {
        /// <summary>
        /// Creates a sub-tree of an <see cref="T:System.Windows.Controls.ItemsControl" /> recursively
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="T:System.Windows.Controls.ItemsControl" /></typeparam>
        /// <param name="itemsControl">The <see cref="T:System.Windows.Controls.ItemsControl" /> that should be filled</param>
        /// <param name="itemDefinition">The data model of the current item</param>
        /// <inheritdoc />
        public override void CreateRecursive<T>(ref T itemsControl, CommandBarDataSource itemDefinition)
        {
            var topItem = GetSingleSubDefinitions(itemDefinition);
            foreach (var item in topItem)
            {
                MenuItem menuItemControl;
                if (item.CommandDefinition is CommandListDefinition)
                    menuItemControl = new DummyListMenuItem(item, itemsControl);
                else
                    menuItemControl = new MenuItem(item);

                if (item is MenuDefinition)
                    CreateRecursive(ref menuItemControl, item);

                itemsControl.Items.Add(menuItemControl);
            }
        }

        public override void CreateRecursive<T>(ref T itemsControl, CommandBarDataSource itemDefinition, IReadOnlyList<CommandBarGroupDefinition> groups,
            Func<CommandBarGroupDefinition, IReadOnlyList<CommandBarItemDefinition>> itemFunc)
        {

            var host = IoC.Get<ICommandBarDefinitionHost>();

            var topItem = GetSingleSubDefinitions(itemDefinition, groups, itemFunc);
            foreach (var item in topItem)
            {
                MenuItem menuItemControl;
                if (item.CommandDefinition is CommandListDefinition)
                    menuItemControl = new DummyListMenuItem(item, itemsControl);
                else
                    menuItemControl = new MenuItem(item);

                if (item is MenuDefinition)
                {
                    groups = host.ItemGroupDefinitions.Where(x => x.Parent == item)
                        .Where(x => !host.ExcludedItemDefinitions.Contains(x))
                        .Where(x => x.Items.Any(y => y.IsVisible))
                        .OrderBy(x => x.SortOrder)
                        .ToList();
                    CreateRecursive(ref menuItemControl, item, groups, itemFunc);
                }

                itemsControl.Items.Add(menuItemControl);
            }
        }
    }
}