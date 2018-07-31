namespace ModernApplicationFramework.TextEditor
{
    public interface IMouseProcessorProvider
    {
        IMouseProcessor GetAssociatedProcessor(ITextView wpfTextView);
    }
}