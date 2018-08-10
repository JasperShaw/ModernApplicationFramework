namespace ModernApplicationFramework.Text.Data
{
    public delegate Span CustomTrackToVersion(ITrackingSpan customSpan, ITextVersion currentVersion, ITextVersion targetVersion, Span currentSpan, object customState);
}