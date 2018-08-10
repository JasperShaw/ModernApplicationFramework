using System.Collections.Generic;

namespace ModernApplicationFramework.Text.Data.Projection
{
    public interface IProjectionBufferBase : ITextBuffer
    {
        new IProjectionSnapshot CurrentSnapshot { get; }

        IList<ITextBuffer> SourceBuffers { get; }

        new IProjectionSnapshot Delete(Span deleteSpan);

        new IProjectionSnapshot Insert(int position, string text);

        new IProjectionSnapshot Replace(Span replaceSpan, string replaceWith);
    }
}