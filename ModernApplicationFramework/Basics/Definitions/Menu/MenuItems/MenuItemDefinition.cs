using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Basics.Definitions.Menu.MenuItems
{
    public class MenuItemDefinition : CommandBarItemDefinition
    {
        private string _displayName;
        private MenuItemGroupDefinition _group;
        private string _text;


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

        public MenuItemDefinition(string text, uint sortOrder, MenuItemGroupDefinition group, DefinitionBase definition, bool visible, bool isChecked,
            bool isCustom)
            : base(text, sortOrder, definition, visible, isChecked, isCustom)
        {
            _displayName = text;
            _group = group;

            if (!string.IsNullOrEmpty(group?.Parent.Text))
                _text = group.Parent.Text + " | " + text;
            else
                _text = text;
        }
    }
}