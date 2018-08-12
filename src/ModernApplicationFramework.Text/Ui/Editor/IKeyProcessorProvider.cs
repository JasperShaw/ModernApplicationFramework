namespace ModernApplicationFramework.Text.Ui.Editor
{
    public interface IKeyProcessorProvider
    {
        KeyProcessor GetAssociatedProcessor(ITextView wpfTextView);
    }
}