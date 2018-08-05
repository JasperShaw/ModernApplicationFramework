using System.Collections.Generic;
using ModernApplicationFramework.TextEditor.Text.Document;

namespace ModernApplicationFramework.TextEditor.Utilities
{
    internal static class ChangeBrushes
    {
        public static NormalizedSnapshotSpanCollection[] GetUnifiedChanges(ITextSnapshot snapshot, IEnumerable<IMappingTagSpan<ChangeTag>> tags)
        {
            List<Span>[] spanListArray = {
                null,
                new List<Span>(),
                new List<Span>(),
                new List<Span>()
            };
            foreach (var tag in tags)
            {
                var index = (int)(tag.Tag.ChangeTypes & (ChangeTypes.ChangedSinceOpened | ChangeTypes.ChangedSinceSaved));
                if (index != 0)
                    spanListArray[index].AddRange((NormalizedSpanCollection)tag.Span.GetSpans(snapshot));
            }
            var snapshotSpanCollectionArray = new NormalizedSnapshotSpanCollection[4];
            for (var index = 1; index <= 3; ++index)
                snapshotSpanCollectionArray[index] = new NormalizedSnapshotSpanCollection(snapshot, spanListArray[index]);
            return snapshotSpanCollectionArray;
        }
    }
}