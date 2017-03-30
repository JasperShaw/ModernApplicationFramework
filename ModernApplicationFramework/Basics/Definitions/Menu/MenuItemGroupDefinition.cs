using System.ComponentModel;
using System.Runtime.CompilerServices;
using ModernApplicationFramework.Annotations;
using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Basics.Definitions.Menu
{
    public class MenuItemGroupDefinition : INotifyPropertyChanged
    {
        private CommandBarDefinitionBase _parent;
        private uint _sortOrder;

        public event PropertyChangedEventHandler PropertyChanged;

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

        public uint SortOrder
        {
            get => _sortOrder;
            set
            {
                if (value == _sortOrder) return;
                _sortOrder = value;
                OnPropertyChanged();
            }
        }

        public MenuItemGroupDefinition(CommandBarDefinitionBase parent, uint sortOrder)
        {
            _parent = parent;
            _sortOrder = sortOrder;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}