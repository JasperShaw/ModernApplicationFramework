using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Basics.Definitions.Menu
{
    public class MenuItemGroupDefinition : CommandBarDefinitionBase
    {
        private CommandBarDefinitionBase _parent;

        public CommandBarDefinitionBase Parent
        {
            get => _parent;
            set
            {
                if (Equals(value, _parent)) return;
                _parent = value;
                OnPropertyChanged();
            }
        }

        public MenuItemGroupDefinition(CommandBarDefinitionBase parent, uint sortOrder) : base(null, sortOrder, null, false, false)
        {
            _parent = parent;
        }
    }
}