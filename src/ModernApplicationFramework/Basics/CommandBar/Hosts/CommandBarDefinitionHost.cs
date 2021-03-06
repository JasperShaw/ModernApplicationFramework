﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Basics.CommandBar.DataSources;
using ModernApplicationFramework.Basics.CommandBar.Elements;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
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
        [ImportMany] private List<Lazy<CommandBarGroup>> _registeredCommandBarGroups;
        [ImportMany] private List<Lazy<ExcludeCommandBarItem>> _excludedItemImports;
        [ImportMany] private List<Lazy<ExcludedItemDefinition>> _excludedItemDefinitionImports;


        private readonly List<CommandBarDataSource> _excludedCommandBarItems = new List<CommandBarDataSource>();
        private readonly List<CommandBarItemDefinition> _excludedItemDefinitions = new List<CommandBarItemDefinition>();

        internal CommandBarDefinitionHost()
        {
            ItemGroupDefinitions = new ObservableCollection<CommandBarGroup>();
            ItemDefinitions = new ObservableCollection<CommandBarDataSource>();
        }

        public ObservableCollection<CommandBarGroup> ItemGroupDefinitions { get; }

        public ObservableCollection<CommandBarDataSource> ItemDefinitions { get; }

        public IReadOnlyCollection<CommandBarDataSource> ExcludedItemDefinitions => _excludedCommandBarItems;

        public IReadOnlyCollection<CommandBarItemDefinition> ExcludedCommandDefinitions => _excludedItemDefinitions;

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
            return list;
        };

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
                            x => x.GetType() == itemDefinition.ItemDefinition.GetType()))
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
            SetupGroups(); 
            SetupItemDefinitions();

            SetupExcludedItems();
            SetupExcludedDefinitions();
        }

        private void SetupExcludedDefinitions()
        {
            foreach (var item in _excludedItemDefinitionImports)
            {
                _excludedItemDefinitions.Add(item.Value.ExcludedDefinition);
                foreach (var itemDefinition in ItemDefinitions.OfType<CommandBarItemDataSource>())
                    if (itemDefinition.ItemDefinition.GetType() == item.GetType())
                        _excludedCommandBarItems.Add(itemDefinition);
                if (item.Value.ExcludedDefinition is CommandDefinition commandDefinition)
                    commandDefinition.AllowExecution = false;
            }
        }

        private void SetupExcludedItems()
        {
            foreach (var item in _excludedItemImports)
                _excludedCommandBarItems.Add(item.Value.ExcludedItem.ItemDataSource);
        }

        private void SetupGroups()
        {
            foreach (var lazy in _registeredCommandBarGroups.OrderBy(x => x.Value.SortOrder))
                ItemGroupDefinitions.Add(lazy.Value);
            ItemGroupDefinitions.CollectionChanged += ItemGroupDefinitions_CollectionChanged;
        }

        private void SetupItemDefinitions()
        {
            var items = _items.Select(x => x.Value).OfType<CommandBarDataSource>().ToList();
            items.AddRange(_registeredCommandBarItems.Select(x => x.Value.ItemDataSource));
            foreach (var item in items)
                ItemDefinitions.Add(item);
            foreach (var itemDefinition in ItemDefinitions.OfType<CommandBarItemDataSource>())
            {
                var group = ItemGroupDefinitions.FirstOrDefault(x => x == itemDefinition.Group);
                group?.Items.AddSorted(itemDefinition, new SortOrderComparer<CommandBarDataSource>());
            }
            ItemDefinitions.CollectionChanged += ItemDefinitions_CollectionChanged;
        }
    }
}