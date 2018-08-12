using System.Collections.ObjectModel;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Differencing;

namespace ModernApplicationFramework.Modules.Editor.Utilities
{
    public class ProjectionSpanDifference
    {
        public ReadOnlyCollection<SnapshotSpan> DeletedSpans { get; }

        public IDifferenceCollection<SnapshotSpan> DifferenceCollection { get; }

        public ReadOnlyCollection<SnapshotSpan> InsertedSpans { get; }

        public ProjectionSpanDifference(IDifferenceCollection<SnapshotSpan> differenceCollection,
            ReadOnlyCollection<SnapshotSpan> insertedSpans, ReadOnlyCollection<SnapshotSpan> deletedSpans)
        {
            DifferenceCollection = differenceCollection;
            InsertedSpans = insertedSpans;
            DeletedSpans = deletedSpans;
        }
    }
}