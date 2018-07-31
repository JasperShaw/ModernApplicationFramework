using System;
using System.Collections.Generic;
using System.Threading;

namespace ModernApplicationFramework.TextEditor
{
    public interface IAccurateOutliningManager : IOutliningManager
    {
        IEnumerable<ICollapsed> CollapseAll(SnapshotSpan span, Predicate<ICollapsible> match, CancellationToken cancel);
    }
}