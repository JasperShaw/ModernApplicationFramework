using System;
using System.Collections.Generic;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.TextEditor
{
    public interface IOverviewMarkMarginTest
    {
        IList<Tuple<string, NormalizedSnapshotSpanCollection, int>> GetMarks();
    }
}