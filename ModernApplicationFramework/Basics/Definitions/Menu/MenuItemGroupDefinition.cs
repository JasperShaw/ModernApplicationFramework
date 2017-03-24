using System.ComponentModel;
using System.Runtime.CompilerServices;
using ModernApplicationFramework.Annotations;

namespace ModernApplicationFramework.Basics.Definitions.Menu
{
    public class MenuItemGroupDefinition : INotifyPropertyChanged
    {
        private MenuDefinitionBase _parent;
        private int _sortOrder;

        public event PropertyChangedEventHandler PropertyChanged;

        public MenuDefinitionBase Parent
        {
            get => _parent;
            set
            {
                if (Equals(value, _parent)) return;
                _parent = value;
                OnPropertyChanged();
            }
        }

        public int SortOrder
        {
            get => _sortOrder;
            set
            {
                if (value == _sortOrder) return;
                _sortOrder = value;
                OnPropertyChanged();
            }
        }

        public MenuItemGroupDefinition(MenuDefinitionBase parent, int sortOrder)
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