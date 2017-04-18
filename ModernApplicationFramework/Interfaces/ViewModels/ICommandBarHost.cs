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

        void Build(CommandBarDefinitionBase definition);

        void BuildLogical(CommandBarDefinitionBase definition);

        void AddItemDefinition(CommandBarItemDefinition definition, CommandBarDefinitionBase parent, bool addAboveSeparator);

        void DeleteItemDefinition(CommandBarItemDefinition definition);
        CommandBarItemDefinition GetPreviousItem(CommandBarItemDefinition definition);

        CommandBarItemDefinition GetNextItem(CommandBarItemDefinition definition);

        CommandBarItemDefinition GetNextItemInGroup(CommandBarItemDefinition definition);

        CommandBarItemDefinition GetPreviousItemInGroup(CommandBarItemDefinition definition);

        void DeleteGroup(CommandBarGroupDefinition group, AppendTo option);

        IEnumerable<CommandBarDefinitionBase> GetMenuHeaderItemDefinitions();
        void AddGroupAt(CommandBarItemDefinition startingDefinition);
        void MoveItem(CommandBarItemDefinition selectedListBoxDefinition, int offset, CommandBarDefinitionBase parent);

        CommandBarGroupDefinition GetNextGroup(CommandBarGroupDefinition group);

        CommandBarGroupDefinition GetPreviousGroup(CommandBarGroupDefinition group);
    }
}
