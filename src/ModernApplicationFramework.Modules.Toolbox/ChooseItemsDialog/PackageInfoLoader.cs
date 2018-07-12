using System;
using System.Linq;
using System.Threading;
using Caliburn.Micro;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Modules.Toolbox.ChooseItemsDialog
{
    internal class PackageInfoLoader : DisposableObject
    {
        private readonly ChooseItemsDataSource _dataSource;
        private ToolboxControlledPageDataSource _currentPage;
        private ItemInfoLoader _currentInfoLoader;
        private IToolboxService _service;

        private IToolboxService Service => _service ?? (_service = IoC.Get<IToolboxService>());

        public ItemInfoLoader ActivePageInfoLoader => _currentInfoLoader;

        public PackageInfoLoader(ChooseItemsDataSource dataSource)
        {
            _dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));

            _dataSource.PropertyChanged += OnDataSourcePropertyChanged;

            if (GetActivePage() == null)
                return;
            OnActivePageChanged();
        }

        private void OnDataSourcePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ChooseItemsDataSource.ActivePageGuid))
                OnActivePageChanged();
        }

        public ToolboxControlledPageDataSource GetActivePage()
        {
            var guid = _dataSource.ActivePageGuid;
            return _dataSource.ControlledPages.FirstOrDefault(x => x.Guid == guid);
        }

        public bool IsItemOnToolbox(ItemDataSource item)
        {
            return Service.FindItemsByDefintion(item.Definition).Any();
        }

        private void OnActivePageChanged()
        {
            ThreadPool.QueueUserWorkItem(
                arg => InitializePageContextOnPageChange((ToolboxControlledPageDataSource) arg), GetActivePage());
        }

        private void InitializePageContextOnPageChange(ToolboxControlledPageDataSource page)
        {
            _currentPage = page;
            if (page == null) 
                return;
            LoadItems(page);
        }

        private void LoadItems(ToolboxControlledPageDataSource page)
        {
            _currentInfoLoader = new ItemInfoLoader(this, page);
        }

        public void SyncToToolbox(ToolboxControlledPageDataSource page)
        {
            RefreshExistingItemsCheckState(page);
        }

        private void RefreshExistingItemsCheckState(ToolboxControlledPageDataSource page)
        {

        }
    }
}
