using System.Collections.ObjectModel;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Logic.Operations
{
    public interface ITextSearchService
    {
        Collection<SnapshotSpan> FindAll(FindData findData);
        SnapshotSpan? FindNext(int startIndex, bool wraparound, FindData findData);
    }
}