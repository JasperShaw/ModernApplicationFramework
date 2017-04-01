using ModernApplicationFramework.Basics.Definitions.Command;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    public abstract class CommandBarItemDefinition : CommandBarDefinitionBase
    {
        private FlagStorage _flagStorage;
        private bool _isVisible;

        public virtual FlagStorage Flags => _flagStorage ?? (_flagStorage = new FlagStorage());



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

        protected CommandBarItemDefinition(string text, uint sortOrder, DefinitionBase definition, bool visible,
            bool isChecked, bool isCustom)
            : base(text, sortOrder, definition, isCustom, isChecked)
        {
            _isVisible = visible;
        }
    }
}