using System.Windows;
using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Modules.Toolbox.Interfaces.Commands
{
    /// <summary>
    ///     Command to create a toolbox item to the current state depending with relative position to the selected node
    /// </summary>
    /// <remarks>
    ///     Command parameter is an <see cref="IDataObject" />
    /// </remarks>
    /// <seealso cref="ICommandDefinitionCommand" />
    public interface IAddItemToSelectedNodeCommand : ICommandDefinitionCommand
    {
    }
}