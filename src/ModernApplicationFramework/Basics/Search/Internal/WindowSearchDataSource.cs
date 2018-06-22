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

        protected override bool OnNotifyNavigationKey(SearchNavigationKeys searchNavigationKeys, UiAccelModifiers uiAccelModifiers)
        {
            return EventsHandler.OnNotifyNavigationKey(searchNavigationKeys, uiAccelModifiers);
        }

        protected override void OnPopulateMruItem(string searchPrefix)
        {
            EventsHandler.OnPopulateMruItems(searchPrefix);
        }

        protected override void OnAddMruItem(string searchedText)
        {
            EventsHandler.OnAddMruItem(searchedText);
        }
    }
}