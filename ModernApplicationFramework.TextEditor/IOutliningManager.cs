using System;
using System.Collections.Generic;

namespace ModernApplicationFramework.TextEditor
{
    public interface IOutliningManager : IDisposable
    {
        IEnumerable<ICollapsed> GetCollapsedRegions(SnapshotSpan span);

        IEnumerable<ICollapsed> GetCollapsedRegions(SnapshotSpan span, bool exposedRegionsOnly);

        IEnumerable<ICollapsed> GetCollapsedRegions(NormalizedSnapshotSpanCollection spans);

        IEnumerable<ICollapsed> GetCollapsedRegions(NormalizedSnapshotSpanCollection spans, bool exposedRegionsOnly);

        IEnumerable<ICollapsible> GetAllRegions(SnapshotSpan span);

        IEnumerable<ICollapsible> GetAllRegions(SnapshotSpan span, bool exposedRegionsOnly);

        IEnumerable<ICollapsible> GetAllRegions(NormalizedSnapshotSpanCollection spans);

        IEnumerable<ICollapsible> GetAllRegions(NormalizedSnapshotSpanCollection spans, bool exposedRegionsOnly);

        event EventHandler<RegionsChangedEventArgs> RegionsChanged;

        event EventHandler<RegionsExpandedEventArgs> RegionsExpanded;

        event EventHandler<RegionsCollapsedEventArgs> RegionsCollapsed;

        event EventHandler<OutliningEnabledEventArgs> OutliningEnabledChanged;

        ICollapsible Expand(ICollapsed collapsible);

        ICollapsed TryCollapse(ICollapsible collapsible);

        IEnumerable<ICollapsed> CollapseAll(SnapshotSpan span, Predicate<ICollapsible> match);

        IEnumerable<ICollapsible> ExpandAll(SnapshotSpan span, Predicate<ICollapsed> match);

        bool Enabled { get; set; }
    }
}