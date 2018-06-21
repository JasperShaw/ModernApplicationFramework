using ModernApplicationFramework.Controls.SearchControl;
using ModernApplicationFramework.Interfaces.Search;

namespace ModernApplicationFramework.Basics.Search.Internal
{
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
}