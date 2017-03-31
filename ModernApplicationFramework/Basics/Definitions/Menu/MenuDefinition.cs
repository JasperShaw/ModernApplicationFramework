using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Basics.Definitions.Menu
{
    public class MenuDefinition : CommandBarDefinitionBase
    {
        private uint _sortOrder;
        private string _text;
        private string _displayName;

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

        public string DisplayName
        {
            get => _displayName;
            set
            {
                if (value == _displayName) return;
                _displayName = value;
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
        public sealed override bool IsChecked { get; set; }
        public override DefinitionBase CommandDefinition { get; }


        public MenuDefinition(uint sortOrder, string text, string displayName, bool isCustom = false)
        {
            _sortOrder = sortOrder;
            _text = text;
            _displayName = displayName;
            IsCustom = isCustom;
            CommandDefinition = new MenuItemCommandDefinition();
        }
    }
}