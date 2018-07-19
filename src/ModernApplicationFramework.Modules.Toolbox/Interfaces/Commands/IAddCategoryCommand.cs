using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Modules.Toolbox.Interfaces.Commands
{
    /// <summary>
    ///     Command to add a new Toolbox category with empty name
    /// </summary>
    /// <remarks>
    ///     Command parameter is not used
    /// </remarks>
    /// <seealso cref="ICommandDefinitionCommand" />
    public interface IAddCategoryCommand : ICommandDefinitionCommand
    {
    }
}