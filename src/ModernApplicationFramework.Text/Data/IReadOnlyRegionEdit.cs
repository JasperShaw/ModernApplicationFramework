namespace ModernApplicationFramework.Text.Data
{
    public interface IReadOnlyRegionEdit : ITextBufferEdit
    {
        IReadOnlyRegion CreateDynamicReadOnlyRegion(Span span, SpanTrackingMode trackingMode,
            EdgeInsertionMode edgeInsertionMode, DynamicReadOnlyRegionQuery callback);

        IReadOnlyRegion CreateReadOnlyRegion(Span span);

        IReadOnlyRegion CreateReadOnlyRegion(Span span, SpanTrackingMode trackingMode,
            EdgeInsertionMode edgeInsertionMode);

        void RemoveReadOnlyRegion(IReadOnlyRegion readOnlyRegion);
    }
}