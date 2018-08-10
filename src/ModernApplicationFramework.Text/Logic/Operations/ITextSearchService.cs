using System.Collections.ObjectModel;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Logic.Operations
{
    public interface ITextSearchService
    {
        SnapshotSpan? FindNext(int startIndex, bool wraparound, FindData findData);

        Collection<SnapshotSpan> FindAll(FindData findData);
    }
}