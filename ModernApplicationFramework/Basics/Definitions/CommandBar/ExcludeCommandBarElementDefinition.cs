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
}