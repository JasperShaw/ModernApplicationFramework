using System.Collections.Generic;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Document;
using ModernApplicationFramework.Text.Logic.Tagging;

namespace ModernApplicationFramework.Modules.Editor.Utilities
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