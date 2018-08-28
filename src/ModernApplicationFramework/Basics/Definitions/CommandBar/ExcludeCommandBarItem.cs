using ModernApplicationFramework.Basics.Definitions.CommandBar.Elements;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    /// <summary>
    /// Container that contains a <see cref="CommandBarDataSource"/> that should be ignored by the application
    /// </summary>
    public class ExcludeCommandBarItem
    {
        /// <summary>
        /// The excluded definition
        /// </summary>
        public CommandBarItem ExcludedItem { get; }

        public ExcludeCommandBarItem(CommandBarItem definition)
        {
            ExcludedItem = definition;
        }
    }
}