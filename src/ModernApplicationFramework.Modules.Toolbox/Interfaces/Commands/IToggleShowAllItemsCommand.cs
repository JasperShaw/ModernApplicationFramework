using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Modules.Toolbox.Interfaces.Commands
{
    /// <summary>
    ///     Command enable visibility to all toobox nodes in the current state
    /// </summary>
    /// <remarks>
    ///     Command parameter is not used
    /// </remarks>
    /// <seealso cref="ICommandDefinitionCommand" />
    public interface IToggleShowAllItemsCommand : ICommandDefinitionCommand
    {
    }
}