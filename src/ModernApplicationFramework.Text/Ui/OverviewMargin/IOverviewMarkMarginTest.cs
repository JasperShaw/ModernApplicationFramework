using System;
using System.Collections.Generic;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Ui.OverviewMargin
{
    public interface IOverviewMarkMarginTest
    {
        IList<Tuple<string, NormalizedSnapshotSpanCollection, int>> GetMarks();
    }
}