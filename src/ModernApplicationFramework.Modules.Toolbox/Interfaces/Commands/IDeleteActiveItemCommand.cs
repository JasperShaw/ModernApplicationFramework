using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Modules.Toolbox.Interfaces.Commands
{
    /// <summary>
    ///     Command to remove a toolbox item from current state
    /// </summary>
    /// <remarks>
    ///     Command parameter is the item to remove
    /// </remarks>
    /// <seealso cref="ICommandDefinitionCommand" />
    internal interface IDeleteActiveItemCommand : ICommandDefinitionCommand
    {
    }
}