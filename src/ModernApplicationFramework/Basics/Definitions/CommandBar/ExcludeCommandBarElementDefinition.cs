namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    /// <summary>
    /// Container that contains a <see cref="CommandBarDataSource"/> that should be ignored by the application
    /// </summary>
    public class ExcludeCommandBarElementDefinition
    {
        /// <summary>
        /// The excluded definition
        /// </summary>
        public CommandBarDataSource ExcludedCommandBarDefinition { get; }

        public ExcludeCommandBarElementDefinition(CommandBarDataSource definition)
        {
            ExcludedCommandBarDefinition = definition;
        }
    }
}