using System;
using System.Windows;
using System.Windows.Controls;
using ModernApplicationFramework.Core.InfoBarUtilities;
using ModernApplicationFramework.Native;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Controls.SearchControl
{
    public class WindowSearchHost : DisposableObject, IWindowSearchHost, IWindowSearchHostPrivate, IWindowSearchEvents, IWindowSearchEventsHandler, ISearchCallback
    {
        private static HybridDictionary<IntPtr, WindowSearchHost> _hwndToInstanceMap = new HybridDictionary<IntPtr, WindowSearchHost>();
        private uint _searchCookie;

        public ISearchQuery SearchQuery => new SearchQuery(SearchString);

        public ISearchTask SearchTask { get; private set; }

        public bool IsVisible { get; set; }

        public bool IsEnabled { get; set; }

        public SearchControl SearchControl { get; }

        private SearchControlDataSource DataSource { get; set; }

        internal object SearchParentControl { get; private set; }

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
    }

    internal class SearchQuery : ISearchQuery
    {
        public string SearchString { get; }

        public SearchQuery(string searchString)
        {
            SearchString = searchString;
        }
    }

    public interface IWindowSearchEventsHandler
    {
        void OnStartSearch(string searchText);

        void OnStopSearch();

        void OnClearSearch();

        bool OnNotifyNavigationKey(SearchNavigationKeys key, UIAccelModifiers modifiers);
    }

    public interface IWindowSearchEvents
    {
        //void SearchOptionsChanged();
    }

    public interface IWindowSearchHostPrivate
    {
        SearchControl SearchControl { get; }
    }

    public interface IWindowSearchHost
    {
        ISearchQuery SearchQuery { get; }

        ISearchTask SearchTask { get; }

        bool IsVisible { get; set; }
        bool IsEnabled { get; set; }

        void SetupSearch(IWindowSearch windowSearch);

        void TerminateSearch();

        void Activate();
    }


    //This is a ITool for example
    public interface IWindowSearch
    {
        bool SearchEnabled { get; }

        ISearchTask CreateSearch(uint cookie, ISearchQuery searchQuery, ISearchCallback searchCallback);

        void ClearSearch();

        void ProvideSearchSettings(SearchSettingsDataSource dataSource);

        bool OnNavigationKeyDown(uint navigationKey, uint modifiers);
    }

    public interface ISearchCallback
    {
        void ReportProgress(ISearchTask task, uint progress, uint maxProgress);

        void ReportComplete(ISearchTask task, uint resultsFound);

    }

    public interface ISearchTask
    {
        void Start();

        void Stop();

        ISearchQuery SearchQuery { get; }

        uint Status { get; }

        int ErrorCode { get; }
    }

    public interface ISearchQuery
    {
        string SearchString { get; }
    }

    internal class WindowSearchDataSource : SearchControlDataSource
    {
        private IWindowSearchEventsHandler EventsHandler { get; }

        public WindowSearchDataSource(IWindowSearchEventsHandler eventsHandler)
        {
            EventsHandler = eventsHandler;
            SearchSettings.ControlMaxWidth = 200;
        }

        protected override void OnStartSearch(string searchText)
        {
            EventsHandler.OnStartSearch(searchText);
        }

        protected override void OnStopSearch()
        {
            EventsHandler.OnStopSearch();
        }

        protected override void OnClearSearch()
        {
            EventsHandler.OnClearSearch();
        }

        protected override bool OnNotifyNavigationKey(SearchNavigationKeys searchNavigationKeys, UIAccelModifiers uiAccelModifiers)
        {
            return EventsHandler.OnNotifyNavigationKey(searchNavigationKeys, uiAccelModifiers);
        }
    }

    public class SearchTask : ISearchTask
    {
        private object syncObj = new object();

        public ISearchQuery SearchQuery { get; }
        public uint Status => (uint) TaskStatus;
        public int ErrorCode { get; }

        public SearchTaskStatus TaskStatus { get; private set; }

        protected ISearchCallback SearchCallback { get; private set; }

        public uint SearchResults { get; protected set; }

        public uint Id { get; private set; }

        public SearchTask(uint cookie, ISearchQuery query, ISearchCallback callback)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));
            if (cookie == 0)
                throw new ArgumentException(nameof(cookie));
            if (query.SearchString == null || string.IsNullOrEmpty(query.SearchString))
                throw new ArgumentException("Empty search string");
            Id = cookie;
            SearchQuery = query;
            SearchCallback = callback;
            ErrorCode = 0;
            TaskStatus = SearchTaskStatus.Created;

        }

        public void Start()
        {
            if (!SetTaskStatus(SearchTaskStatus.Started))
                return;
            OnStartSearch();
        }

        public void Stop()
        {
            lock (syncObj)
            {
                if (!SetTaskStatus(SearchTaskStatus.Stopped))
                    return;
                OnStopSearch();
                SearchCallback.ReportComplete(this, SearchResults);
            }
        }

        protected virtual void OnStartSearch()
        {
            if (!SetTaskStatus(SearchTaskStatus.Completed))
                return;
            SearchCallback.ReportComplete(this, SearchResults);
        }

        protected virtual void OnStopSearch()
        {
        }

        protected bool SetTaskStatus(SearchTaskStatus status)
        {
            lock (syncObj)
            {
                if (status == SearchTaskStatus.Started && TaskStatus != SearchTaskStatus.Created ||
                    status == SearchTaskStatus.Stopped && TaskStatus != SearchTaskStatus.Created &&
                    TaskStatus != SearchTaskStatus.Started || 
                    TaskStatus == SearchTaskStatus.Stopped ||
                    (status == SearchTaskStatus.Completed || status == SearchTaskStatus.Error) &&
                    TaskStatus != SearchTaskStatus.Started)
                    return false;
                TaskStatus = status;
                return true;
            }
        }
    }

    public enum SearchTaskStatus
    {
        Created,
        Started,
        Completed,
        Stopped,
        Error,
    }

    public enum SearchPlacement
    {
        None,
        Dynamic,
        Stretch,
        Custom
    }
}

