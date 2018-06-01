using System;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Extended.Layout;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;

namespace ModernApplicationFramework.Modules.Toolbox
{
    public class ToolboxItemCategory : ToolboxNodeItem, IToolboxCategory
    {
        public static Guid DefaultCategoryId = new Guid("{41047F4D-A2CF-412F-B216-A8B1E3C08F36}");
        internal static IToolboxCategory DefaultCategory = new ToolboxItemCategory(new Guid("{41047F4D-A2CF-412F-B216-A8B1E3C08F36}"), null, "Default");

        public Type TargetType { get; }

        private IObservableCollection<IToolboxItem> _items;
        private bool _hasItems;

        public static bool IsDefaultCategory(IToolboxCategory category)
        {
            return category.Id.Equals(DefaultCategoryId);
        }

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

        static ToolboxItemCategory()
        {
            var i = new ToolboxItem(Guid.NewGuid(), "Test", typeof(int), DefaultCategory, new[] { typeof(ILayoutItem) });
            var j = new ToolboxItem(Guid.NewGuid(), "String", typeof(string), DefaultCategory, new[] { typeof(object) });
            DefaultCategory.Items.Add(i);
            DefaultCategory.Items.Add(j);
        }


        public ToolboxItemCategory(Guid id, Type targetType, string name) : base(id, name)
        {
            TargetType = targetType;
            Items = new BindableCollection<IToolboxItem>();
            Items.CollectionChanged += Items_CollectionChanged;
        }

        public void EvaluateItems(Type targetType)
        {
            foreach (var item in Items)
            {
                item.EvaluateVisibility(targetType);
            }
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

    public class TestClass : LayoutItem
    {

    }
}
