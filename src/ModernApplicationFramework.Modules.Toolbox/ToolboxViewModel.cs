using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using GongSolutions.Wpf.DragDrop;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Extended.Layout;
using ModernApplicationFramework.Extended.Utilities.PaneUtilities;

namespace ModernApplicationFramework.Modules.Toolbox
{
    [Export(typeof(IToolbox))]
    public class ToolboxViewModel : Tool, IToolbox
    {
        //private readonly IToolboxService _toolboxService;
        //private readonly BindableCollection<ToolboxItemViewModel> _filteredItems;
        //private readonly BindableCollection<ToolboxItemViewModel> _items;
        private readonly IDockingHostViewModel _hostViewModel;

        private readonly BindableCollection<ToolboxItemCategory> _categories;
        private ToolboxNodeItem _selectedNode;

        public override PaneLocation PreferredLocation => PaneLocation.Left;

        //public IObservableCollection<ToolboxItemViewModel> Items => _filteredItems.Count == 0 ? _items : _filteredItems;

        public IObservableCollection<ToolboxItemCategory> Categories => _categories;

        public IDropTarget ToolboxDropHandler { get; } = new ToolboxDropHandler();

        //public IObservableCollection<TreeModel> Categories { get; }

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
        public ToolboxViewModel(IDockingHostViewModel hostViewModel) //, IToolboxService service)
        {
            DisplayName = "Toolbox";

            //_items = new BindableCollection<ToolboxItemViewModel>();
            _categories = new BindableCollection<ToolboxItemCategory>();

            //_filteredItems = new BindableCollection<ToolboxItemViewModel>();
            //var groupedItems = CollectionViewSource.GetDefaultView(_items);
            //groupedItems.GroupDescriptions.Add(new PropertyGroupDescription("Category"));

            //_toolboxService = service;
            _hostViewModel = hostViewModel;

            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return;

            hostViewModel.ActiveLayoutItemChanged += (sender, e) => RefreshToolboxItems();
            RefreshToolboxItems();

        }

        private void RefreshToolboxItems()
        {
            //_items.Clear();
            _categories.Clear();
            if (_hostViewModel.ActiveItem == null)
                return;

            var item = IoC.GetAll<IToolboxItem>();

            var categories = IoC.GetAll<ToolboxItemCategory>()
                .Where(x => x.TargetType == _hostViewModel.ActiveItem.GetType());

            _categories.AddRange(categories);

            //_items.AddRange(_toolboxService.GetToolboxItems(_hostViewModel.ActiveItem.GetType())
            //    .Select(x => new ToolboxItemViewModel(x)));
        }
    }
}
