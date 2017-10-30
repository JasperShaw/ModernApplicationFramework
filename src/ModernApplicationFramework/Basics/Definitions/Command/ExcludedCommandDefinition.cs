namespace ModernApplicationFramework.Basics.Definitions.Command
{
    /// <summary>
    /// Container that contains a <see cref="CommandDefinitionBase"/> that should be ignored by the application
    /// </summary>
    public class ExcludedCommandDefinition
    {
        /// <summary>
        /// The excluded definition
        /// </summary>
        public CommandDefinitionBase ExcludedDefinition { get; }

        public ExcludedCommandDefinition(CommandDefinitionBase excludedDefinition)
        {
            ExcludedDefinition = excludedDefinition;
        }
    }
}