using System;
using ModernApplicationFramework.Basics.Search;

namespace ModernApplicationFramework.Interfaces.Search
{
    public interface IWindowSearch
    {
        bool SearchEnabled { get; }

        SearchPlacement SearchControlPlacement { get; set; }

        Guid Category { get; }

        ISearchTask CreateSearch(uint cookie, ISearchQuery searchQuery, ISearchCallback searchCallback);

        void ClearSearch();

        void ProvideSearchSettings(SearchSettingsDataSource dataSource);

        bool OnNavigationKeyDown(uint navigationKey, uint modifiers);
    }
}