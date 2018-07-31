namespace ModernApplicationFramework.TextEditor
{
    public interface ICollapsible
    {
        ITrackingSpan Extent { get; }

        bool IsCollapsed { get; }

        bool IsCollapsible { get; }

        object CollapsedForm { get; }

        object CollapsedHintForm { get; }

        IOutliningRegionTag Tag { get; }
    }
}