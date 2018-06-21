using System;
using ModernApplicationFramework.Interfaces.Search;

namespace ModernApplicationFramework.Basics.Search
{
    public class SearchTask : ISearchTask
    {
        private readonly object _syncObj = new object();

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
            if (cookie == 0)
                throw new ArgumentException(nameof(cookie));
            if (query.SearchString == null || string.IsNullOrEmpty(query.SearchString))
                throw new ArgumentException("Empty search string");
            Id = cookie;
            SearchQuery = query;
            SearchCallback = callback ?? throw new ArgumentNullException(nameof(callback));
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
            lock (_syncObj)
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
            lock (_syncObj)
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
}