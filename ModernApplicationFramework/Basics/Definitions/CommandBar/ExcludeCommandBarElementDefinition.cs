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
        public DefinitionBase ExcludedDefinition { get; }

        public ExcludedCommandDefinition(DefinitionBase excludedDefinition)
        {
            ExcludedDefinition = excludedDefinition;
        }
    }
}