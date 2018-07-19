using System;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Resources;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Modules.Toolbox.Items
{
    public class ToolboxCategory : ToolboxNode, IToolboxCategory
    {
        [Export] internal static ToolboxCategoryDefinition DefaultCategoryDefinition =
            new ToolboxCategoryDefinition(Guids.DefaultCategoryId, ToolboxResources.DefaultCategoryName);


        [Export] private static IToolboxCategory _defaultCategory =
            new ToolboxCategory(Guids.DefaultCategoryId, DefaultCategoryDefinition);

        private Type _currentType;
        private bool _hasEnabledItems;
        private bool _hasItems;
        private bool _hasVisibleItems;
        private bool _isVisible;

        private IObservableCollection<IToolboxItem> _items;
        private bool _showAllStatus;

        public ToolboxCategoryDefinition DataSource { get; }

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

        public ToolboxCategory() : this(Guid.NewGuid(), new ToolboxCategoryDefinition(Guid.Empty, string.Empty), true)
        {
            IsNewlyCreated = true;
            EnterRenameMode();
            IsVisible = true;
        }

        public ToolboxCategory(string name) : this(Guid.NewGuid(), new ToolboxCategoryDefinition(Guid.Empty, name),
            true)
        {
        }

        public ToolboxCategory(Guid id, ToolboxCategoryDefinition definition, bool isCustom = false) : base(id,
            definition.Name, isCustom)
        {
            Validate.IsNotNull(definition, nameof(definition));
            DataSource = definition;

            Items = new BindableCollection<IToolboxItem>();
            Items.CollectionChanged += Items_CollectionChanged;

            InternalEvaluateVisibility();
        }

        public ToolboxCategory(ToolboxCategoryDefinition definition) : this(Guid.NewGuid(), definition, true)
        {
        }

        public static bool IsDefaultCategory(IToolboxCategory category)
        {
            return category.DataSource.Id.Equals(Guids.DefaultCategoryId);
        }

        public void InvalidateState()
        {
            HasVisibleItems = false;
            HasEnabledItems = false;
            if (!HasItems)
            {
                InternalEvaluateVisibility();
                return;
            }

            if (Items.Any(x => x.IsVisible))
                HasVisibleItems = true;
            if (Items.Any(x => x.IsEnabled))
                HasEnabledItems = true;
            InternalEvaluateVisibility();
        }

        public override bool IsRenameValid(out string errorMessage)
        {
            if (!base.IsRenameValid(out errorMessage))
                return false;
            if (EditingName == Name)
                return true;
            if (!ToolboxService.Instance.GetAllToolboxCategoryNames().Contains(EditingName))
                return true;
            errorMessage = string.Format(ToolboxResources.Error_Duplicate, EditingName);
            return false;
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

            InvalidateState();
        }

        public bool RemoveItem(IToolboxItem item)
        {
            if (!Items.Contains(item))
                return false;
            Items.Remove(item);
            return true;
        }

        private void InternalEvaluateVisibility()
        {
            if (_showAllStatus || HasEnabledItems || HasVisibleItems || IsCustom || IsDefaultCategory(this))
                IsVisible = true;
            else
                IsVisible = false;
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
                foreach (IToolboxItem item in e.OldItems)
                    item.Parent = null;
            if (e.NewItems != null)
                foreach (IToolboxItem item in e.NewItems)
                {
                    item.Parent = this;
                    var flag2 = item.EvaluateEnabled(_currentType);
                    item.IsEnabled = flag2;
                    item.IsVisible = _showAllStatus || flag2;
                }

            HasItems = Items.Any();
            InvalidateState();
        }
    }
}