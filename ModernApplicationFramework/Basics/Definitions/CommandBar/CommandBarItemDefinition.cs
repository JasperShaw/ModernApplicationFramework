using ModernApplicationFramework.Basics.Definitions.Command;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    public abstract class CommandBarItemDefinition : CommandBarDefinitionBase
    {
        private bool _isVisible;
        private bool _precededBySeparator;

        public virtual bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (value == _isVisible) return;
                _isVisible = value;
                OnPropertyChanged();
            }
        }

        public virtual bool PrecededBySeparator
        {
            get => _precededBySeparator;
            set
            {
                if (value == _precededBySeparator)
                    return;
                _precededBySeparator = value;
                OnPropertyChanged();
            }
        }

        protected CommandBarItemDefinition(string text, uint sortOrder, DefinitionBase definition, bool visible,
            bool isChecked, bool isCustom, bool isCustomizable)
            : base(text, sortOrder, definition, isCustom, isCustomizable, isChecked)
        {
            _isVisible = visible;
        }
    }
}