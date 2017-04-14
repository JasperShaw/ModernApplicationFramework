using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Basics.Definitions.Menu
{
    public sealed class MenuBarDefinition : CommandBarDefinitionBase, IHasInternalName
    {
        private string _internalName;

        public string InternalName
        {
            get => _internalName;
            set
            {
                if (value == _internalName) return;
                _internalName = value;
                OnPropertyChanged();
            }
        }

        public MenuBarDefinition(string text, uint sortOrder) : base(text, sortOrder, null, false, false, false)
        {
            _internalName = text;
        }
    }
}
