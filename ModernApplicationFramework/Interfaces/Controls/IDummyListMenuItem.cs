using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.CommandBase;

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
        CommandBarDefinitionBase CommandBarItemDefinition { get; }

        /// <summary>
        /// Updates Menu
        /// </summary>
        /// <param name="commandHandler"></param>
        void Update(CommandHandlerWrapper commandHandler);
    }
}
