namespace ModernApplicationFramework.Text.Utilities
{
    public struct ProgressInfo
    {
        public int CompletedItems { get; }

        public int TotalItems { get; }

        public ProgressInfo(int completedItems, int totalItems)
        {
            CompletedItems = completedItems;
            TotalItems = totalItems;
        }
    }
}