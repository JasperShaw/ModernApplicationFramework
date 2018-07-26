namespace ModernApplicationFramework.TextEditor
{
    public interface IMappingTagSpan<out T> where T : ITag
    {
        T Tag { get; }

        IMappingSpan Span { get; }
    }
}