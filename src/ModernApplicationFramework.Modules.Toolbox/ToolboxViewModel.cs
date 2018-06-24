using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Search;
using ModernApplicationFramework.DragDrop;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Extended.Layout;
using ModernApplicationFramework.Extended.Utilities.PaneUtilities;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Search;
using ModernApplicationFramework.Modules.Toolbox.CommandBar;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;

namespace ModernApplicationFramework.Modules.Toolbox
{
    [Export(typeof(IToolbox))]
    [Guid("DD8E4CE1-7431-47C1-9496-B8C91D6E0B55")]
    public sealed class ToolboxViewModel : Tool, IToolbox
    {
        private readonly IDockingHostViewModel _hostViewModel;
        private readonly IToolboxStateProvider _stateProvider;

        private readonly BindableCollection<IToolboxCategory> _categories;
        private IToolboxNode _selectedNode;
        private bool _showAllItems;
        private bool _supressStore;

        private bool _isSearching;

        public override PaneLocation PreferredLocation => PaneLocation.Left;

        public IObservableCollection<IToolboxCategory> Categories => _categories;

        public IDropTarget ToolboxDropHandler { get; } = new ToolboxDropHandler();
        public IDragSource ToolboxDragHandler { get; } = new ToolboxDragHandler();

        public override bool SearchEnabled => true;

        public IToolboxCategory SelectedCategory { get; private set; }


        public IReadOnlyCollection<IToolboxCategory> CurrentLayout => _categories.ToList();

        public bool ShowAllItems
        {
            get => _showAllItems;
            set
            {
                if (value == _showAllItems)
                    return;
                _showAllItems = value;
                ToggleShowAllItems(_showAllItems);
                NotifyOfPropertyChange();
            }
        }

        public ContextMenu ContextMenu => IoC.Get<IContextMenuHost>()
            .GetContextMenu(ToolboxContextMenuDefinition.ToolboxContextMenu);


        public IToolboxNode SelectedNode
        {
            get => _selectedNode;
            set
            {
                if (Equals(value, _selectedNode))
                    return;
                _selectedNode = value;
                UpdateSelectedCategory(_selectedNode);
                NotifyOfPropertyChange();
            }
        }


        [ImportingConstructor]
        public ToolboxViewModel(IDockingHostViewModel hostViewModel, IToolboxStateProvider stateProvider)
        {
            DisplayName = "Toolbox";
            ContextMenuProvider = new ToolboxContextMenuProvider();
            _categories = new BindableCollection<IToolboxCategory>();
            _hostViewModel = hostViewModel;
            _stateProvider = stateProvider;
            //TODO: change to collection changed         
            hostViewModel.ActiveLayoutItemChanging += (sender, e) => StoreItems();
            hostViewModel.ActiveLayoutItemChanged += (sender, e) => RefreshToolboxItems(e.NewLayoutItem);
            RefreshView();
            Categories.CollectionChanged += (sender, args) => StoreItems();
        }

        public void RefreshView()
        {
            RefreshToolboxItems(_hostViewModel.ActiveItem);
        }

        private void StoreItems()
        {
            if (_supressStore || _isSearching)
                return;
            _stateProvider.ItemsSource.Clear();
            _stateProvider.ItemsSource.AddRange(_categories);
        }

        private void RefreshToolboxItems(ILayoutItem item)
        {
            _supressStore = true;
            _categories.Clear();
            var i = _stateProvider.ItemsSource.ToList();

            //Change type to object when current item is null, which means that it will get accepted by convention
            var type = item == null ? typeof(object) : item.GetType();
            i.ForEach(x => x.Refresh(type, _showAllItems));
            _categories.AddRange(i);
            _supressStore = false;
        }

        private void ToggleShowAllItems(bool showAllItems)
        {
            var type = typeof(object);
            if (_hostViewModel.ActiveItem != null)
                type = _hostViewModel.ActiveItem.GetType();

            foreach (var category in Categories)
            {
                category.Refresh(type, showAllItems);
            }
        }

        private void UpdateSelectedCategory(IToolboxNode selectedNode)
        {
            if (selectedNode is IToolboxCategory category)
                SelectedCategory = category;
            else if (selectedNode is IToolboxItem item)
                SelectedCategory = item.Parent;
            else
                SelectedCategory = null;
        }

        public override void ProvideSearchSettings(SearchSettingsDataSource dataSource)
        {
            dataSource.SearchStartType = SearchStartType.Instant;
        }

        public override ISearchTask CreateSearch(uint cookie, ISearchQuery searchQuery, ISearchCallback searchCallback)
        {
            if (searchQuery == null || searchCallback == null)
                return null;
            _isSearching = true;
            return new ToolboxSearchTask(cookie, searchQuery, searchCallback, this);
        }

        public override void ClearSearch()
        {
            _isSearching = false;
            RefreshView();
            base.ClearSearch();
        }
    }
}
