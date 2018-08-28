namespace ModernApplicationFramework.Basics.Definitions.ItemDefinitions
{
    /// <summary>
    /// Container that contains a <see cref="CommandBarItemDefinition"/> that should be ignored by the application
    /// </summary>
    public class ExcludedItemDefinition
    {
        /// <summary>
        /// The excluded definition
        /// </summary>
        public CommandBarItemDefinition ExcludedDefinition { get; }

        public ExcludedItemDefinition(CommandBarItemDefinition excludedDefinition)
        {
            ExcludedDefinition = excludedDefinition;
        }
    }
}