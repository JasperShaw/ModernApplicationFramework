namespace ModernApplicationFramework.TextEditor
{
    public interface ISequenceElement
    {
        IMappingSpan Span { get; }

        bool ShouldRenderText { get; }
    }
}