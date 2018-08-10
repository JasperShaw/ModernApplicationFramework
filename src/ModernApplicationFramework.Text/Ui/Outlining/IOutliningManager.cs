using System;
using System.Collections.Generic;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Ui.Outlining
{
    public interface IOutliningManager : IDisposable
    {
        event EventHandler<OutliningEnabledEventArgs> OutliningEnabledChanged;

        event EventHandler<RegionsChangedEventArgs> RegionsChanged;

        event EventHandler<RegionsCollapsedEventArgs> RegionsCollapsed;

        event EventHandler<RegionsExpandedEventArgs> RegionsExpanded;

        bool Enabled { get; set; }

        IEnumerable<ICollapsed> CollapseAll(SnapshotSpan span, Predicate<ICollapsible> match);

        ICollapsible Expand(ICollapsed collapsible);

        IEnumerable<ICollapsible> ExpandAll(SnapshotSpan span, Predicate<ICollapsed> match);

        IEnumerable<ICollapsible> GetAllRegions(SnapshotSpan span);

        IEnumerable<ICollapsible> GetAllRegions(SnapshotSpan span, bool exposedRegionsOnly);

        IEnumerable<ICollapsible> GetAllRegions(NormalizedSnapshotSpanCollection spans);

        IEnumerable<ICollapsible> GetAllRegions(NormalizedSnapshotSpanCollection spans, bool exposedRegionsOnly);
        IEnumerable<ICollapsed> GetCollapsedRegions(SnapshotSpan span);

        IEnumerable<ICollapsed> GetCollapsedRegions(SnapshotSpan span, bool exposedRegionsOnly);

        IEnumerable<ICollapsed> GetCollapsedRegions(NormalizedSnapshotSpanCollection spans);

        IEnumerable<ICollapsed> GetCollapsedRegions(NormalizedSnapshotSpanCollection spans, bool exposedRegionsOnly);

        ICollapsed TryCollapse(ICollapsible collapsible);
    }
}