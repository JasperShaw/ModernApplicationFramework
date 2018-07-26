namespace ModernApplicationFramework.TextEditor
{
    public interface IReadOnlyRegionEdit : ITextBufferEdit
    {
        IReadOnlyRegion CreateReadOnlyRegion(Span span);

        IReadOnlyRegion CreateReadOnlyRegion(Span span, SpanTrackingMode trackingMode, EdgeInsertionMode edgeInsertionMode);

        IReadOnlyRegion CreateDynamicReadOnlyRegion(Span span, SpanTrackingMode trackingMode, EdgeInsertionMode edgeInsertionMode, DynamicReadOnlyRegionQuery callback);

        void RemoveReadOnlyRegion(IReadOnlyRegion readOnlyRegion);
    }
}