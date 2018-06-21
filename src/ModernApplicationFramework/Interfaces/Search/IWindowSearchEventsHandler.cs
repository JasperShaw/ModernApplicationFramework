using ModernApplicationFramework.Controls.SearchControl;

namespace ModernApplicationFramework.Interfaces.Search
{
    public interface IWindowSearchEventsHandler
    {
        void OnStartSearch(string searchText);

        void OnStopSearch();

        void OnClearSearch();

        bool OnNotifyNavigationKey(SearchNavigationKeys key, UIAccelModifiers modifiers);
    }
}