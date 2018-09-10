using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Search;
using ModernApplicationFramework.Basics.Search.Internal;
using ModernApplicationFramework.Basics.Services;
using ModernApplicationFramework.Interfaces.Controls.Search;
using ModernApplicationFramework.Interfaces.Search;
using ModernApplicationFramework.Native;
using ModernApplicationFramework.Utilities;
using Action = System.Action;

namespace ModernApplicationFramework.Controls.SearchControl
{
    public class WindowSearchHost : DisposableObject, IWindowSearchHost, IWindowSearchHostPrivate, IWindowSearchEvents,
        IWindowSearchEventsHandler, ISearchCallback
    {
        private Guid _searchCategory = Guid.Empty;
        private uint _searchCookie;

        public bool IsEnabled { get; set; }

        public IWindowSearchEvents SearchEvents
        {
            get
            {
                ThrowIfDisposed();
                return this;
            }
        }

        public bool IsVisible { get; set; }

        public SearchControl SearchControl { get; }

        public ISearchQuery SearchQuery => new SearchQuery(SearchString);

        public ISearchTask SearchTask { get; private set; }

        public object SearchParentControl { get; }

        private SearchControlDataSource DataSource { get; set; }

        private string SearchString
        {
            get
            {
                if (DataSource != null)
                    return DataSource.SearchText;
                return string.Empty;
            }
        }

        private IWindowSearch WindowSearch { get; set; }

        public WindowSearchHost(FrameworkElement parentControl)
        {
            if (parentControl == null)
                throw new ArgumentNullException(nameof(parentControl));
            SearchControl = new SearchControl();
            WindowHelper.CreateChildElement(SearchControl, parentControl);
            SearchParentControl = parentControl;
        }

        public void Activate()
        {
            ThrowIfDisposed();
            if (!IsEnabled)
                throw new InvalidOperationException("WindowSearchHost.cs -- No Enabled");
            if (SearchControl.Visibility != Visibility.Visible)
                throw new InvalidOperationException("WindowSearchHost.cs -- Control no Visible");
            PendingFocusHelper.SetFocusOnLoad(SearchControl);
        }

        public void OnAddMruItem(string searchedText)
        {
            ThrowIfDisposedOrSearchNotSetup();
            var mruStore = IoC.Get<ISearchMruItemStore>();
            mruStore.AddMruItem(ref _searchCategory, searchedText);

            var listCopy = DataSource.SearchMruItems.ToList();
            listCopy.Insert(0, new WindowSearchMruItem(searchedText, this));
            var num = listCopy.Count;
            if (num <= DataSource.SearchSettings.MaximumMruItems)
            {
                DataSource.SearchMruItems = listCopy;
                return;
            }

            listCopy.RemoveAt(num - 1);
            DataSource.SearchMruItems = listCopy;
        }

        public void OnClearSearch()
        {
            ThrowIfDisposedOrSearchNotSetup();
            WindowSearch.ClearSearch();
        }

        public void OnDeleteMruItem(SearchMruItem item)
        {
            ThrowIfDisposedOrSearchNotSetup();
            var mruStore = IoC.Get<ISearchMruItemStore>();
            mruStore.DeleteMruItem(ref _searchCategory, item.Text);
            var maximumMruItems = DataSource.SearchSettings.MaximumMruItems;
            var strArray = new string[maximumMruItems];
            var items = mruStore.GetMruItems(ref _searchCategory, SearchString, maximumMruItems, strArray);

            var itemsCopy = DataSource.SearchMruItems.ToList();
            RemoveItem(ref itemsCopy, item);

            if (items <= itemsCopy.Count)
            {
                DataSource.SearchMruItems = itemsCopy;
                return;
            }

            itemsCopy.Add(new WindowSearchMruItem(strArray[items - 1], this));
            DataSource.SearchMruItems = itemsCopy;
        }

        public bool OnNotifyNavigationKey(SearchNavigationKeys key, UiAccelModifiers modifiers)
        {
            ThrowIfDisposedOrSearchNotSetup();
            return WindowSearch.OnNavigationKeyDown((uint) key, (uint) modifiers);
        }

        public void OnPopulateMruItems(string searchPrefix)
        {
            ThrowIfDisposedOrSearchNotSetup();
            var mruStore = IoC.Get<ISearchMruItemStore>();
            var maximumMruItems = DataSource.SearchSettings.MaximumMruItems;
            var strArray = new string[maximumMruItems];
            var items = mruStore.GetMruItems(ref _searchCategory, searchPrefix, maximumMruItems, strArray);

            var list = new List<SearchMruItem>();
            for (var i = 0; i < items; ++i)
                list.Add(new WindowSearchMruItem(strArray[i], this));

            DataSource.SearchMruItems = list;
        }

        public void OnSelectMruItem(SearchMruItem item)
        {
            ThrowIfDisposedOrSearchNotSetup();
            var mruStore = IoC.Get<ISearchMruItemStore>();
            mruStore.SetMruItem(ref _searchCategory, item.Text);
            var newItem = new WindowSearchMruItem(item.Text, this);
            var itemsCopy = DataSource.SearchMruItems.ToList();
            RemoveItem(ref itemsCopy, item);
            itemsCopy.Insert(0, newItem);
            DataSource.SearchMruItems = itemsCopy;
        }

        public void OnStartSearch(string searchText)
        {
            ThrowIfDisposedOrSearchNotSetup();
            if (searchText == null || DataSource.SearchSettings.SearchTrimsWhitespaces &&
                string.IsNullOrWhiteSpace(searchText))
                throw new InvalidOperationException("WindowSearchHost.cs -- SearchQuery Empty");
            StartSearch(SearchQuery, null);
        }

        public void OnStopSearch()
        {
            ThrowIfDisposedOrSearchNotSetup();
            if (SearchTask == null)
                return;
            SearchTask.Stop();
            SearchTask = null;
        }

        public void ReportComplete(ISearchTask task, uint resultsFound)
        {
            Execute.OnUIThread(() =>
            {
                if (task == SearchTask)
                {
                    DataSource.SearchStatus = SearchStatus.Complete;
                    SearchTask = null;
                }

                if ((int) resultsFound < 0 || DataSource == null)
                    return;
                if (DataSource.SearchResultsCount == -1)
                    DataSource.SearchResultsCount = (int) resultsFound;
                else
                    DataSource.SearchResultsCount += (int) resultsFound;
            });
        }

        public void ReportProgress(ISearchTask task, uint progress, uint maxProgress)
        {
            if (maxProgress == 0)
                throw new ArgumentException("WindowSearchHowst.cs -- maxProgress is 0");
            Execute.OnUIThread(() =>
            {
                if (task == SearchTask)
                    return;
                DataSource.SearchProgress = (int) (progress * 100.0 / maxProgress);
            });
        }

        public void SearchOptionsChanged()
        {
            ThrowIfDisposed();
            UpdateSearchOptions();
        }

        public void SearchOptionValueChanged(IWindowSearchBooleanOption option)
        {
            ThrowIfDisposed();
            using (var enumerator = DataSource.SearchOptions.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current is WindowSearchBooleanOptionDataSource current && option == current.Option)
                        current.Value = option.Value;
                }
            }
        }

        public void SetupSearch(IWindowSearch windowSearch)
        {
            ThrowIfDisposed();
            if (windowSearch == null)
                throw new ArgumentNullException(nameof(windowSearch));

            Execute.OnUIThread(() =>
            {
                if (WindowSearch != null)
                    throw new InvalidOperationException("WindowSearchHost.cs -- Already Setted up");
                WindowSearch = windowSearch;
                _searchCategory = WindowSearch.Category;
                CreateDataSource();
                WindowSearch.ProvideSearchSettings(DataSource.SearchSettings);
                SearchControl.DataContext = DataSource;
            });
        }

        public void TerminateSearch()
        {
            if (IsDisposed)
                return;
            Execute.OnUIThread(() =>
            {
                if (WindowSearch == null)
                    return;
                SearchTask = null;
                _searchCookie = 0;
                DataSource = null;
                if (SearchControl != null)
                    SearchControl.DataContext = null;
                _searchCategory = Guid.Empty;
                WindowSearch = null;
            });
        }

        protected override void DisposeManagedResources()
        {
            TerminateSearch();
        }

        private void CreateDataSource()
        {
            DataSource = new WindowSearchDataSource(this);
            UpdateSearchOptions();
            //TODO: Update filter options

        }

        private void UpdateSearchOptions()
        {
            var collection = new List<SearchOptionDataSource>();
            var searchOptions = WindowSearch.SearchOptionsEnum;

            if (searchOptions != null)
            {
                using (var enumerator = new EnumerableSearchOptionsCollection(searchOptions).GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        var current = enumerator.Current;
                        WindowSearchOptionDataSource optionDataSource;

                        switch (current)
                        {
                            case IWindowSearchBooleanOption option1:
                                optionDataSource = new WindowSearchBooleanOptionDataSource(option1);
                                break;
                            case IWindowSearchCommandOption option2:
                                optionDataSource = new WindowSearchCommandOptionDataSource(option2);
                                break;
                            default:
                                continue;
                        }
                        collection.Add(optionDataSource);
                    }              
                }
            }
            DataSource.SearchOptions = collection;
        }

        private bool RemoveItem(ref List<SearchMruItem> collection, SearchMruItem item)
        {
            var count = collection.Count;
            for (var i = 0; i < count; ++i)
            {
                var originalItem = collection.ElementAt(i);

                if (originalItem == item)
                {
                    collection.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        private void StartSearch(ISearchQuery searchQuery, Action beforeSearchStartCallback)
        {
            if (searchQuery == null)
                throw new ArgumentNullException("WindowSearcHost.cs -- searchquery is null");
            if (SearchTask != null)
                throw new InvalidOperationException("WindowSearcHost.cs -- search already in progress");
            ++_searchCookie;
            var search = WindowSearch.CreateSearch(_searchCookie, searchQuery, this);
            SearchTask = search;
            beforeSearchStartCallback?.Invoke();
            if (search == null)
                ReportComplete(null, uint.MaxValue);
            else
                ThreadPool.QueueUserWorkItem(task =>
                {
                    if (!(task is ISearchTask searchTask) || searchTask != SearchTask)
                        return;
                    searchTask.Start();
                }, search);
        }

        private void ThrowIfDisposedOrSearchNotSetup()
        {
            ThrowIfDisposed();
            ThrowIfSearchNotSetup();
        }

        private void ThrowIfSearchNotSetup()
        {
            if (WindowSearch == null)
                throw new InvalidOperationException("WindowSearchHost.cs -- WindowSearch not setted up");
        }
    }
}