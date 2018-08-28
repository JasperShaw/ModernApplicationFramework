namespace ModernApplicationFramework.Basics.Definitions.Command
{
    /// <summary>
    /// Container that contains a <see cref="CommandDefinitionBase"/> that should be ignored by the application
    /// </summary>
    public class ExcludedItemDefinition
    {
        /// <summary>
        /// The excluded definition
        /// </summary>
        public CommandDefinitionBase ExcludedDefinition { get; }

        public ExcludedItemDefinition(CommandDefinitionBase excludedDefinition)
        {
            ExcludedDefinition = excludedDefinition;
        }
    }
}