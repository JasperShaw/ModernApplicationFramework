namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    public class CommandBarGroupDefinition : CommandBarDefinitionBase
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

        public CommandBarGroupDefinition(CommandBarDefinitionBase parent, uint sortOrder) 
            : base(null, sortOrder, null, false, false, false)
        {
            _parent = parent;
        }
    }
}
