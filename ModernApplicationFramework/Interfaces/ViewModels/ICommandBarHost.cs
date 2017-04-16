using System.Collections.Generic;
using System.Collections.ObjectModel;
using ModernApplicationFramework.Basics.CommandBar.Hosts;
using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Interfaces.ViewModels
{
    public interface ICommandBarHost
    {
        ICommandBarDefinitionHost DefinitionHost { get; }

        ObservableCollection<CommandBarDefinitionBase> TopLevelDefinitions { get; }

        void Build();
        void AddItemDefinition(CommandBarItemDefinition definition, CommandBarDefinitionBase parent, bool addAboveSeparator);

        void DeleteItemDefinition(CommandBarItemDefinition definition, CommandBarDefinitionBase parent); //, bool addAboveSeparator);
        CommandBarItemDefinition GetPreviousItem(CommandBarItemDefinition definition, CommandBarDefinitionBase parent);

        CommandBarItemDefinition GetNextItem(CommandBarItemDefinition definition, CommandBarDefinitionBase parent);

        CommandBarItemDefinition GetNextItemInGroup(CommandBarItemDefinition definition);

        CommandBarItemDefinition GetPreviousItemInGroup(CommandBarItemDefinition definition);

        void DeleteGroup(CommandBarGroupDefinition group, CommandBarDefinitionBase parent, AppendTo option);

        IEnumerable<CommandBarDefinitionBase> GetMenuHeaderItemDefinitions();
    }
}
