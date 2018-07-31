using System.Collections.Generic;

namespace ModernApplicationFramework.TextEditor
{
    public interface ITextViewConnectionListener
    {
        void SubjectBuffersConnected(ITextView textView, ConnectionReason reason, IReadOnlyCollection<ITextBuffer> subjectBuffers);

        void SubjectBuffersDisconnected(ITextView textView, ConnectionReason reason, IReadOnlyCollection<ITextBuffer> subjectBuffers);
    }
}