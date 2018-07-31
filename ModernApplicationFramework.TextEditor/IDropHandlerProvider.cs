namespace ModernApplicationFramework.TextEditor
{
    public interface IDropHandlerProvider
    {
        IDropHandler GetAssociatedDropHandler(ITextView wpfTextView);
    }
}