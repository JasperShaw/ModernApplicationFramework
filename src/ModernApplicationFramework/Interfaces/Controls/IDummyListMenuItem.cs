using ModernApplicationFramework.Basics.CommandBar.DataSources;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Interfaces.Controls
{
    /// <summary>
    /// A DummyListMenuItem updates its Content when opened.
    /// </summary>
    public interface IDummyListMenuItem
    {
        /// <summary>
        /// Containes the CommandBarDefinition of the MenuItem
        /// </summary>
        CommandBarItemDataSource CommandBarItemDefinition { get; }

        /// <summary>
        /// Updates Menu
        /// </summary>
        /// <param name="commandHandler"></param>
        void Update(CommandHandlerWrapper commandHandler);
    }
}
