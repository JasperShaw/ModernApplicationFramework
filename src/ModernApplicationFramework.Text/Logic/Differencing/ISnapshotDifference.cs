using System.Collections.Generic;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Differencing;
using ModernApplicationFramework.Text.Data.Projection;
using ModernApplicationFramework.Text.Ui.Differencing;

namespace ModernApplicationFramework.Text.Logic.Differencing
{
    public interface ISnapshotDifference
    {
        IDifferenceBuffer DifferenceBuffer { get; }

        ITextSnapshot LeftBufferSnapshot { get; }

        ITextSnapshot RightBufferSnapshot { get; }

        IProjectionSnapshot InlineBufferSnapshot { get; }

        StringDifferenceOptions DifferenceOptions { get; }

        IEnumerable<SnapshotLineTransform> SnapshotLineTransforms { get; }

        IEnumerable<IgnoreDifferencePredicate> IgnoreDifferencePredicates { get; }

        IHierarchicalDifferenceCollection LineDifferences { get; }

        IDifferenceTrackingSpanCollection DifferenceSpans { get; }

        SnapshotPoint MapToInlineSnapshot(SnapshotPoint point);

        int FindMatchOrDifference(SnapshotPoint point, out Match match, out Difference difference);

        SnapshotPoint TranslateToSnapshot(SnapshotPoint point);

        SnapshotPoint MapToSourceSnapshot(SnapshotPoint inlinePoint);

        SnapshotPoint MapToSnapshot(SnapshotPoint point, ITextSnapshot target, DifferenceMappingMode mode = DifferenceMappingMode.LineColumn);

        SnapshotSpan MapToSnapshot(Difference difference, ITextSnapshot target);
    }
}
