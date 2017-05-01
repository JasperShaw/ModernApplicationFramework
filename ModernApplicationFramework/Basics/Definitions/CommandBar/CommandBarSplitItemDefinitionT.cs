using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Interfaces.Command;
using ModernApplicationFramework.Interfaces.Utilities;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    public sealed class CommandBarSplitItemDefinitionT<T> : CommandBarItemDefinition where T : DefinitionBase
    {
        private int _selectedIndex;
        private string _statusString;
        public override DefinitionBase CommandDefinition { get; }

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

        public CommandBarSplitItemDefinitionT(string statusString, CommandBarGroupDefinition group, uint sortOrder,
            bool isVisible = true, bool isChecked = false, bool isCustom = false, bool isCustomizable = true)
            : base(null, sortOrder, group, null, isVisible, isChecked, isCustom, isCustomizable)
        {
            CommandDefinition = IoC.Get<ICommandService>().GetCommandDefinition(typeof(T));
            Text = CommandDefinition.Text;
            _statusString = statusString;
        }

        public CommandBarSplitItemDefinitionT(IStatusStringCreator statusStringCreator, CommandBarGroupDefinition group, uint sortOrder,
            bool isVisible = true, bool isChecked = false, bool isCustom = false, bool isCustomizable = true)
            : base(null, sortOrder, group, null, isVisible, isChecked, isCustom, isCustomizable)
        {
            CommandDefinition = IoC.Get<ICommandService>().GetCommandDefinition(typeof(T));
            Text = CommandDefinition.Text;
            StringCreator = statusStringCreator;
            _statusString = StringCreator.CreateMessage(1);
        }
    }
}