using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Core.Comparers;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Basics.CommandBar.Hosts
{
    [Export(typeof(ICommandBarDefinitionHost))]
    public class CommandBarDefinitionHost : ICommandBarDefinitionHost
    {
        public ObservableCollection<CommandBarGroupDefinition> ItemGroupDefinitions { get; }
        public ObservableCollection<CommandBarItemDefinition> ItemDefinitions { get; }
        public ObservableCollection<CommandBarDefinitionBase> ExcludedItemDefinitions { get; }

        [ImportingConstructor]
        public CommandBarDefinitionHost([ImportMany] CommandBarGroupDefinition[] menuItemGroups,
            [ImportMany] CommandBarItemDefinition[] menuItems,
            [ImportMany] ExcludeCommandBarElementDefinition[] excludedItems)
        {
            ItemGroupDefinitions = new ObservableCollection<CommandBarGroupDefinition>(menuItemGroups.OrderBy(x => x.SortOrder));
            ItemDefinitions = new ObservableCollection<CommandBarItemDefinition>(menuItems.OrderBy(x => x.SortOrder));
            ExcludedItemDefinitions = new ObservableCollection<CommandBarDefinitionBase>();
            foreach (var item in excludedItems)
                ExcludedItemDefinitions.Add(item.ExcludedCommandBarDefinition);

            foreach (var itemDefinition in ItemDefinitions)
            {
                var group = ItemGroupDefinitions.FirstOrDefault(x => x == itemDefinition.Group);
                group?.Items.AddSorted(itemDefinition, new SortOrderComparer<CommandBarDefinitionBase>());
            }

            ItemDefinitions.CollectionChanged += ItemDefinitions_CollectionChanged;
            ItemGroupDefinitions.CollectionChanged += ItemGroupDefinitions_CollectionChanged;
        }

        private void ItemGroupDefinitions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (var item in e.NewItems)
                    if (item is CommandBarGroupDefinition groupDefinition)
                    {
                        var items = ItemDefinitions.Where(x => x.Group == groupDefinition);
                        foreach (var itemDefinition in items)
                            if (!groupDefinition.Items.Contains(itemDefinition))
                                groupDefinition.Items.AddSorted(itemDefinition, new SortOrderComparer<CommandBarDefinitionBase>());
                    }

            if (e.OldItems != null)
                foreach (var item in e.OldItems)
                    if (item is CommandBarGroupDefinition groupDefinition)
                        groupDefinition.Items.Clear();
        }

        private void ItemDefinitions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (var item in e.NewItems)
                    if (item is CommandBarItemDefinition itemDefinition)
                        if (!itemDefinition.Group.Items.Contains(itemDefinition))
                            itemDefinition.Group.Items.AddSorted(itemDefinition, new SortOrderComparer<CommandBarDefinitionBase>());
            if (e.OldItems != null)
                foreach (var item in e.OldItems)
                    if (item is CommandBarItemDefinition itemDefinition)
                        itemDefinition.Group.Items.Remove(itemDefinition);
        }
    }
}