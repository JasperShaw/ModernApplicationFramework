using System.Collections.ObjectModel;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Differencing;
using ModernApplicationFramework.TextEditor.Text;

namespace ModernApplicationFramework.TextEditor
{
    public class ProjectionSpanDifference
    {
        public ProjectionSpanDifference(IDifferenceCollection<SnapshotSpan> differenceCollection, ReadOnlyCollection<SnapshotSpan> insertedSpans, ReadOnlyCollection<SnapshotSpan> deletedSpans)
        {
            DifferenceCollection = differenceCollection;
            InsertedSpans = insertedSpans;
            DeletedSpans = deletedSpans;
        }

        public IDifferenceCollection<SnapshotSpan> DifferenceCollection { get; }

        public ReadOnlyCollection<SnapshotSpan> InsertedSpans { get; }

        public ReadOnlyCollection<SnapshotSpan> DeletedSpans { get; }
    }
}