using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Modules.Toolbox.Interfaces.Commands
{
    /// <summary>
    ///     Command to remove to selected toolbox category
    /// </summary>
    /// <remarks>
    ///     Command parameter is not used
    /// </remarks>
    /// <seealso cref="ICommandDefinitionCommand" />
    public interface IDeleteActiveToolbarCategoryCommand : ICommandDefinitionCommand
    {
    }
}