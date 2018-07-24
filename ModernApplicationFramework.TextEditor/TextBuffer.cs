using ModernApplicationFramework.TextEditor.Text;
using ModernApplicationFramework.TextEditor.Utilities;

namespace ModernApplicationFramework.TextEditor
{
    internal sealed class TextBuffer : BaseBuffer
    {

        public TextBuffer(IContentType contentType, StringRebuilder content, ITextDifferencingService textDifferencingService, 
            GuardedOperations guardedOperations, bool spurnGroup)
            : base(contentType, content.Length, textDifferencingService, guardedOperations)
        {
        }

    }

    internal abstract class BaseBuffer : ITextBuffer
    {
        protected BaseBuffer(IContentType contentType, int initialLength, ITextDifferencingService textDifferencingService,
            GuardedOperations guardedOperations)
        {
        }

        public IContentType ContentType { get; }

        public ITextSnapshot CurrentSnapshot { get; }
    }
}
