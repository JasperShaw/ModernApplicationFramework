using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Interfaces.Commands;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Modules.Toolbox.ChooseItemsDialog.Internal
{
    internal class PackageInfoLoader : DisposableObject
    {
        private readonly ChooseItemsDataSource _dataSource;
        private ItemInfoLoader _currentInfoLoader;
        private IToolboxService _service;

        private IToolboxService Service => _service ?? (_service = IoC.Get<IToolboxService>());

        public PackageInfoLoader(ChooseItemsDataSource dataSource)
        {
            _dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
            _dataSource.PropertyChanged += OnDataSourcePropertyChanged;
            if (GetActivePage() == null)
                return;
            OnActivePageChanged();
        }

        public void ApplyChanges(ToolboxControlledPageDataSource page)
        {
            foreach (var item in page.Items)
            {
                var isChecked = item.IsChecked;
                var flag = _service.ToolboxHasItem(item.Definition);

                if (isChecked && !flag)
                    InstallItem(item);
                else if (!isChecked & flag)
                    RemoveItemFromToolbox(item);
            }
        }


        public ToolboxControlledPageDataSource GetActivePage()
        {
            var guid = _dataSource.ActivePageGuid;
            return _dataSource.ControlledPages.FirstOrDefault(x => x.Guid == guid);
        }

        public bool IsItemOnToolbox(ItemDataSource item)
        {
            return Service.ToolboxHasItem(item.Definition);
        }

        public void SyncToToolbox(ToolboxControlledPageDataSource page)
        {
            RefreshExistingItemsCheckState(page);
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();
            _currentInfoLoader?.Shutdown();
        }

        private static void InstallItem(ItemDataSource item)
        {
            IoC.Get<IAddItemToSelectedNodeCommand>()
                .Execute(new DataObject(ToolboxItemDataFormats.DataSource, item.Definition));
        }

        private void InitializePageContextOnPageChange(ToolboxControlledPageDataSource page)
        {
            if (page == null)
                return;
            LoadItems(page);
        }

        private void LoadItems(ToolboxControlledPageDataSource page)
        {
            _currentInfoLoader = new ItemInfoLoader(this, page);
        }

        private void OnActivePageChanged()
        {
            ThreadPool.QueueUserWorkItem(
                arg => InitializePageContextOnPageChange((ToolboxControlledPageDataSource) arg), GetActivePage());
        }

        private void OnDataSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ChooseItemsDataSource.ActivePageGuid))
            {
                OnActivePageChanged();
            }
            else if (e.PropertyName == nameof(ToolboxControlledPageDataSource.ListPopulationComplete))
            {
                if (!(sender is ToolboxControlledPageDataSource dataSource))
                    return;
                RefreshExistingItemsCheckState(dataSource);
            }
        }

        private void RefreshExistingItemsCheckState(ToolboxControlledPageDataSource page)
        {
            foreach (var item in page.Items)
                item.IsChecked = IsItemOnToolbox(item);
            if (page.ListPopulationComplete)
                return;
            page.ListPopulationComplete = true;
        }

        private void RemoveItemFromToolbox(ItemDataSource item)
        {
            var itemsToRemove = _service.FindItemsByDefintion(item.Definition);
            foreach (var toolboxItem in itemsToRemove)
                toolboxItem.Parent?.RemoveItem(toolboxItem);
        }
    }
}