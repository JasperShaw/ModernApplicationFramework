using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Caliburn.Micro;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Extended.Layout;
using ModernApplicationFramework.Extended.Utilities.PaneUtilities;
using ModernApplicationFramework.Modules.Toolbox.Services;

namespace ModernApplicationFramework.Modules.Toolbox
{
    [Export(typeof(IToolbox))]
    public class ToolboxViewModel : Tool, IToolbox
    {
        private readonly IToolboxService _toolboxService;
        private readonly BindableCollection<ToolboxItemViewModel> _filteredItems;
        private readonly BindableCollection<ToolboxItemViewModel> _items;
        private readonly IDockingHostViewModel _hostViewModel;

        public override PaneLocation PreferredLocation => PaneLocation.Left;

        public IObservableCollection<ToolboxItemViewModel> Items => _filteredItems.Count == 0 ? _items : _filteredItems;

        [ImportingConstructor]
        public ToolboxViewModel(IDockingHostViewModel hostViewModel, IToolboxService service)
        {
            DisplayName = "Toolbox";

            _items = new BindableCollection<ToolboxItemViewModel>();
            _filteredItems = new BindableCollection<ToolboxItemViewModel>();

            var groupedItems = CollectionViewSource.GetDefaultView(_items);
            groupedItems.GroupDescriptions.Add(new PropertyGroupDescription("Category"));

            _toolboxService = service;
            _hostViewModel = hostViewModel;

            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return;

            hostViewModel.ActiveLayoutItemChanged += (sender, e) => RefreshToolboxItems();
            RefreshToolboxItems();
        }

        private void RefreshToolboxItems()
        {
            _items.Clear();
            if (_hostViewModel.ActiveItem == null)
                return;
            _items.AddRange(_toolboxService.GetToolboxItems(_hostViewModel.ActiveItem.GetType())
                .Select(x => new ToolboxItemViewModel(x)));
        }
    }
}
