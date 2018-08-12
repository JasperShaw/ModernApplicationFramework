namespace ModernApplicationFramework.Text.Ui.Editor
{
    public interface IMouseProcessorProvider
    {
        IMouseProcessor GetAssociatedProcessor(ITextView wpfTextView);
    }
}