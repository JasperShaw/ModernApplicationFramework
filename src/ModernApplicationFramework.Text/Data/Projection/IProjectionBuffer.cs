using System;
using System.Collections.Generic;

namespace ModernApplicationFramework.Text.Data.Projection
{
    public interface IProjectionBuffer : IProjectionBufferBase
    {
        event EventHandler<ProjectionSourceBuffersChangedEventArgs> SourceBuffersChanged;

        event EventHandler<ProjectionSourceSpansChangedEventArgs> SourceSpansChanged;

        IProjectionSnapshot DeleteSpans(int position, int spansToDelete);
        IProjectionSnapshot InsertSpan(int position, ITrackingSpan spanToInsert);

        IProjectionSnapshot InsertSpan(int position, string literalSpanToInsert);

        IProjectionSnapshot InsertSpans(int position, IList<object> spansToInsert);

        IProjectionSnapshot ReplaceSpans(int position, int spansToReplace, IList<object> spansToInsert,
            EditOptions options, object editTag);
    }
}