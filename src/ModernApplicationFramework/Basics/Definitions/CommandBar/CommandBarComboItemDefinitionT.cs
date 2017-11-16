using System;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Controls.ComboBox;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    /// <inheritdoc />
    /// <summary>
    /// Combo box command bar item definition that contains a <see cref="T:ModernApplicationFramework.Basics.Definitions.CommandBar.CommandBarDefinitionBase" />
    /// </summary>
    /// <typeparam name="T">The type of the command definition this item should have</typeparam>
    /// <seealso cref="T:ModernApplicationFramework.Basics.Definitions.CommandBar.CommandBarItemDefinition`1" />
    public sealed class CommandBarComboItemDefinition<T> : CommandBarComboItemDefinition where T : CommandDefinitionBase
	{
        public override Guid Id { get; }

	    public override CommandDefinitionBase CommandDefinition { get; }

        public CommandBarComboItemDefinition(Guid id, CommandBarGroupDefinition group, uint sortOrder, bool isEditable, bool stretchHorizontally, bool showText,
            bool isVisible = true, bool isChecked = false, bool isCustom = false, bool isCustomizable = true)
            : base(null, sortOrder, group, null, isVisible, isChecked, isCustom, isCustomizable)
        {
            Id = id;
            Flags.PictAndText = showText;

            CommandDefinition = IoC.Get<ICommandService>().GetCommandDefinition(typeof(T));

            VisualSource = new ComboBoxVisualSource();
            VisualSource.Flags.StretchHorizontally = stretchHorizontally;
            VisualSource.IsEditable = isEditable;

            if (CommandDefinition is CommandComboBoxDefinition comboBoxDefinition)
                DataSource = comboBoxDefinition.DataSource;
        }
	}

    public abstract class CommandBarComboItemDefinition : CommandBarItemDefinition
    {
        private ComboBoxDataSource _dataSource;
        private ComboBoxVisualSource _visualSource;

        /// <summary>
        /// The <see cref="ComboBoxDataSource"/> of the combo box item
        /// </summary>
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

        /// <summary>
        /// The <see cref="ComboBoxVisualSource"/> of the combo box item
        /// </summary>
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

        protected CommandBarComboItemDefinition(string text, uint sortOrder, CommandBarGroupDefinition group, CommandDefinitionBase definition, bool visible, bool isChecked, bool isCustom, bool isCustomizable) 
            : base(text, sortOrder, @group, definition, visible, isChecked, isCustom, isCustomizable)
        {
        }
    }
}