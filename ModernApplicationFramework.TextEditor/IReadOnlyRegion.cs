namespace ModernApplicationFramework.TextEditor
{
    public interface IReadOnlyRegion
    {
        EdgeInsertionMode EdgeInsertionMode { get; }

        ITrackingSpan Span { get; }

        DynamicReadOnlyRegionQuery QueryCallback { get; }
    }
}