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
	    public override CommandDefinitionBase CommandDefinition { get; }

        public CommandBarComboItemDefinition(Guid id, CommandBarGroupDefinition group, uint sortOrder, bool isEditable, bool stretchHorizontally,
            bool isVisible = true, bool isChecked = false, bool isCustom = false, bool isCustomizable = true, CommandBarFlags flags = CommandBarFlags.CommandFlagPictAndText)
            : base(id, null, sortOrder, group, null, isVisible, isChecked, isCustom, isCustomizable, flags)
        {
            CommandDefinition = IoC.Get<ICommandService>().GetCommandDefinition(typeof(T));

            VisualSource.Flags.StretchHorizontally = stretchHorizontally;
            VisualSource.IsEditable = isEditable;

            if (CommandDefinition is CommandComboBoxDefinition comboBoxDefinition)
                DataSource = comboBoxDefinition.DataSource;
        }
	}

    public class CommandBarComboItemDefinition : CommandBarItemDefinition
    {
        private ComboBoxDataSource _dataSource;
        private ComboBoxVisualSource _visualSource;

        public override Guid Id { get; }

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

        internal CommandBarComboItemDefinition(Guid id, string text, uint sortOrder, CommandBarGroupDefinition group, CommandDefinitionBase definition,
            bool visible = true, bool isChecked = false, bool isCustom = false, bool isCustomizable = true, CommandBarFlags flags = CommandBarFlags.CommandFlagNone) 
            : base(text, sortOrder, group, definition, visible, isChecked, isCustom, isCustomizable, flags)
        {
            Id = id;
            VisualSource = new ComboBoxVisualSource();
            if (definition is CommandComboBoxDefinition comboBoxDefinition)
                DataSource = comboBoxDefinition.DataSource;
        }
    }
}