using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Threading;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Modules.Toolbox.Interfaces.Commands;
using Action = System.Action;

namespace ModernApplicationFramework.Modules.Toolbox.ChooseItemsDialog
{
    [Export(typeof(ChooseItemsDialogViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ChooseItemsDialogViewModel : Screen
    {
        public static event Action<ChooseItemsDialogViewModel> DialogInitFinished;

        private ChooseItemsDialogView _window;
        private ChooseItemsDataSource _dataSource;
        private IList<TabItem> _tabPages;
        private TabItem _activeTab;

        private readonly Dictionary<TabItem, ToolboxControlledPageDataSource> _tabItemDataSourceMap = new Dictionary<TabItem, ToolboxControlledPageDataSource>();

        public ICommand OkCommand => new Command(OnOk);
        public ICommand ResetCommand => new Command(OnReset);


        internal ChooseItemsDataSource DataSource
        {
            get => _dataSource;
            set
            {
                if (_dataSource == value)
                    return;
                _dataSource = value;
                if (_dataSource == null)
                    return;
                _tabItemDataSourceMap.Clear();
                var sortedList = new SortedList<PageOrderKey, TabItem>();
                GetControlledPages(_dataSource, sortedList, _tabItemDataSourceMap);
                TabPages = new ObservableCollection<TabItem>(
                    sortedList.Select(pair => pair.Value));
                SyncActiveTabToDataModel();
                SyncDataModelToActiveTab();
            }
        }

        public IList<TabItem> TabPages
        {
            get => _tabPages;
            set
            {
                if (Equals(value, _tabPages)) return;
                _tabPages = value;
                NotifyOfPropertyChange();
            }
        }

        public TabItem ActiveTab
        {
            get => _activeTab;
            set
            {
                if (Equals(value, _activeTab)) return;
                _activeTab = value;
                NotifyOfPropertyChange();
                SyncDataModelToActiveTab();
            }
        }

        public ChooseItemsDialogViewModel()
        {
            TabPages = new ObservableCollection<TabItem>();
        }

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            if (!(view is ChooseItemsDialogView window))
                throw new InvalidOperationException("View is not a window");
            _window = window;
            MakeSureActivePageGetsFocusEvent();
            window.Closing += Window_Closing;
            window.Closed += Window_Closed;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (DataSource != null)
            {
                ThreadPool.QueueUserWorkItem(state => ((ChooseItemsDataSource) state).Dispose(), DataSource);
                DataSource = null;
            }
            DialogInitFinished = null;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !TryCloseAllPages();
        }

        private void OnOk()
        {
            var f = IoC.Get<IWaitDialogFactory>();
            f.CreateInstance(out var window);
            window.StartWaitDialog(null, "Wait", null, null, 0, false, true);
            try
            {
                AcceptChangesAndClose();
            }
            finally
            {
                window.EndWaitDialog();
            }
        }

        private void AcceptChangesAndClose()
        {
            ChooseItemsDataSource.ApplyChangesAction(DataSource);
            TryClose(true);
        }

        private void OnReset()
        {
            IoC.Get<IResetToolboxCommand>().Execute(null);
            SyncAfterReset();
            //TODO: Show Message
            _window.FocusButton(ChooseItemsDialogView.ButtonType.Ok);
            _window.EnsureDialogVisible();
        }

        private void SyncAfterReset()
        {
            if (DataSource == null)
                return;
            ChooseItemsDataSource.SyncToToolboxAction(DataSource);
            _window.EnsureDialogVisible();
        }

        private bool TryCloseAllPages()
        {
            var okToClose = true;
            //TODO
            return okToClose;
        }

        private void DiscardChangesAndClose()
        {
            if (!TryCloseAllPages())
                return;
            TryClose(true);
        }

        private void SyncDataModelToActiveTab()
        {
            if (ActiveTab == null)
                return;
            _dataSource.ActivePageGuid = _tabItemDataSourceMap[ActiveTab].Guid;
        }

        private void SyncActiveTabToDataModel()
        {
            var guid = _dataSource.ActivePageGuid;
            var tabItem = TabPages.FirstOrDefault(page => _tabItemDataSourceMap[page].Guid == guid);
            if (tabItem == null && TabPages.Count > 0)
                tabItem = TabPages[0];
            if (ActiveTab == tabItem)
                return;
            ActiveTab = tabItem;
        }

        private void GetControlledPages(ChooseItemsDataSource dataSource, SortedList<PageOrderKey, TabItem> tabItems,
            IDictionary<TabItem, ToolboxControlledPageDataSource> tabItemDataSourceMap)
        {
            using (var enumerator = dataSource.ControlledPages.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var current = enumerator.Current;
                    var key = new TabItem
                    {
                        Header = current.Name,
                        Content = new ChooseItemsPage(current)
                    };
                    tabItemDataSourceMap.Add(key, current);
                    tabItems.Add(new PageOrderKey(current), key);
                }
            }
        }

        private void MakeSureActivePageGetsFocusEvent()
        {
            void FocusAction() => _window.Dispatcher.BeginInvoke((Action)(() =>
           {
               ActiveTab?.Focus();
               var dialogInitFinished = DialogInitFinished;
               if (dialogInitFinished == null)
                   return;
               dialogInitFinished(this);
           }), DispatcherPriority.ApplicationIdle);

            if (_window.IsLoaded)
                FocusAction();
            else
                _window.Loaded += (sender, e) => FocusAction();
        }

        private class PageOrderKey : IComparable<PageOrderKey>
        {
            private readonly ToolboxControlledPageDataSource _page;
            private string _title;
            private int _order = -1;

            private int Order
            {
                get
                {
                    if (_order == -1)
                    {
                        _order = _page.Order;
                        //if (_order == 0)
                        //    _order = int.MaxValue;
                    }
                    return _order;
                }
            }

            private string Title => _title ?? (_title = _page.Name);

            public PageOrderKey(ToolboxControlledPageDataSource page)
            {
                _page = page;
            }

            public int CompareTo(PageOrderKey other)
            {
                if (other == null)
                    return -1;
                if (Order != other.Order)
                    return Order - other.Order;
                return string.Compare(Title, other.Title, StringComparison.CurrentCulture);
            }
        }
    }
}
