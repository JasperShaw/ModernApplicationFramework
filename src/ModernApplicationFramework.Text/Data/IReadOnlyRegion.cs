namespace ModernApplicationFramework.Text.Data
{
    public interface IReadOnlyRegion
    {
        EdgeInsertionMode EdgeInsertionMode { get; }

        ITrackingSpan Span { get; }

        DynamicReadOnlyRegionQuery QueryCallback { get; }
    }
}