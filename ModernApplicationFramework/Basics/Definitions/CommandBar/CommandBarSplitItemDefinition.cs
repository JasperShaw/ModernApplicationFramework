using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Interfaces.Utilities;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    /// <inheritdoc />
    /// <summary>
    /// Command bar split button definition that contains a <see cref="CommandBarDefinitionBase"/>
    /// </summary>
    /// <typeparam name="T">The type of the command definition this item should have</typeparam>
    /// <seealso cref="T:ModernApplicationFramework.Basics.Definitions.CommandBar.CommandBarItemDefinition`1" />
    public sealed class CommandBarSplitItemDefinition<T> : CommandBarItemDefinition<T> where T : CommandDefinitionBase
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


        private CommandBarSplitItemDefinition(CommandBarGroupDefinition group, uint sortOrder,
            bool isVisible = true, bool isChecked = false, bool isCustom = false, bool isCustomizable = true)
            : base(null, sortOrder, group, null, isVisible, isChecked, isCustom, isCustomizable)
        {
        }

        public CommandBarSplitItemDefinition(string statusString, CommandBarGroupDefinition group, uint sortOrder,
            bool isVisible = true, bool isChecked = false, bool isCustom = false, bool isCustomizable = true)
            : this(group, sortOrder, isVisible, isChecked, isCustom, isCustomizable)
        {
            _statusString = statusString;
        }

        public CommandBarSplitItemDefinition(IStatusStringCreator statusStringCreator, CommandBarGroupDefinition group, uint sortOrder,
            bool isVisible = true, bool isChecked = false, bool isCustom = false, bool isCustomizable = true)
            : this(group, sortOrder, isVisible, isChecked, isCustom, isCustomizable)
        {
            StringCreator = statusStringCreator;
            _statusString = StringCreator.CreateMessage(1);
        }
    }
}