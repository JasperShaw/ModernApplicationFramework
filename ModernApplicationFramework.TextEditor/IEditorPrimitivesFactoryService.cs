using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.TextEditor
{
    public interface IEditorPrimitivesFactoryService
    {
        IViewPrimitives GetViewPrimitives(ITextView textView);

        IBufferPrimitives GetBufferPrimitives(ITextBuffer textBuffer);
    }
}