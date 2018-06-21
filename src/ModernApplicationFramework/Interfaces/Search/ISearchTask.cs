namespace ModernApplicationFramework.Interfaces.Search
{
    public interface ISearchTask
    {
        void Start();

        void Stop();

        ISearchQuery SearchQuery { get; }

        uint Status { get; }

        int ErrorCode { get; }
    }
}