using System;
using Caliburn.Micro;

namespace ModernApplicationFramework.Modules.Toolbox
{
    public class ToolboxItemCategory : ToolboxNodeItem
    {
        internal static ToolboxItemCategory DefaultCategory = new ToolboxItemCategory(null, "Default");

        public Type TargetType { get; }

        private IObservableCollection<ToolboxItem> _items;

        public IObservableCollection<ToolboxItem> Items
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
            Items = new BindableCollection<ToolboxItem>();
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
