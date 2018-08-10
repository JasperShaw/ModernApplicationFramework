using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.TextEditor
{
    public interface IKeyProcessorProvider
    {
        KeyProcessor GetAssociatedProcessor(ITextView wpfTextView);
    }
}