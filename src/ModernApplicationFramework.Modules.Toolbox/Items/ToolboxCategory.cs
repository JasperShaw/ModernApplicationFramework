using System;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.State;

namespace ModernApplicationFramework.Modules.Toolbox.Items
{
    public class ToolboxCategory : ToolboxNode, IToolboxCategory
    {
        [Export] internal static IToolboxCategory DefaultCategory = new ToolboxCategory(Guids.DefaultCategoryId, "Default");

        private IObservableCollection<IToolboxItem> _items;
        private bool _hasItems;
        private bool _hasVisibleItems;
        private Type _currentType;
        private bool _showAllStatus;
        private bool _hasEnabledItems;
        private bool _isVisible;

        public static bool IsDefaultCategory(IToolboxCategory category)
        {
            return category.Id.Equals(Guids.DefaultCategoryId);
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

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (value == _isVisible) return;
                _isVisible = value;
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

        public bool HasEnabledItems
        {
            get => _hasEnabledItems;
            protected set
            {
                if (value == _hasEnabledItems) return;
                _hasEnabledItems = value;
                OnPropertyChanged();
            }
        }

        public ToolboxCategory() : this(Guid.Empty, string.Empty, true)
        {
            IsNewlyCreated = true;
            EnterRenameMode();
        }

        public ToolboxCategory(string name) : this(Guid.Empty, name, true)
        {

        }

        public ToolboxCategory(Guid id, string name, bool isCustom = false) : base(id, name, isCustom)
        {
            Items = new BindableCollection<IToolboxItem>();
            Items.CollectionChanged += Items_CollectionChanged;
        }

        public void Refresh(Type targetType, bool forceVisible = false)
        {
            _currentType = targetType;
            _showAllStatus = forceVisible;
            foreach (var item in Items)
            {
                var flag = item.EvaluateEnabled(targetType);
                item.IsEnabled = flag;
                item.IsVisible = forceVisible || flag;
            }
            InternalRefreshState();
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
                    IoC.Get<ToolboxItemHost>().RegisterNode(item);
                    var flag2 = item.EvaluateEnabled(_currentType);
                    item.IsEnabled = flag2;
                    item.IsVisible = _showAllStatus || flag2;
                }
            }
            HasItems = Items.Any();
            InternalRefreshState();
        }

        private void InternalRefreshState()
        {
            HasVisibleItems = false;
            HasEnabledItems = false;
            if (!HasItems)
                return;
            if (Items.Any(x => x.IsVisible))
                HasVisibleItems = true;
            if (Items.Any(x => x.IsEnabled))
                HasEnabledItems = true;
            InternalEvaluateVisibility();
        }

        private void InternalEvaluateVisibility()
        {
            if (_showAllStatus || HasEnabledItems || HasVisibleItems || IsCustom || IsDefaultCategory(this))
                IsVisible = true;
            else
                IsVisible = false;
        }
    }
}
