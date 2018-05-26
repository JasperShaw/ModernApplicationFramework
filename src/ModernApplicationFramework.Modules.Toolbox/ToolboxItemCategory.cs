using System;
using System.Collections.ObjectModel;

namespace ModernApplicationFramework.Modules.Toolbox
{
    public class ToolboxItemCategory : ToolboxNodeItem
    {
        public Type TargetType { get; }

        private ObservableCollection<ToolboxItem> _items;

        public ObservableCollection<ToolboxItem> Items
        {
            get => _items;
            set
            {
                if (Equals(value, _items)) return;
                _items = value;
                OnPropertyChanged();
            }
        }

        public ToolboxItemCategory(Type targetType, string name) : base(name)
        {
            TargetType = targetType;
            Items = new ObservableCollection<ToolboxItem>();
            Items.CollectionChanged += Items_CollectionChanged;
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (ToolboxItem item in e.OldItems)
                    item.Parent = null;
            }
            if (e.NewItems != null)
            {
                foreach (ToolboxItem item in e.NewItems)
                    item.Parent = this;
            }
        }
    }
}
