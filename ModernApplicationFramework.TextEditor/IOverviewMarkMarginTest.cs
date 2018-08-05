using System;
using System.Collections.Generic;

namespace ModernApplicationFramework.TextEditor
{
    public interface IOverviewMarkMarginTest
    {
        IList<Tuple<string, NormalizedSnapshotSpanCollection, int>> GetMarks();
    }
}