﻿using System;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Services;
using ModernApplicationFramework.Modules.Toolbox.State;

namespace ModernApplicationFramework.Modules.Toolbox.Items
{
    public class ToolboxItemCategory : ToolboxNodeItem, IToolboxCategory
    {
        public static Guid DefaultCategoryId = new Guid("{41047F4D-A2CF-412F-B216-A8B1E3C08F36}");
        public static Guid CustomCategoryGuid = Guid.Empty;

        internal static IToolboxCategory DefaultCategory = new ToolboxItemCategory(new Guid("{41047F4D-A2CF-412F-B216-A8B1E3C08F36}"), typeof(object), "Default");

        public Type TargetType { get; }

        private IObservableCollection<IToolboxItem> _items;
        private bool _hasItems;
        private bool _hasVisibleItems;
        private Type _currentType;
        private bool _currentVisibleStatus;

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
            protected set
            {
                if (value == _hasItems) return;
                _hasItems = value;
                OnPropertyChanged();
            }
        }

        public bool HasVisibleItems
        {
            get => _hasVisibleItems;
            protected set
            {
                if (value == _hasVisibleItems) return;
                _hasVisibleItems = value;
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

        public ToolboxItemCategory() : this(string.Empty)
        {
            IsNewlyCreated = true;
            EnterRenameMode();
        }


        public ToolboxItemCategory(string name) : this(DefaultCategoryId, typeof(object), name, true)
        {

        }

        public ToolboxItemCategory(Guid id, Type targetType, string name, bool isCustom = false) : base(id, name, isCustom)
        {
            TargetType = targetType;
            Items = new BindableCollection<IToolboxItem>();
            Items.CollectionChanged += Items_CollectionChanged;
        }

        public void Refresh(Type targetType, bool forceVisible = false)
        {
            _currentType = targetType;
            _currentVisibleStatus = forceVisible;
            HasVisibleItems = false;
            foreach (var item in Items)
            {
                var flag = item.EvaluateEnabled(targetType);
                item.IsEnabled = flag;
                item.IsVisible = forceVisible || flag;

                if (item.IsVisible)
                    HasVisibleItems = true;
            }
        }

        public bool RemoveItem(IToolboxItem item)
        {
            if (!Items.Contains(item))
                return false;
            Items.Remove(item);
            IoC.Get<ToolboxItemHost>().DeleteNode(item);
            return true;
        }

        public override bool IsRenameValid(out string errorMessage)
        {
            if (!base.IsRenameValid(out errorMessage))
                return false;
            if (EditingName == Name)
                return true;
            if (!ToolboxService.Instance.GetAllToolboxCategoryNames().Contains(EditingName))
                return true;
            errorMessage = "Already exists";
            return false;
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
                {
                    item.Parent = this;

                    var flag2 = item.EvaluateEnabled(_currentType);
                    item.IsEnabled = flag2;
                    item.IsVisible = _currentVisibleStatus || flag2;
                }
            }
            HasItems = Items.Any();
            InternalRefreshVisibleItems();
        }

        private void InternalRefreshVisibleItems()
        {
            HasVisibleItems = false;
            if (!HasItems)
                return;
            if (Items.Any(x => x.IsVisible))
                HasVisibleItems = true;
        }
    }
}
