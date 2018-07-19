using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Modules.Toolbox.Interfaces.Commands
{
    /// <summary>
    ///     Command to copy the data of a toolbox item to the system clipboard
    /// </summary>
    /// <remarks>
    ///     Command parameter is <see cref="bool" /> to determinate whether the action shall be executed as copy or cut.
    ///     <see langword="true" /> performes a cut logic
    /// </remarks>
    /// <seealso cref="ICommandDefinitionCommand" />
    public interface ICopySelectedItemCommand : ICommandDefinitionCommand
    {
    }
}