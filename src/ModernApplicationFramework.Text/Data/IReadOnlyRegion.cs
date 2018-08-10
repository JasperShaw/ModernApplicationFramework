namespace ModernApplicationFramework.Text.Data
{
    public interface IReadOnlyRegion
    {
        EdgeInsertionMode EdgeInsertionMode { get; }

        DynamicReadOnlyRegionQuery QueryCallback { get; }

        ITrackingSpan Span { get; }
    }
}