using ModernApplicationFramework.Controls.ComboBox;

namespace ModernApplicationFramework.Basics.Definitions.Command
{
    public abstract class CommandComboBoxDefinition : DefinitionBase
    {
        public override bool IsList => false;
        public override CommandControlTypes ControlType => CommandControlTypes.Combobox;

        public abstract ComboBoxDataSource DataSource { get; }
    }
}
