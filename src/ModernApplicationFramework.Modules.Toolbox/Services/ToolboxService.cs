using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.State;

namespace ModernApplicationFramework.Modules.Toolbox.Services
{
    [Export(typeof(IToolboxService))]
    internal class ToolboxService : IToolboxService
    {
        private readonly ToolboxItemHost _host;
        private readonly IToolboxStateProvider _stateProvider;

        internal static IToolboxService Instance { get; private set; }

        [ImportingConstructor]
        public ToolboxService(ToolboxItemHost host, IToolboxStateProvider stateProvider)
        {
            _host = host;
            _stateProvider = stateProvider;
            Instance = this;
        }

        public IReadOnlyCollection<IToolboxCategory> GetToolboxItemSource()
        {
            return _stateProvider.ItemsSource.ToList();
        }


        public void StoreItemsSource(IEnumerable<IToolboxCategory> itemsSource)
        {
            _stateProvider.ItemsSource.Clear();
            _stateProvider.ItemsSource.AddRange(itemsSource);
        }

        public IReadOnlyCollection<string> GetAllToolboxCategoryNames()
        {
            return _host.AllCategories.Select(x => x.Name).ToList();
        }
    }
}
