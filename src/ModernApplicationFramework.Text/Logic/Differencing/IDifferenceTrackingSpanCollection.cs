using System.Collections.Generic;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Logic.Differencing
{
    public interface IDifferenceTrackingSpanCollection
    {
        IEnumerable<ITrackingSpan> RemovedLineSpans { get; }

        IEnumerable<ITrackingSpan> RemovedWordSpans { get; }

        IEnumerable<ITrackingSpan> AddedLineSpans { get; }

        IEnumerable<ITrackingSpan> AddedWordSpans { get; }
    }
}