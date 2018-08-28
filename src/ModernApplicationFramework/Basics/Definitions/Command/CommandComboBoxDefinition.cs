using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.CommandBar.Models;

namespace ModernApplicationFramework.Basics.Definitions.Command
{
    /// <inheritdoc />
    /// <summary>
    ///  Special <see cref="T:ModernApplicationFramework.Basics.Definitions.Command.CommandDefinition" /> for combo box commands
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Basics.Definitions.Command.CommandDefinitionBase" />
    public abstract class CommandComboBoxDefinition : CommandBarItemDefinition
    {
        public override CommandControlTypes ControlType => CommandControlTypes.Combobox;

        public abstract ComboBoxModel Model { get; }
    }
}
