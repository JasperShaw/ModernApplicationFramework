using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
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
            ItemGroupDefinitions = new ObservableCollection<CommandBarGroupDefinition>(menuItemGroups);
            ItemDefinitions = new ObservableCollection<CommandBarItemDefinition>(menuItems);
            ExcludedItemDefinitions = new ObservableCollection<CommandBarDefinitionBase>();
            foreach (var item in excludedItems)
                ExcludedItemDefinitions.Add(item.ExcludedCommandBarDefinition);
        }
    }
}