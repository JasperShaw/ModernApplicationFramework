using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;
using Caliburn.Micro;
using ModernApplicationFramework.DragDrop;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Extended.Layout;
using ModernApplicationFramework.Extended.Utilities.PaneUtilities;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.CommandBar;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;

namespace ModernApplicationFramework.Modules.Toolbox
{
    [Export(typeof(IToolbox))]
    public class ToolboxViewModel : Tool, IToolbox
    {
        //private readonly IToolboxService _toolboxService;

        //private readonly BindableCollection<ToolboxItemViewModel> _filteredItems;
        //private readonly BindableCollection<ToolboxItemViewModel> _items;
        private readonly IDockingHostViewModel _hostViewModel;
        private readonly IToolboxStateProvider _stateProvider;

        private readonly BindableCollection<IToolboxCategory> _categories;
        private IToolboxNode _selectedNode;
        private bool _showAllItems;

        public override PaneLocation PreferredLocation => PaneLocation.Left;

        //public IObservableCollection<ToolboxItemViewModel> Items => _filteredItems.Count == 0 ? _items : _filteredItems;

        public IObservableCollection<IToolboxCategory> Categories => _categories;

        public IDropTarget ToolboxDropHandler { get; } = new ToolboxDropHandler();
        public IDragSource ToolboxDragHandler { get; } = new ToolboxDragHandler();

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

            //_items = new BindableCollection<ToolboxItemViewModel>();
            _categories = new BindableCollection<IToolboxCategory>();


            //_filteredItems = new BindableCollection<ToolboxItemViewModel>();
            //var groupedItems = CollectionViewSource.GetDefaultView(_items);
            //groupedItems.GroupDescriptions.Add(new PropertyGroupDescription("Category"));

            //_toolboxService = service;
            _hostViewModel = hostViewModel;
            _stateProvider = stateProvider;

            //TODO: change to collection changed?
            hostViewModel.ActiveLayoutItemChanging += (sender, e) => StoreItems();
            hostViewModel.ActiveLayoutItemChanged += (sender, e) => RefreshToolboxItems(e.NewLayoutItem);

            RefreshView();
        }


        public void RefreshView()
        {
            RefreshToolboxItems(_hostViewModel.ActiveItem);
        }

        private void StoreItems()
        {
            _stateProvider.ItemsSource.Clear();
            _stateProvider.ItemsSource.AddRange(_categories);
        }

        private void RefreshToolboxItems(ILayoutItem item)
        {
            _categories.Clear();
            var i = _stateProvider.ItemsSource.ToList();

            //Change type to object when current item is null, which means that it will get accepted by convention
            var type = item == null ? typeof(object) : item.GetType();
            i.ForEach(x => x.Refresh(type, _showAllItems));
            _categories.AddRange(i);
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
    }
}
