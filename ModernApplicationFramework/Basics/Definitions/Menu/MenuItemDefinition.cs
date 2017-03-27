using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Interfaces.Command;

namespace ModernApplicationFramework.Basics.Definitions.Menu
{
    public class MenuItemDefinition : CommandBarItemDefinitionBase
    {
        private uint _sortOrder;
        private string _text;
        private string _displayName;
        private DefinitionBase _commandDefinition;
        private MenuItemGroupDefinition _group;

        public override uint SortOrder
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

        public virtual string DisplayName
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

        public MenuItemDefinition(MenuItemGroupDefinition group, uint sortOrder)
        {
            _group = group;
            _sortOrder = sortOrder;
        }
    }

    public sealed class CommandMenuItemDefinition<T> : MenuItemDefinition where T : DefinitionBase
    {
        public CommandMenuItemDefinition(MenuItemGroupDefinition group, uint sortOrder) : base(group, sortOrder)
        {
            CommandDefinition = IoC.Get<ICommandService>().GetCommandDefinition(typeof(T));
        }
    }
}