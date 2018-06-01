using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.DragDrop;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Extended.Layout;
using ModernApplicationFramework.Extended.Utilities.PaneUtilities;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;

namespace ModernApplicationFramework.Modules.Toolbox
{
    [Export(typeof(IToolbox))]
    public class ToolboxViewModel : Tool, IToolbox
    {
        private readonly IToolboxService _toolboxService;
        //private readonly BindableCollection<ToolboxItemViewModel> _filteredItems;
        //private readonly BindableCollection<ToolboxItemViewModel> _items;
        private readonly IDockingHostViewModel _hostViewModel;

        private readonly BindableCollection<IToolboxCategory> _categories;
        private ToolboxNodeItem _selectedNode;

        public override PaneLocation PreferredLocation => PaneLocation.Left;

        //public IObservableCollection<ToolboxItemViewModel> Items => _filteredItems.Count == 0 ? _items : _filteredItems;

        public IObservableCollection<IToolboxCategory> Categories => _categories;

        public IDropTarget ToolboxDropHandler { get; } = new ToolboxDropHandler();
        public IDragSource ToolboxDragHandler { get; } = new ToolboxDragHandler();

        public ToolboxNodeItem SelectedNode
        {
            get => _selectedNode;
            set
            {
                if (Equals(value, _selectedNode)) return;
                _selectedNode = value;
                NotifyOfPropertyChange();
            }
        }

        [ImportingConstructor]
        public ToolboxViewModel(IDockingHostViewModel hostViewModel, IToolboxService service)
        {
            DisplayName = "Toolbox";

            //_items = new BindableCollection<ToolboxItemViewModel>();
            _categories = new BindableCollection<IToolboxCategory>();

            //_filteredItems = new BindableCollection<ToolboxItemViewModel>();
            //var groupedItems = CollectionViewSource.GetDefaultView(_items);
            //groupedItems.GroupDescriptions.Add(new PropertyGroupDescription("Category"));

            _toolboxService = service;
            _hostViewModel = hostViewModel;

            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return;

            hostViewModel.ActiveLayoutItemChanging += (sender, e) => StoreItems(e.OldLayoutItem);
            hostViewModel.ActiveLayoutItemChanged += (sender, e) => RefreshToolboxItems(e.NewLayoutItem);
            
            RefreshToolboxItems(_hostViewModel.ActiveItem);

        }

        private void StoreItems(ILayoutItem item)
        {
            if(item == null)
                return;
            _toolboxService.StoreItemSource(item.GetType(), _categories.ToList());
        }

        private void RefreshToolboxItems(ILayoutItem item)
        {
            _categories.Clear();
            if (item == null)
            {
                //Change targettype to object, which means that it will get accepted by convention
                ToolboxItemCategory.DefaultCategory.Refresh(typeof(object));
                _categories.Add(ToolboxItemCategory.DefaultCategory);
                return;
            }
            var i = _toolboxService.GetToolboxItemSource(item.GetType());
            i.ForEach(x => x.Refresh(item.GetType()));         
            _categories.AddRange(i);
        }
    }
}
