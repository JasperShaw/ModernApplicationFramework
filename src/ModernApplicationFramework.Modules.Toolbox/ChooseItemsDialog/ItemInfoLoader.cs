using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.Modules.Toolbox.Items;
using ModernApplicationFramework.Modules.Toolbox.State;

namespace ModernApplicationFramework.Modules.Toolbox.ChooseItemsDialog
{
    internal class ItemInfoLoader
    {
        private ToolboxControlledPageDataSource _page;
        private readonly Status _status;
        private readonly BackgroundWorker _worker = new BackgroundWorker();
        private readonly List<ToolboxItemDefinitionBase> _definitions;

        public ItemInfoLoader(PackageInfoLoader loader, ToolboxControlledPageDataSource page)
        {
            _page = page;
            _status = new Status(page, loader);
            _definitions = IoC.Get<ToolboxItemDefinitionHost>().Definitions.Where(x => _page.Selector(x)).ToList();
            SetTotalCount();
            _worker.DoWork += LoadToolboxDefinitions;
            _worker.RunWorkerAsync();
        }

        public void Shutdown()
        {
            _status.Dispose();         
            _page = null;
        }

        private void SetTotalCount()
        {
            Execute.OnUIThread(() => { _page.TotalDefititions = _definitions.Count; });
        }

        private void SetCurrentDefinition(ToolboxItemDefinitionBase definition)
        {
            Execute.OnUIThread(() => { _page.CurrentDefinition = definition.Name; });
        }

        private void SetLoadedDefinitionCount(int count)
        {
            Execute.OnUIThread(() => { _page.LoadedDefinitions = count; });
        }

        private void LoadToolboxDefinitions(object sender, DoWorkEventArgs e)
        {
            var index = 0;
            while (!_status.IsComplete)
            {
                if (index >= _definitions.Count)
                {
                    _status.IsComplete = true;
                    break;
                }
                var definition = _definitions[index];
                SetCurrentDefinition(definition);
                var item = _page.ItemFactory.Create(definition);
                _status.AddItem(item);
                ++index;
                SetLoadedDefinitionCount(index);
            }
        }

        public class Status : IDisposable
        {
            private ToolboxControlledPageDataSource _page;
            private PackageInfoLoader _loader;
            private bool _isComplete;

            public bool IsComplete
            {
                get => _isComplete;
                set
                {
                    if (value == _isComplete)
                        return;
                    _isComplete = value;
                    _page.ListPopulationComplete = _isComplete;
                }
            }

            public Status(ToolboxControlledPageDataSource page, PackageInfoLoader loader)
            {
                _page = page ?? throw new ArgumentNullException(nameof(page));
                _loader = loader ?? throw new ArgumentNullException(nameof(loader));
                page.PropertyChanged += Page_PropertyChanged;
                UpdateIsComplete();
            }

            private void Page_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == nameof(ToolboxControlledPageDataSource.ListItemsAdded))
                    UpdateIsComplete();
            }

            public void AddItem(ItemDataSource itemData)
            {
                Execute.OnUIThread(() =>
                {
                    itemData.IsChecked = _loader.IsItemOnToolbox(itemData);
                    _page.InsertSortedAndFiltered(itemData);
                });
            }

            private void UpdateIsComplete()
            {
                IsComplete = _page.ListPopulationComplete;
            }

            public void Dispose()
            {
                _page = null;
                _loader = null;
            }
        }
    }
}
