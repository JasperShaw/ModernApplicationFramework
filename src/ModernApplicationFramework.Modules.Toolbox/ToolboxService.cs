using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Items;

namespace ModernApplicationFramework.Modules.Toolbox
{
    [Export(typeof(IToolboxService))]
    internal class ToolboxService : IToolboxService
    {
        private readonly IInternalToolboxStateProvider _stateProvider;
        private readonly IToolbox _toolbox;

        internal static IToolboxService Instance { get; private set; }

        [ImportingConstructor]
        public ToolboxService(IInternalToolboxStateProvider stateProvider, IToolbox toolbox)
        {
            _stateProvider = stateProvider;
            _toolbox = toolbox;
            Instance = this;
        }

        public IReadOnlyCollection<IToolboxCategory> GetToolboxItemSource()
        {
            return _stateProvider.ItemsSource.ToList();
        }

        public void StoreCurrentLayout()
        {
            InternalStoreLayout(_toolbox.CurrentLayout);
        }

        public void StoreAndApplyLayout(IEnumerable<IToolboxCategory> layout)
        {
            InternalStoreLayout(layout);
            _toolbox.RefreshView();
        }

        private void InternalStoreLayout(IEnumerable<IToolboxCategory> layout)
        {
            _stateProvider.ItemsSource.Clear();
            _stateProvider.ItemsSource.AddRange(layout);
        }

        public void AddCategory(IToolboxCategory category, bool suppressRefresh = false)
        {
            var index = _stateProvider.ItemsSource.Count;
            if (ToolboxCategory.IsDefaultCategory(_stateProvider.ItemsSource.LastOrDefault()))
                index--;
            InsertCategory(index, category);
        }

        public void InsertCategory(int index, IToolboxCategory category, bool supressRefresh = false)
        {
            _stateProvider.ItemsSource.Insert(index, category);
            if (!supressRefresh)
                _toolbox.RefreshView();
        }

        public void RemoveCategory(IToolboxCategory category, bool cascading = true, bool supressRefresh = false)
        {
            if (_stateProvider.ItemsSource.Contains(category))
                _stateProvider.ItemsSource.Remove(category);
            if (cascading)
            {
                foreach (var item in category.Items.ToList())
                    category.RemoveItem(item);
            }
            if (!supressRefresh)
                _toolbox.RefreshView();
        }

        public IToolboxCategory GetCategoryById(Guid guid)
        {
            if (guid == Guid.Empty)
                throw new InvalidOperationException("Guid cannot be empty");
            return _stateProvider.State.FirstOrDefault(x => x.Id.Equals(guid));
        }

        public IToolboxItem GetItemById(Guid guid)
        {
            if (guid == Guid.Empty)
                throw new InvalidOperationException("Guid cannot be empty");

            return _stateProvider.State.Select(category => category.Items.FirstOrDefault(x => x.Id == guid))
                .FirstOrDefault(item => item != null);
        }

        public IEnumerable<IToolboxItem> FindItemsByDefintion(ToolboxItemDefinitionBase definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            foreach (var category in _stateProvider.State)
            {
                var item = category.Items.FirstOrDefault(x => x.DataSource.Equals(definition));
                if (item != null)
                    yield return item;
            }
        }

        public bool ToolboxHasItem(ToolboxItemDefinitionBase definition)
        {
            return _stateProvider.State.Any(category =>
                category.Items.FirstOrDefault(x => x.DataSource.Equals(definition)) != null);
        }

        public IToolboxCategory GetSelectedCategory()
        {
            return _toolbox.SelectedCategory;
        }

        public IReadOnlyCollection<string> GetAllToolboxCategoryNames()
        {
            return _stateProvider.State.Select(x => x.Name).ToList();
        }
    }
}
