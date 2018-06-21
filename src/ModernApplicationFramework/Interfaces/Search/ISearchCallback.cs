namespace ModernApplicationFramework.Interfaces.Search
{
    public interface ISearchCallback
    {
        void ReportProgress(ISearchTask task, uint progress, uint maxProgress);

        void ReportComplete(ISearchTask task, uint resultsFound);

    }
}