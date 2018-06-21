using System;
using System.Windows;
using ModernApplicationFramework.Basics.Search.Internal;
using ModernApplicationFramework.Interfaces.Controls.Search;
using ModernApplicationFramework.Interfaces.Search;
using ModernApplicationFramework.Native;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Controls.SearchControl
{
    public class WindowSearchHost : DisposableObject, IWindowSearchHost, IWindowSearchHostPrivate, IWindowSearchEvents, IWindowSearchEventsHandler, ISearchCallback
    {
        private uint _searchCookie;

        public ISearchQuery SearchQuery => new SearchQuery(SearchString);

        public ISearchTask SearchTask { get; private set; }

        public bool IsVisible { get; set; }

        public bool IsEnabled { get; set; }

        public SearchControl SearchControl { get; }

        private SearchControlDataSource DataSource { get; set; }

        internal object SearchParentControl { get; }

        private IWindowSearch WindowSearch { get; set; }

        private string SearchString
        {
            get
            {
                if (DataSource != null)
                    return DataSource.SearchText;
                return string.Empty;
            }
        }

        public WindowSearchHost(FrameworkElement parentControl)
        {
            if (parentControl == null)
                throw new ArgumentNullException(nameof(parentControl));
            SearchControl = new SearchControl();
            WindowHelper.CreateChildElement(SearchControl, parentControl);
            SearchParentControl = parentControl;
        }

        public void SetupSearch(IWindowSearch windowSearch)
        {
            ThrowIfDisposed();
            if (windowSearch == null)
                throw new ArgumentNullException(nameof(windowSearch));

            Caliburn.Micro.Execute.OnUIThread(() =>
            {
                if (WindowSearch != null)
                    throw new InvalidOperationException("WindowSearchHost.cs -- Already Setted up");
                WindowSearch = windowSearch;
                CreateDataSource();
                WindowSearch.ProvideSearchSettings(DataSource.SearchSettings);
                SearchControl.DataContext = DataSource;
            });
        }

        private void CreateDataSource()
        {
            DataSource = new WindowSearchDataSource(this);
        }

        public void TerminateSearch()
        {
            if (IsDisposed)
                return;
            Caliburn.Micro.Execute.OnUIThread(() =>
            {
                if (WindowSearch == null)
                    return;
                SearchTask = null;
                _searchCookie = 0;
                DataSource = null;
                if (SearchControl != null)
                    SearchControl.DataContext = null;
                WindowSearch = null;
            });
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

        public void OnStartSearch(string searchText)
        {
            ThrowIfDisposedOrSearchNotSetup();
            if (searchText == null || DataSource.SearchSettings.SearchTrimsWhitespaces && string.IsNullOrWhiteSpace(searchText))
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

        public void OnClearSearch()
        {
            ThrowIfDisposedOrSearchNotSetup();
            WindowSearch.ClearSearch();
        }

        public void OnDeleteMruItem(SearchMruItem item)
        {
            ThrowIfDisposedOrSearchNotSetup();
        }

        public void OnSelectMruItem(SearchMruItem item)
        {
            ThrowIfDisposedOrSearchNotSetup();
        }

        public void OnPopulateMruItems(string searchPrefix)
        {
            ThrowIfDisposedOrSearchNotSetup();
        }

        public void OnAddMruItem(string searchedText)
        {
            ThrowIfDisposedOrSearchNotSetup();
        }

        public bool OnNotifyNavigationKey(SearchNavigationKeys key, UIAccelModifiers modifiers)
        {
            ThrowIfDisposedOrSearchNotSetup();
            return WindowSearch.OnNavigationKeyDown((uint)key, (uint)modifiers);
        }

        protected override void DisposeManagedResources()
        {
            TerminateSearch();
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
                search.Start();

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

        public void ReportProgress(ISearchTask task, uint progress, uint maxProgress)
        {
            if (maxProgress == 0)
                throw new ArgumentException("WindowSearchHowst.cs -- maxProgress is 0");
            Caliburn.Micro.Execute.OnUIThread(() =>
            {
                if (task == SearchTask)
                    return;
                DataSource.SearchProgress = (int)(progress * 100.0 / maxProgress);
            });
        }

        public void ReportComplete(ISearchTask task, uint resultsFound)
        {
            Caliburn.Micro.Execute.OnUIThread(() =>
            {
                if (task == SearchTask)
                {
                    DataSource.SearchStatus = SearchStatus.Complete;
                    SearchTask = null;
                }
                if ((int)resultsFound < 0 || DataSource == null)
                    return;
                if (DataSource.SearchResultsCount == -1)
                    DataSource.SearchResultsCount = (int)resultsFound;
                else
                    DataSource.SearchResultsCount += (int)resultsFound;
            });
        }

        public void SearchOptionsChanged()
        {
            ThrowIfDisposed();
            //TODO:
        }
    }
}

