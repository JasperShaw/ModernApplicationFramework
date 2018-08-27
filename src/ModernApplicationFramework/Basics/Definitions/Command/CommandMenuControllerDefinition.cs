using ModernApplicationFramework.Basics.Definitions.CommandBar.Models;

namespace ModernApplicationFramework.Basics.Definitions.Command
{
    /// <inheritdoc />
    /// <summary>
    ///     Special <see cref="CommandDefinition" /> for menu controller commands
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Basics.Definitions.Command.CommandDefinitionBase" />
    public abstract class CommandMenuControllerDefinition : CommandDefinitionBase
    {
        public override CommandControlTypes ControlType => CommandControlTypes.MenuController;

        public abstract MenuControllerModel Model { get; }
    }
}