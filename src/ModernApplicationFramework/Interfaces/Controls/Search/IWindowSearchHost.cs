using ModernApplicationFramework.Interfaces.Search;

namespace ModernApplicationFramework.Interfaces.Controls.Search
{
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
}