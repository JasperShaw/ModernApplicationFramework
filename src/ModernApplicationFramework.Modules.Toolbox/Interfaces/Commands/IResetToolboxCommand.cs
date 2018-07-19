using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Modules.Toolbox.Interfaces.Commands
{
    /// <summary>
    ///     Command to reset the toolbox state to the last known backup
    /// </summary>
    /// <remarks>
    ///     Command parameter is not used
    /// </remarks>
    /// <seealso cref="ICommandDefinitionCommand" />
    public interface IResetToolboxCommand : ICommandDefinitionCommand
    {
    }
}