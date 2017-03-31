using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Basics.Definitions.Menu.MenuItems
{
    public class MenuItemDefinition : CommandBarItemDefinition
    {
        private uint _sortOrder;
        private string _text;
        private string _displayName;
        private MenuItemGroupDefinition _group;
        private bool _isChecked;

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

        public override bool IsCustom { get; }

        public override bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (value == _isChecked) return;
                _isChecked = value;
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

        public override DefinitionBase CommandDefinition { get; }

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

        public MenuItemDefinition(DefinitionBase commandDefinition = null, bool isCustom = false)
        {
            CommandDefinition = commandDefinition ?? new MenuItemCommandDefinition();
            IsCustom = isCustom;
        }

        public MenuItemDefinition(MenuItemGroupDefinition group, uint sortOrder, DefinitionBase commandDefinition = null, bool isCustom = false) 
            : this (commandDefinition, isCustom)
        {
            _group = group;
            _sortOrder = sortOrder;
        }

        public MenuItemDefinition(string text, MenuItemGroupDefinition group, uint sortOrder, DefinitionBase commandDefinition = null,  bool isCustom = false)
            : this(group, sortOrder, commandDefinition, isCustom)
        {
            _displayName = text;
        } 
    }
}