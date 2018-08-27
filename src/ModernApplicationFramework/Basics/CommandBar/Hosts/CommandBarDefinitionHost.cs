using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.CommandBar.Elements;
using ModernApplicationFramework.Core.Comparers;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Basics.CommandBar.Hosts
{
    /// <inheritdoc />
    /// <summary>
    ///     Implementation of <see cref="ICommandBarDefinitionHost" />
    /// </summary>
    /// <seealso cref="T:ICommandBarDefinitionHost" />
    [Export(typeof(ICommandBarDefinitionHost))]
    internal sealed class CommandBarDefinitionHost : ICommandBarDefinitionHost, IPartImportsSatisfiedNotification
    {

        [ImportMany] private List<Lazy<CommandBarItemDataSource>> _items;
        [ImportMany] private List<Lazy<CommandBarItem>> _registeredCommandBarItems;


        [ImportingConstructor]
        internal CommandBarDefinitionHost([ImportMany] CommandBarGroup[] menuItemGroups,
            [ImportMany] CommandBarItemDataSource[] menuItems,
            [ImportMany] ExcludeCommandBarElementDefinition[] excludedItems,
            [ImportMany] ExcludedCommandDefinition[] excludedCommands)
        {
            ItemGroupDefinitions =
                new ObservableCollection<CommandBarGroup>(menuItemGroups.OrderBy(x => x.SortOrder));
            //ItemDefinitions = new ObservableCollection<CommandBarItemDataSource>(menuItems.OrderBy(x => x.SortOrder));
            ItemDefinitions = new ObservableCollection<CommandBarDataSource>();
            ExcludedItemDefinitions = new ObservableCollection<CommandBarDataSource>();
            foreach (var item in excludedItems)
                ExcludedItemDefinitions.Add(item.ExcludedCommandBarDefinition);


            ExcludedCommandDefinitions = new ObservableCollection<CommandDefinitionBase>();
            ExcludedCommandDefinitions.CollectionChanged += ExcludedCommandDefinitions_CollectionChanged;
            foreach (var item in excludedCommands)
                ExcludedCommandDefinitions.Add(item.ExcludedDefinition);

            ItemGroupDefinitions.CollectionChanged += ItemGroupDefinitions_CollectionChanged;
        }

        public ObservableCollection<CommandBarGroup> ItemGroupDefinitions { get; }

        public ObservableCollection<CommandBarDataSource> ItemDefinitions { get; }

        public ObservableCollection<CommandBarDataSource> ExcludedItemDefinitions { get; }

        public ObservableCollection<CommandDefinitionBase> ExcludedCommandDefinitions { get; }

        public IReadOnlyList<CommandBarGroup> GetSortedGroupsOfDefinition(CommandBarDataSource definition, bool onlyGroupsWithVisibleItems = true)
        {
            var groups = ItemGroupDefinitions.Where(x => x.Parent == definition)
                .Where(x => !ExcludedItemDefinitions.Contains(x));
            if (onlyGroupsWithVisibleItems)
                groups = groups.Where(x => x.Items.Any(y => y.IsVisible)).ToList();
            return groups.OrderBy(x => x.SortOrder).ToList();
        }

        public Func<CommandBarGroup, IReadOnlyList<CommandBarItemDataSource>> GetItemsOfGroup => group =>
        {
            var list = ItemDefinitions.OfType<CommandBarItemDataSource>().Where(x => x.Group == group)
                .Where(x => !ExcludedItemDefinitions.Contains(x))
                .OrderBy(x => x.SortOrder).ToList();

            //list.RemoveAll(x => x.CommandDefinition is CommandDefinition cd && cd.Command.Status == 16);

            return list;
        };

        private void ExcludedCommandDefinitions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (var item in e.NewItems)
                {
                    foreach (var itemDefinition in ItemDefinitions)
                        if (itemDefinition.CommandDefinition.GetType() == item.GetType())
                            ExcludedItemDefinitions.Add(itemDefinition);
                    if (item is CommandDefinition commandDefinition)
                        commandDefinition.AllowExecution = false;
                }

            if (e.OldItems != null)
                foreach (var item in e.OldItems)
                {
                    foreach (var itemDefinition in ExcludedItemDefinitions.ToList())
                        if (itemDefinition.CommandDefinition.GetType() == item.GetType())
                            ExcludedItemDefinitions.Remove(itemDefinition);
                    if (item is CommandDefinition commandDefinition)
                        commandDefinition.AllowExecution = true;
                }
        }

        private void ItemGroupDefinitions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (var item in e.NewItems)
                    if (item is CommandBarGroup groupDefinition)
                    {
                        var items = ItemDefinitions.OfType<CommandBarItemDataSource>().Where(x => x.Group == groupDefinition);
                        foreach (var itemDefinition in items)
                            if (!groupDefinition.Items.Contains(itemDefinition))
                                groupDefinition.Items.AddSorted(itemDefinition,
                                    new SortOrderComparer<CommandBarDataSource>());
                    }

            if (e.OldItems != null)
                foreach (var item in e.OldItems)
                    if (item is CommandBarGroup groupDefinition)
                    {
                        groupDefinition.Parent.ContainedGroups.Remove(groupDefinition);
                        groupDefinition.Items.Clear();
                    }
        }

        private void ItemDefinitions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (var item in e.NewItems)
                    if (item is CommandBarItemDataSource itemDefinition)
                    {
                        if (ExcludedCommandDefinitions.Any(
                            x => x.GetType() == itemDefinition.CommandDefinition.GetType()))
                        {
                            ItemDefinitions.Remove(itemDefinition);
                            continue;
                        }

                        if (!itemDefinition.Group.Items.Contains(itemDefinition))
                            itemDefinition.Group.Items.AddSorted(itemDefinition,
                                new SortOrderComparer<CommandBarDataSource>());
                    }
            if (e.OldItems != null)
                foreach (var item in e.OldItems)
                    if (item is CommandBarItemDataSource itemDefinition)
                        itemDefinition.Group.Items.Remove(itemDefinition);
        }

        public void OnImportsSatisfied()
        {

            var items = _items.Select(x => x.Value).OfType<CommandBarDataSource>().ToList();
            items.AddRange(_registeredCommandBarItems.Select(x => x.Value.ItemDataSource));

            foreach (var item in items)
            {
                ItemDefinitions.Add(item);
            }


            foreach (var itemDefinition in ItemDefinitions.OfType<CommandBarItemDataSource>())
            {
                var group = ItemGroupDefinitions.FirstOrDefault(x => x == itemDefinition.Group);
                group?.Items.AddSorted(itemDefinition, new SortOrderComparer<CommandBarDataSource>());
            }

            ItemDefinitions.CollectionChanged += ItemDefinitions_CollectionChanged;
        }
    }
}