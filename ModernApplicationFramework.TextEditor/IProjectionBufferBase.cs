using System.Collections.Generic;

namespace ModernApplicationFramework.TextEditor
{
    public interface IProjectionBufferBase : ITextBuffer
    {
        new IProjectionSnapshot CurrentSnapshot { get; }

        IList<ITextBuffer> SourceBuffers { get; }

        new IProjectionSnapshot Insert(int position, string text);

        new IProjectionSnapshot Delete(Span deleteSpan);

        new IProjectionSnapshot Replace(Span replaceSpan, string replaceWith);
    }
}