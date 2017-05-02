using ModernApplicationFramework.Basics.Definitions.Command;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    public sealed class CommandBarComboItemDefinition<T> : CommandBarItemDefinition<T> where T : DefinitionBase
	{
        private ComboBoxDataSource _dataSource;
        private ComboBoxVisualSource _visualSource;

        public ComboBoxDataSource DataSource
        {
            get => _dataSource;
            set
            {
                if (Equals(value, _dataSource)) return;
                _dataSource = value;
                OnPropertyChanged();
            }
        }

        public ComboBoxVisualSource VisualSource
        {
            get => _visualSource;
            set
            {
                if (Equals(value, _visualSource)) return;
                _visualSource = value;
                OnPropertyChanged();
            }
        }

        public CommandBarComboItemDefinition(CommandBarGroupDefinition group, uint sortOrder, bool isEditable, bool stretchHorizontally, bool showText,
            bool isVisible = true, bool isChecked = false, bool isCustom = false, bool isCustomizable = true)
            : base(null, sortOrder, group, null, isVisible, isChecked, isCustom, isCustomizable)
        {
            Flags.PictAndText = showText;

            VisualSource = new ComboBoxVisualSource();
            VisualSource.Flags.StretchHorizontally = stretchHorizontally;
            VisualSource.IsEditable = isEditable;

            if (CommandDefinition is CommandComboBoxDefinition comboBoxDefinition)
                DataSource = comboBoxDefinition.DataSource;
        }
    }
}