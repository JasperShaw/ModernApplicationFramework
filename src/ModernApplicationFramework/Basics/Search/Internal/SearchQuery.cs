using ModernApplicationFramework.Interfaces.Search;

namespace ModernApplicationFramework.Basics.Search.Internal
{
    internal class SearchQuery : ISearchQuery
    {
        public string SearchString { get; }

        public SearchQuery(string searchString)
        {
            SearchString = searchString;
        }
    }
}