using System.Collections.ObjectModel;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Interfaces
{
    public interface ICommandBarDefinitionHost
    {
        ObservableCollection<CommandBarGroupDefinition> ItemGroupDefinitions { get; }
        ObservableCollection<CommandBarItemDefinition> ItemDefinitions { get; }
        ObservableCollection<CommandBarDefinitionBase> ExcludedItemDefinitions { get; }
        ObservableCollection<DefinitionBase> ExcludedCommandDefinitions { get; }
    }
}
