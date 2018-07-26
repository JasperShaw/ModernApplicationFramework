namespace ModernApplicationFramework.TextEditor
{
    public interface ITagSpan<out T> where T : ITag
    {
        T Tag { get; }

        SnapshotSpan Span { get; }
    }
}