namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    /// <summary>
    /// Container that contains a <see cref="CommandBarDefinitionBase"/> that should be ignored by the application
    /// </summary>
    public class ExcludeCommandBarElementDefinition
    {
        /// <summary>
        /// The excluded definition
        /// </summary>
        public CommandBarDefinitionBase ExcludedCommandBarDefinition { get; }

        public ExcludeCommandBarElementDefinition(CommandBarDefinitionBase definition)
        {
            ExcludedCommandBarDefinition = definition;
        }
    }
}