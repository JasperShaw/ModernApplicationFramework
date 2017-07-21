using ModernApplicationFramework.Basics.Definitions.Command;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    public class ExcludeCommandBarElementDefinition
    {
        public CommandBarDefinitionBase ExcludedCommandBarDefinition { get; }

        public ExcludeCommandBarElementDefinition(CommandBarDefinitionBase definition)
        {
            ExcludedCommandBarDefinition = definition;
        }
    }

    public class ExcludedCommandDefinition
    {
        public CommandDefinitionBase ExcludedDefinition { get; }

        public ExcludedCommandDefinition(CommandDefinitionBase excludedDefinition)
        {
            ExcludedDefinition = excludedDefinition;
        }
    }
}