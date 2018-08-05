namespace ModernApplicationFramework.TextEditor.Implementation
{
    public interface IScrollMapFactoryService
    {
        IScrollMap Create(ITextView textView);

        IScrollMap Create(ITextView textView, bool areElisionsExpanded);
    }
}