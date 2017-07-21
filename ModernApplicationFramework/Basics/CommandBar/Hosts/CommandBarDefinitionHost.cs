using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
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
    public sealed class CommandBarDefinitionHost : ICommandBarDefinitionHost
    {
        [ImportingConstructor]
        public CommandBarDefinitionHost([ImportMany] CommandBarGroupDefinition[] menuItemGroups,
            [ImportMany] CommandBarItemDefinition[] menuItems,
            [ImportMany] ExcludeCommandBarElementDefinition[] excludedItems,
            [ImportMany] ExcludedCommandDefinition[] excludedCommands)
        {
            ItemGroupDefinitions =
                new ObservableCollection<CommandBarGroupDefinition>(menuItemGroups.OrderBy(x => x.SortOrder));
            ItemDefinitions = new ObservableCollection<CommandBarItemDefinition>(menuItems.OrderBy(x => x.SortOrder));
            ExcludedItemDefinitions = new ObservableCollection<CommandBarDefinitionBase>();
            foreach (var item in excludedItems)
                ExcludedItemDefinitions.Add(item.ExcludedCommandBarDefinition);


            ExcludedCommandDefinitions = new ObservableCollection<DefinitionBase>();
            ExcludedCommandDefinitions.CollectionChanged += ExcludedCommandDefinitions_CollectionChanged;
            foreach (var item in excludedCommands)
                ExcludedCommandDefinitions.Add(item.ExcludedDefinition);

            foreach (var itemDefinition in ItemDefinitions)
            {
                var group = ItemGroupDefinitions.FirstOrDefault(x => x == itemDefinition.Group);
                group?.Items.AddSorted(itemDefinition, new SortOrderComparer<CommandBarDefinitionBase>());
            }

            ItemDefinitions.CollectionChanged += ItemDefinitions_CollectionChanged;
            ItemGroupDefinitions.CollectionChanged += ItemGroupDefinitions_CollectionChanged;
        }

        public ObservableCollection<CommandBarGroupDefinition> ItemGroupDefinitions { get; }

        public ObservableCollection<CommandBarItemDefinition> ItemDefinitions { get; }

        public ObservableCollection<CommandBarDefinitionBase> ExcludedItemDefinitions { get; }

        public ObservableCollection<DefinitionBase> ExcludedCommandDefinitions { get; }

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
                    if (item is CommandBarGroupDefinition groupDefinition)
                    {
                        var items = ItemDefinitions.Where(x => x.Group == groupDefinition);
                        foreach (var itemDefinition in items)
                            if (!groupDefinition.Items.Contains(itemDefinition))
                                groupDefinition.Items.AddSorted(itemDefinition,
                                    new SortOrderComparer<CommandBarDefinitionBase>());
                    }

            if (e.OldItems != null)
                foreach (var item in e.OldItems)
                    if (item is CommandBarGroupDefinition groupDefinition)
                    {
                        groupDefinition.Parent.ContainedGroups.Remove(groupDefinition);
                        groupDefinition.Items.Clear();
                    }
        }

        private void ItemDefinitions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (var item in e.NewItems)
                    if (item is CommandBarItemDefinition itemDefinition)
                    {
                        if (ExcludedCommandDefinitions.Any(
                            x => x.GetType() == itemDefinition.CommandDefinition.GetType()))
                        {
                            ItemDefinitions.Remove(itemDefinition);
                            continue;
                        }

                        if (!itemDefinition.Group.Items.Contains(itemDefinition))
                            itemDefinition.Group.Items.AddSorted(itemDefinition,
                                new SortOrderComparer<CommandBarDefinitionBase>());
                    }
            if (e.OldItems != null)
                foreach (var item in e.OldItems)
                    if (item is CommandBarItemDefinition itemDefinition)
                        itemDefinition.Group.Items.Remove(itemDefinition);
        }
    }
}