using System.Windows.Controls;
using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Basics.Definitions.Toolbar
{
    public class ToolbarDefinition : CommandBarDefinitionBase
    {
        private uint _sortOrder;
        private string _text;
        private Dock _position;
        private bool _isVisible;

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

        public Dock Position
        {
            get => _position;
            set
            {
                if (value == _position) return;
                _position = value;
                OnPropertyChanged();
            }
        }

        public bool IsCustom { get; }

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (value == _isVisible) return;
                _isVisible = value;
                OnPropertyChanged();
            }
        }

        public ToolbarDefinition(string text, uint sortOrder, bool visible, Dock position, bool isCustom = false)
        {
            _text = text;
            _sortOrder = sortOrder;
            _position = position;
            IsCustom = isCustom;
            _isVisible = visible;
        }
    }
}
