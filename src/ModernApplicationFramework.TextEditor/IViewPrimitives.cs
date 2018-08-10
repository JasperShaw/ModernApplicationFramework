using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.TextEditor
{
    public interface IViewPrimitives : IBufferPrimitives
    {
        PrimitiveTextView View { get; }

        Selection Selection { get; }

        Caret Caret { get; }
    }
}