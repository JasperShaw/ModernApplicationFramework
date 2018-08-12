using ModernApplicationFramework.Editor.Implementation;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Editor
{
    public interface IEditorAdaptersFactoryService
    {
        IMafTextBuffer CreateTextBufferAdapter();

        IMafTextBuffer CreateTextBufferAdapter(IContentType contentType);

        IMafTextBuffer CreateTextBufferAdapterForSecondaryBuffer(ITextBuffer secondaryBuffer);

        IMafTextView CreateTextViewAdapter();

        IMafTextView CreateTextViewAdapter(ITextViewRoleSet roles);

        //TODO: Activate Code Window
        //IMafCodeWindow CreateVsCodeWindowAdapter();

        //IMafTextBufferCoordinator CreateVsTextBufferCoordinatorAdapter();

        ITextBuffer GetDataBuffer(IMafTextBuffer bufferAdapter);

        ITextBuffer GetDocumentBuffer(IMafTextBuffer bufferAdapter);

        ITextView GetTextView(IMafTextView viewAdapter);

        ITextViewHost GetTextViewHost(IMafTextView viewAdapter);

        void SetDataBuffer(IMafTextBuffer bufferAdapter, ITextBuffer dataBuffer);

        IMafTextBuffer GetBufferAdapter(ITextBuffer textBuffer);

        IMafTextView GetViewAdapter(ITextView textView);
    }
}