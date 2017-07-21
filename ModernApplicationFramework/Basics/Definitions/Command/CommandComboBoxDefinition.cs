using ModernApplicationFramework.Controls.ComboBox;

namespace ModernApplicationFramework.Basics.Definitions.Command
{
    /// <inheritdoc />
    /// <summary>
    ///  Special <see cref="T:ModernApplicationFramework.Basics.Definitions.Command.CommandDefinition" /> for combo boxes
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Basics.Definitions.Command.CommandDefinitionBase" />
    public abstract class CommandComboBoxDefinition : CommandDefinitionBase
    {
        public override bool IsList => false;
        public override CommandControlTypes ControlType => CommandControlTypes.Combobox;

        /// <summary>
        /// The data model of the combo box
        /// </summary>
        public abstract ComboBoxDataSource DataSource { get; }
    }
}
