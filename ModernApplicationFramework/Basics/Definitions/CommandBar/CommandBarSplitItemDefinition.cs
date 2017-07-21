using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Interfaces.Utilities;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    public sealed class CommandBarSplitItemDefinition<T> : CommandBarItemDefinition<T> where T : CommandDefinitionBase
	{
        private int _selectedIndex;
        private string _statusString;

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