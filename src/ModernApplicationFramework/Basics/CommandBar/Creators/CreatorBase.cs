using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Utilities;

namespace ModernApplicationFramework.Basics.CommandBar.Creators
{
    /// <inheritdoc />
    /// <summary>
    /// Implementation of <see cref="T:ModernApplicationFramework.Interfaces.Utilities.ICreatorBase" />
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Interfaces.Utilities.ICreatorBase" />
    public abstract class CreatorBase : ICreatorBase
    {
        /// <inheritdoc />
        /// <summary>
        /// Creates a sub-tree of an <see cref="T:System.Windows.Controls.ItemsControl" /> recursively 
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="T:System.Windows.Controls.ItemsControl" /></typeparam>
        /// <param name="itemsControl">The <see cref="T:System.Windows.Controls.ItemsControl" /> that should be filled</param>
        /// <param name="itemDefinition">The data model of the current item</param>
        public abstract void CreateRecursive<T>(ref T itemsControl, CommandBarDataSource itemDefinition)
            where T : ItemsControl;

        public abstract void CreateRecursive<T>(ref T itemsControl, CommandBarDataSource itemDefinition, 
            IReadOnlyList<CommandBarGroup> groups, Func<CommandBarGroup ,IReadOnlyList<CommandBarItemDataSource>> itemFunc) where T : ItemsControl;

        /// <inheritdoc />
        /// <summary>
        /// Gets the single sub definitions.
        /// </summary>
        /// <param name="menuDefinition">The menu definition.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public IEnumerable<CommandBarItemDataSource> GetSingleSubDefinitions(CommandBarDataSource menuDefinition,
            CommandBarCreationOptions options = CommandBarCreationOptions.DisplaySeparatorsOnlyIfGroupNotEmpty)
        {

            var host = IoC.Get<ICommandBarDefinitionHost>();
            var groups = host.ItemGroupDefinitions.Where(x => x.Parent == menuDefinition)
                .Where(x => !host.ExcludedItemDefinitions.Contains(x))
                .Where(x => x.Items.Any(y => y.IsVisible))
                .OrderBy(x => x.SortOrder)
                .ToList();

            return GetSingleSubDefinitions(menuDefinition, groups, group =>
            {
                return host.ItemDefinitions.OfType<CommandBarItemDataSource>().Where(x => x.Group == group)
                    .Where(x => !host.ExcludedItemDefinitions.Contains(x))
                    .OrderBy(x => x.SortOrder).ToList();
            });


            //var list = new List<CommandBarItemDefinition>();
            //var host = IoC.Get<ICommandBarDefinitionHost>();

            //var groups = host.ItemGroupDefinitions.Where(x => x.Parent == menuDefinition)
            //    .Where(x => !host.ExcludedItemDefinitions.Contains(x))
            //    .Where(x => x.Items.Any(y => y.IsVisible))
            //    .OrderBy(x => x.SortOrder)
            //    .ToList();
            //for (var i = 0; i < groups.Count; i++)
            //{
            //    var group = groups[i];
            //    var menuItems = host.ItemDefinitions.Where(x => x.Group == group)
            //        .Where(x => !host.ExcludedItemDefinitions.Contains(x))
            //        .OrderBy(x => x.SortOrder);
            //    if (i > 0 && i <= groups.Count - 1)
            //    {
            //        if (options == CommandBarCreationOptions.DisplaySeparatorsOnlyIfGroupNotEmpty)
            //        {
            //            if (menuItems.Any(x => x.IsVisible))
            //            {
            //                var separatorDefinition = CommandBarSeparatorDefinition.SeparatorDefinition;
            //                separatorDefinition.Group = groups[i - 1];
            //                list.Add(separatorDefinition);
            //            }
            //        }
            //        else
            //        {
            //            var separatorDefinition = CommandBarSeparatorDefinition.SeparatorDefinition;
            //            separatorDefinition.Group = groups[i - 1];
            //            list.Add(separatorDefinition);
            //        }
            //    }
            //    list.AddRange(menuItems);
            //}
            //return list;
        }


        public IEnumerable<CommandBarItemDataSource> GetSingleSubDefinitions(CommandBarDataSource menuDefinition, IReadOnlyList<CommandBarGroup> groups,
            Func<CommandBarGroup ,IReadOnlyList<CommandBarItemDataSource>> items,
            CommandBarCreationOptions options = CommandBarCreationOptions.DisplaySeparatorsOnlyIfGroupNotEmpty)
        {
            if (options.HasFlag(CommandBarCreationOptions.DisplaySeparatorsInAnyCase) && 
                options.HasFlag(CommandBarCreationOptions.DisplaySeparatorsOnlyIfGroupNotEmpty))
                throw new InvalidOperationException("Invalid flags");

            var list = new List<CommandBarItemDataSource>();

            if (!options.HasFlag(CommandBarCreationOptions.DisplayInvisibleItems))
                groups = groups.Where(x => x.Items.Any(y => y.IsVisible)).ToList();

            groups = groups.OrderBy(x => x.SortOrder).ToList();

            for (var i = 0; i < groups.Count; i++)
            {
                var itemList =  items.Invoke(groups[i]).ToList();
                if (i > 0 && i <= groups.Count - 1)
                {
                    if (options == CommandBarCreationOptions.DisplaySeparatorsOnlyIfGroupNotEmpty)
                    {
                        if (itemList.Any(x => x.IsVisible))
                        {
                            var separatorDefinition = SeparatorDataSource.NewInstance;
                            separatorDefinition.Group = groups[i - 1];
                            list.Add(separatorDefinition);
                        }
                    }
                    else
                    {
                        var separatorDefinition = SeparatorDataSource.NewInstance;
                        separatorDefinition.Group = groups[i - 1];
                        list.Add(separatorDefinition);
                    }
                }
                list.AddRange(itemList);
            }
            return list;
        }
    }
}