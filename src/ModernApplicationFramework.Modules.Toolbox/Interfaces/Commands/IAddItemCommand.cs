using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Modules.Toolbox.Interfaces.Commands
{
    /// <summary>
    ///     Command to add a new Toolbox items from the selection dialog window
    /// </summary>
    /// <remarks>
    ///     Command parameter is not used
    /// </remarks>
    /// <seealso cref="ICommandDefinitionCommand" />
    public interface IAddItemCommand : ICommandDefinitionCommand
    {
    }
}