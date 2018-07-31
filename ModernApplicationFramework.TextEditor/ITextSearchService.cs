using System.Collections.ObjectModel;

namespace ModernApplicationFramework.TextEditor
{
    public interface ITextSearchService
    {
        SnapshotSpan? FindNext(int startIndex, bool wraparound, FindData findData);

        Collection<SnapshotSpan> FindAll(FindData findData);
    }
}