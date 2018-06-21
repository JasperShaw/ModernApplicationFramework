using ModernApplicationFramework.Controls.SearchControl;

namespace ModernApplicationFramework.Interfaces.Search
{
    public interface IWindowSearchEventsHandler
    {
        void OnStartSearch(string searchText);

        void OnStopSearch();

        void OnClearSearch();

        void OnDeleteMruItem(SearchMruItem item);

        void OnSelectMruItem(SearchMruItem item);

        void OnPopulateMruItems(string searchPrefix);

        void OnAddMruItem(string searchedText);

        bool OnNotifyNavigationKey(SearchNavigationKeys key, UIAccelModifiers modifiers);
    }
}