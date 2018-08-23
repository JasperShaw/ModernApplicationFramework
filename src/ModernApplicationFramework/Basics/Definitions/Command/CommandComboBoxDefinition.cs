using ModernApplicationFramework.Controls.ComboBox;

namespace ModernApplicationFramework.Basics.Definitions.Command
{
    /// <inheritdoc />
    /// <summary>
    ///  Special <see cref="T:ModernApplicationFramework.Basics.Definitions.Command.CommandDefinition" /> for combo box commands
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Basics.Definitions.Command.CommandDefinitionBase" />
    public abstract class CommandComboBoxDefinition : CommandDefinitionBase
    {
        public override bool IsList => false;
        public override CommandControlTypes ControlType => CommandControlTypes.Combobox;

        public virtual ComboBoxModel Model => ComboBoxModel.Instance;
    }
}
