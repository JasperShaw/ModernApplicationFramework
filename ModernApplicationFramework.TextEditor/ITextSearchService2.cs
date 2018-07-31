using System;
using System.Collections.Generic;

namespace ModernApplicationFramework.TextEditor
{
    public interface ITextSearchService2 : ITextSearchService
    {
        SnapshotSpan? Find(SnapshotPoint startingPosition, string searchPattern, FindOptions options);

        SnapshotSpan? Find(SnapshotSpan searchRange, SnapshotPoint startingPosition, string searchPattern, FindOptions options);

        SnapshotSpan? FindForReplace(SnapshotPoint startingPosition, string searchPattern, string replacePattern, FindOptions options, out string expandedReplacePattern);

        SnapshotSpan? FindForReplace(SnapshotSpan searchRange, string searchPattern, string replacePattern, FindOptions options, out string expandedReplacePattern);

        IEnumerable<SnapshotSpan> FindAll(SnapshotSpan searchRange, string searchPattern, FindOptions options);

        IEnumerable<SnapshotSpan> FindAll(SnapshotSpan searchRange, SnapshotPoint startingPosition, string searchPattern, FindOptions options);

        IEnumerable<Tuple<SnapshotSpan, string>> FindAllForReplace(SnapshotSpan searchRange, string searchPattern, string replacePattern, FindOptions options);
    }
}