using ModernApplicationFramework.Basics.Definitions.Command;

namespace ModernApplicationFramework.Basics.Definitions.Menu
{
    public class MenuDefinition : MenuDefinitionBase
    {
        private int _sortOrder;
        private string _text;
        private string _displayName;
        private DefinitionBase _commandDefinition;

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

        public MenuDefinition(int sortOrder, string text, string displayName)
        {
            _sortOrder = sortOrder;
            _text = text;
            _displayName = displayName;
        }
    }
}