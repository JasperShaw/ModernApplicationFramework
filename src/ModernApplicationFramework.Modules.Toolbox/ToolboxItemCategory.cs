using System;
using System.Linq;
using Caliburn.Micro;

namespace ModernApplicationFramework.Modules.Toolbox
{
    public class ToolboxItemCategory : ToolboxNodeItem
    {
        internal static ToolboxItemCategory DefaultCategory = new ToolboxItemCategory(null, "Default");

        public Type TargetType { get; }

        private IObservableCollection<IToolboxItem> _items;
        private bool _hasItems;

        public IObservableCollection<IToolboxItem> Items
        {
            get => _items;
            set
            {
                if (Equals(value, _items)) return;
                _items = value;
                OnPropertyChanged();
            }
        }

        public bool HasItems
        {
            get => _hasItems;
            set
            {
                if (value == _hasItems) return;
                _hasItems = value;
                OnPropertyChanged();
            }
        }

        public ToolboxItemCategory(Type targetType, string name) : base(name)
        {
            TargetType = targetType;
            Items = new BindableCollection<IToolboxItem>();
            Items.CollectionChanged += Items_CollectionChanged;
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (IToolboxItem item in e.OldItems)
                    item.Parent = null;
            }
            if (e.NewItems != null)
            {
                foreach (IToolboxItem item in e.NewItems)
                    item.Parent = this;
            }

            HasItems = Items.Any();
        }
    }
}
