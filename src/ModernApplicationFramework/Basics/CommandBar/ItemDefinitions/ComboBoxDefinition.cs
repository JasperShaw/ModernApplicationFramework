using ModernApplicationFramework.Basics.CommandBar.Models;

namespace ModernApplicationFramework.Basics.CommandBar.ItemDefinitions
{
    /// <inheritdoc />
    /// <summary>
    ///  Special <see cref="T:ModernApplicationFramework.Basics.Definitions.Command.CommandDefinition" /> for combo box commands
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Basics.Definitions.Command.CommandDefinitionBase" />
    public abstract class ComboBoxDefinition : CommandBarItemDefinition
    {
        public override CommandControlTypes ControlType => CommandControlTypes.Combobox;

        public abstract ComboBoxModel Model { get; }
    }
}
