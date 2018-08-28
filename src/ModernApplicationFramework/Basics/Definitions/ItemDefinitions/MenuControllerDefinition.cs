using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.CommandBar.Models;

namespace ModernApplicationFramework.Basics.Definitions.ItemDefinitions
{
    /// <inheritdoc />
    /// <summary>
    ///     Special <see cref="CommandDefinition" /> for menu controller commands
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Basics.Definitions.Command.CommandDefinitionBase" />
    public abstract class MenuControllerDefinition : CommandBarItemDefinition
    {
        public override CommandControlTypes ControlType => CommandControlTypes.MenuController;

        public abstract MenuControllerModel Model { get; }
    }
}