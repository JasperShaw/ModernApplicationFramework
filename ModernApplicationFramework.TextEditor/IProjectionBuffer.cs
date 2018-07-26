using System;
using System.Collections.Generic;

namespace ModernApplicationFramework.TextEditor
{
    public interface IProjectionBuffer : IProjectionBufferBase
    {
        IProjectionSnapshot InsertSpan(int position, ITrackingSpan spanToInsert);

        IProjectionSnapshot InsertSpan(int position, string literalSpanToInsert);

        IProjectionSnapshot InsertSpans(int position, IList<object> spansToInsert);

        IProjectionSnapshot DeleteSpans(int position, int spansToDelete);

        IProjectionSnapshot ReplaceSpans(int position, int spansToReplace, IList<object> spansToInsert, EditOptions options, object editTag);

        event EventHandler<ProjectionSourceSpansChangedEventArgs> SourceSpansChanged;

        event EventHandler<ProjectionSourceBuffersChangedEventArgs> SourceBuffersChanged;
    }
}