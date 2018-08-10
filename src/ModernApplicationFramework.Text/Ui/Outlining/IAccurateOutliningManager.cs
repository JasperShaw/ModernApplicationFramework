using System;
using System.Collections.Generic;
using System.Threading;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Ui.Outlining
{
    public interface IAccurateOutliningManager : IOutliningManager
    {
        IEnumerable<ICollapsed> CollapseAll(SnapshotSpan span, Predicate<ICollapsible> match, CancellationToken cancel);
    }
}