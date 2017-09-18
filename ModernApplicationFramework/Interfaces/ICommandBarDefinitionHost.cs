using System.Collections.ObjectModel;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Interfaces
{
    /// <summary>
    /// A collection container holding all commandbar definitions and exclusions
    /// </summary>
    public interface ICommandBarDefinitionHost
    {
        /// <summary>
        /// Collection of all group definitions
        /// </summary>
        ObservableCollection<CommandBarGroupDefinition> ItemGroupDefinitions { get; }

        /// <summary>
        /// Collection of all commandbar item definitions
        /// </summary>
        ObservableCollection<CommandBarItemDefinition> ItemDefinitions { get; }

        /// <summary>
        /// Collection of excluded commandbar item definitions
        /// </summary>
        ObservableCollection<CommandBarDefinitionBase> ExcludedItemDefinitions { get; }

        /// <summary>
        /// Collection of excluded command definitions
        /// </summary>
        ObservableCollection<CommandDefinitionBase> ExcludedCommandDefinitions { get; }
    }
}
