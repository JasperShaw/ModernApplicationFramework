using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Text.Ui.Differencing
{
    public interface IDifferenceViewerFactoryService
    {
        IDifferenceViewer CreateDifferenceView(IDifferenceBuffer buffer, IEditorOptions parentOptions = null);

        IDifferenceViewer CreateDifferenceView(IDifferenceBuffer buffer, ITextViewRoleSet roles, IEditorOptions parentOptions = null);

        IDifferenceViewer CreateDifferenceView(IDifferenceBuffer buffer, CreateTextViewHostCallback callback, IEditorOptions parentOptions = null);

        IDifferenceViewer CreateUninitializedDifferenceView();

        IDifferenceViewer TryGetViewerForTextView(ITextView textView);
    }
}