namespace ModernApplicationFramework.TextEditor
{
    public interface IKeyProcessorProvider
    {
        KeyProcessor GetAssociatedProcessor(ITextView wpfTextView);
    }
}