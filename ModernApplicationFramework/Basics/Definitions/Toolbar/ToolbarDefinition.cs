using System.Windows.Controls;
using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Basics.Definitions.Toolbar
{
    public class ToolbarDefinition : CommandBarDefinitionBase
    {
        private Dock _position;
        private bool _isVisible;


        public Dock Position
        {
            get => _position;
            set
            {
                if (value == _position) return;
                LastPosition = _position;
                _position = value;
                OnPropertyChanged();
            }
        }

        public Dock LastPosition { get; private set; }

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

        public ToolbarDefinition(string text, uint sortOrder, bool visible, Dock position,
            bool isCustom = false) : base(text, sortOrder, null, isCustom)
        {
            _position = position;
            _isVisible = visible;
        }
    }
}