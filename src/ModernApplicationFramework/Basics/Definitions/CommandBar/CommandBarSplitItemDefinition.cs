using System;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Interfaces.Utilities;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    /// <inheritdoc />
    /// <summary>
    /// Command bar split button definition that contains a <see cref="CommandBarDefinitionBase"/>
    /// </summary>
    /// <typeparam name="T">The type of the command definition this item should have</typeparam>
    /// <seealso cref="T:ModernApplicationFramework.Basics.Definitions.CommandBar.CommandBarItemDefinition`1" />
    public sealed class CommandBarSplitItemDefinition<T> : CommandBarSplitItemDefinition where T : CommandDefinitionBase
	{
	    public override CommandDefinitionBase CommandDefinition { get; }


        public CommandBarSplitItemDefinition(Guid id, string statusString, CommandBarGroupDefinition group, uint sortOrder,
            bool isVisible = true, bool isChecked = false, bool isCustom = false, bool isCustomizable = true)
            : this(id, group, sortOrder, isVisible, isChecked, isCustom, isCustomizable)
        {
            StatusString = statusString;
        }

        public CommandBarSplitItemDefinition(Guid id, CommandBarGroupDefinition group, uint sortOrder,
            bool isVisible = true, bool isChecked = false, bool isCustom = false, bool isCustomizable = true)
            : base(id, null, sortOrder, group, null, isVisible, isChecked, isCustom, isCustomizable)
        {

            CommandDefinition = IoC.Get<ICommandService>().GetCommandDefinition(typeof(T));
            Text = CommandDefinition.Text;

            if (CommandDefinition is CommandSplitButtonDefinition splitButtonDefinition)
                StringCreator = splitButtonDefinition.StatusStringCreator;

            StatusString = StringCreator?.CreateDefaultMessage();
        }
	}

    public class CommandBarSplitItemDefinition : CommandBarItemDefinition
    {
        private int _selectedIndex;
        private string _statusString;

        /// <summary>
        /// The currently selected item index
        /// </summary>
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (value == _selectedIndex)
                    return;
                _selectedIndex = value;
                OnPropertyChanged();
                if (StringCreator != null)
                    StatusString = StringCreator.CreateMessage(value + 1);
            }
        }

        /// <summary>
        /// The message of the status bar of a split button
        /// </summary>
        public string StatusString
        {
            get => _statusString;
            set
            {
                if (value == _statusString) return;
                _statusString = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The <see cref="IStatusStringCreator"/> that creates an localized status message
        /// </summary>
        public IStatusStringCreator StringCreator { get; set; }


        internal CommandBarSplitItemDefinition(Guid id, string text, uint sortOrder, CommandBarGroupDefinition group, CommandDefinitionBase definition, 
            bool visible = true, bool isChecked = false, bool isCustom = false, bool isCustomizable = false) 
            : base(text, sortOrder, group, definition, visible, isChecked, isCustom, isCustomizable)
        {
            Id = id;

            if (definition is CommandSplitButtonDefinition splitButtonDefinition)
                StringCreator = splitButtonDefinition.StatusStringCreator;

            StatusString = StringCreator?.CreateDefaultMessage();
        }

        public override Guid Id { get; }
    }
}