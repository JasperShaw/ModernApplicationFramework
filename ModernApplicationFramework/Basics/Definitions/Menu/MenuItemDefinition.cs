using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Interfaces.Command;

namespace ModernApplicationFramework.Basics.Definitions.Menu
{
    public class MenuItemDefinition : MenuDefinitionBase
    {
        private int _sortOrder;
        private string _text;
        private string _displayName;
        private DefinitionBase _commandDefinition;
        private MenuItemGroupDefinition _group;

        public override int SortOrder
        {
            get => _sortOrder;
            set
            {
                if (value == _sortOrder) return;
                _sortOrder = value;
                OnPropertyChanged();
            }
        }

        public override string Text
        {
            get => _text;
            set
            {
                if (value == _text) return;
                _text = value;
                OnPropertyChanged();
            }
        }

        public override string DisplayName
        {
            get => _displayName;
            set
            {
                if (value == _displayName) return;
                _displayName = value;
                OnPropertyChanged();
            }
        }

        public override DefinitionBase CommandDefinition
        {
            get => _commandDefinition;
            set
            {
                if (Equals(value, _commandDefinition)) return;
                _commandDefinition = value;
                OnPropertyChanged();
            }
        }

        public MenuItemGroupDefinition Group
        {
            get => _group;
            set
            {
                if (Equals(value, _group)) return;
                _group = value;
                OnPropertyChanged();
            }
        }

        public MenuItemDefinition(MenuItemGroupDefinition group, int sortOrder)
        {
            _group = group;
            _sortOrder = sortOrder;
        }
    }

    public sealed class CommandMenuItemDefinition<T> : MenuItemDefinition where T : DefinitionBase
    {
        public CommandMenuItemDefinition(MenuItemGroupDefinition group, int sortOrder) : base(group, sortOrder)
        {
            CommandDefinition = IoC.Get<ICommandService>().GetCommandDefinition(typeof(T));
        }
    }
}