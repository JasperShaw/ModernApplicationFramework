using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Logic.Tagging;
using ModernApplicationFramework.Text.Ui.Outlining;
using ModernApplicationFramework.Text.Ui.Tagging;
using ModernApplicationFramework.Text.Utilities;
using ModernApplicationFramework.TextEditor.Utilities;

namespace ModernApplicationFramework.TextEditor
{
    internal sealed class OutliningManager : IAccurateOutliningManager
    {
        private bool _isEnabled = true;
        private readonly ITextBuffer _editBuffer;
        private readonly IAccurateTagAggregator<IOutliningRegionTag> _tagAggregator;
        internal bool IsDisposed;
        private readonly TrackingSpanTree<Collapsed> _collapsedRegionTree;

        internal OutliningManager(ITextBuffer editBuffer, ITagAggregator<IOutliningRegionTag> tagAggregator, IEditorOptions options)
        {
            _editBuffer = editBuffer;
            _tagAggregator = tagAggregator as IAccurateTagAggregator<IOutliningRegionTag>;
            var keepTrackingCurrent = false;
            if (options != null && options.IsOptionDefined("Stress Test Mode", false))
                keepTrackingCurrent = options.GetOptionValue<bool>("Stress Test Mode");
            _collapsedRegionTree = new TrackingSpanTree<Collapsed>(editBuffer, keepTrackingCurrent);
            tagAggregator.BatchedTagsChanged += OutliningRegionTagsChanged;
            _editBuffer.Changed += SourceTextChanged;
        }

        public event EventHandler<RegionsChangedEventArgs> RegionsChanged;

        public event EventHandler<RegionsExpandedEventArgs> RegionsExpanded;

        public event EventHandler<RegionsCollapsedEventArgs> RegionsCollapsed;

        public event EventHandler<OutliningEnabledEventArgs> OutliningEnabledChanged;

        private void OutliningRegionTagsChanged(object sender, BatchedTagsChangedEventArgs e)
        {
            if (!_isEnabled)
                return;
            UpdateAfterChange(new NormalizedSnapshotSpanCollection(e.Spans.SelectMany(s => s.GetSpans(_editBuffer) as IEnumerable<SnapshotSpan>)));
        }

        private void SourceTextChanged(object sender, TextContentChangedEventArgs e)
        {
            if (!_isEnabled || e.Changes.Count <= 0)
                return;
            UpdateAfterChange(new NormalizedSnapshotSpanCollection(e.After, e.Changes.Select(c => c.NewSpan)));
            AvoidPartialLinebreaks(e);
        }

        private void AvoidPartialLinebreaks(TextContentChangedEventArgs args)
        {
            var before = args.Before;
            var flag = false;
            foreach (var change in args.Changes)
            {
                if (change.OldLength != 0)
                {
                    if (change.OldPosition > 0 && before[change.OldPosition] == '\n' && before[change.OldPosition - 1] == '\r')
                    {
                        flag = true;
                        break;
                    }
                    if (change.OldEnd > 0 && change.OldEnd < before.Length && (before[change.OldEnd] == '\n' && before[change.OldEnd - 1] == '\r'))
                    {
                        flag = true;
                        break;
                    }
                }
            }
            if (!flag)
                return;
            ExpandAll(new SnapshotSpan(args.After, Span.FromBounds(args.Changes[0].NewPosition, args.Changes[args.Changes.Count - 1].NewEnd)), collapsed => true);
        }

        private void UpdateAfterChange(NormalizedSnapshotSpanCollection changedSpans)
        {
            if (changedSpans.Count == 0)
                return;
            var collapsedRegionsInternal = GetCollapsedRegionsInternal(changedSpans, false);
            if (collapsedRegionsInternal.Count > 0)
            {
                var snapshot = changedSpans[0].Snapshot;
                var keys = CollapsiblesFromTags(_tagAggregator.GetTags(NormalizedSnapshotSpanCollection.Intersection(changedSpans, new NormalizedSnapshotSpanCollection(collapsedRegionsInternal.Select(c => c.Extent.GetSpan(snapshot)))))).Keys;
                MergeRegions(collapsedRegionsInternal, keys, out var removedRegions);
                var collapsibleList = new List<ICollapsible>();
                foreach (var collapsed in removedRegions)
                {
                    var collapsible = ExpandInternal(collapsed);
                    collapsibleList.Add(collapsible);
                }
                if (collapsibleList.Count > 0)
                {
                    var regionsExpanded = RegionsExpanded;
                    regionsExpanded?.Invoke(this, new RegionsExpandedEventArgs(collapsibleList, true));
                }
            }
            var regionsChanged = RegionsChanged;
            regionsChanged?.Invoke(this, new RegionsChangedEventArgs(new SnapshotSpan(changedSpans[0].Start, changedSpans[changedSpans.Count - 1].End)));
        }

        public ICollapsed TryCollapse(ICollapsible collapsible)
        {
            var element = CollapseInternal(collapsible);
            if (element == null)
                return null;
            var regionsCollapsed = RegionsCollapsed;
            regionsCollapsed?.Invoke(this, new RegionsCollapsedEventArgs(Enumerable.Repeat(element, 1)));
            return element;
        }

        private ICollapsed CollapseInternal(ICollapsible collapsible)
        {
            EnsureValid();
            if (collapsible.IsCollapsed)
                return null;
            var collapsed = new Collapsed(collapsible.Extent, collapsible.Tag);
            collapsed.Node = _collapsedRegionTree.TryAddItem(collapsed, collapsed.Extent);
            return collapsed.Node == null ? null : collapsed;
        }

        public ICollapsible Expand(ICollapsed collapsed)
        {
            var element = ExpandInternal(collapsed);
            var regionsExpanded = RegionsExpanded;
            regionsExpanded?.Invoke(this, new RegionsExpandedEventArgs(Enumerable.Repeat(element, 1)));
            return element;
        }

        private ICollapsible ExpandInternal(ICollapsed collapsed)
        {
            EnsureValid();
            var collapsed1 = collapsed as Collapsed;
            if (collapsed1 == null)
                throw new ArgumentException("The given collapsed region was not created by this outlining manager.", nameof(collapsed));
            if (!collapsed1.IsValid)
                throw new InvalidOperationException("The collapsed region is invalid, meaning it has already been expanded.");
            if (!_collapsedRegionTree.RemoveItem(collapsed1, collapsed1.Extent))
                throw new ApplicationException("Unable to remove the collapsed region from outlining manager, which means there is an internal consistency issue.");
            collapsed1.Invalidate();
            return new Collapsible(collapsed.Extent, collapsed.Tag);
        }

        public IEnumerable<ICollapsed> CollapseAll(SnapshotSpan span, Predicate<ICollapsible> match)
        {
            return InternalCollapseAll(span, match, new CancellationToken?());
        }

        internal IEnumerable<ICollapsed> InternalCollapseAll(SnapshotSpan span, Predicate<ICollapsible> match, CancellationToken? cancel)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));
            EnsureValid(span);
            var collapsedList = new List<ICollapsed>();
            foreach (var allRegion in InternalGetAllRegions(new NormalizedSnapshotSpanCollection(span), false, cancel))
            {
                if (!allRegion.IsCollapsed && allRegion.IsCollapsible && match(allRegion))
                {
                    var collapsed = CollapseInternal(allRegion);
                    if (collapsed != null)
                        collapsedList.Add(collapsed);
                }
            }
            if (collapsedList.Count > 0)
            {
                var regionsCollapsed = RegionsCollapsed;
                regionsCollapsed?.Invoke(this, new RegionsCollapsedEventArgs(collapsedList));
            }
            return collapsedList;
        }

        public IEnumerable<ICollapsible> ExpandAll(SnapshotSpan span, Predicate<ICollapsed> match)
        {
            return ExpandAllInternal(false, span, match);
        }

        public IEnumerable<ICollapsible> ExpandAllInternal(bool removalPending, SnapshotSpan span, Predicate<ICollapsed> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));
            EnsureValid(span);
            var collapsibleList = new List<ICollapsible>();
            foreach (var collapsedRegion in GetCollapsedRegions(span))
            {
                if (match(collapsedRegion))
                {
                    var collapsible = ExpandInternal(collapsedRegion);
                    collapsibleList.Add(collapsible);
                }
            }
            if (collapsibleList.Count > 0)
            {
                var regionsExpanded = RegionsExpanded;
                regionsExpanded?.Invoke(this, new RegionsExpandedEventArgs(collapsibleList, removalPending));
            }
            return collapsibleList;
        }

        public bool Enabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled == value)
                    return;
                var currentSnapshot = _editBuffer.CurrentSnapshot;
                var snapshotSpan = new SnapshotSpan(currentSnapshot, 0, currentSnapshot.Length);
                if (!value)
                    ExpandAllInternal(true, snapshotSpan, collapsed => true);
                _isEnabled = value;
                var regionsChanged = RegionsChanged;
                if (regionsChanged != null && !value)
                    regionsChanged(this, new RegionsChangedEventArgs(snapshotSpan));
                var outliningEnabledChanged = OutliningEnabledChanged;
                outliningEnabledChanged?.Invoke(this, new OutliningEnabledEventArgs(_isEnabled));
                if (!(regionsChanged != null & value))
                    return;
                regionsChanged(this, new RegionsChangedEventArgs(snapshotSpan));
            }
        }

        private SortedList<Collapsible, object> CollapsiblesFromTags(IEnumerable<IMappingTagSpan<IOutliningRegionTag>> tagSpans)
        {
            var currentSnapshot = _editBuffer.CurrentSnapshot;
            var sortedList = new SortedList<Collapsible, object>(new CollapsibleSorter(_editBuffer));
            foreach (var tagSpan in tagSpans)
            {
                var spans = tagSpan.Span.GetSpans(currentSnapshot);
                if (spans.Count == 1)
                {
                    var span = spans[0];
                    if (span.Length > 0)
                    {
                        span = spans[0];
                        var length1 = span.Length;
                        span = tagSpan.Span.GetSpans(tagSpan.Span.AnchorBuffer)[0];
                        var length2 = span.Length;
                        if (length1 == length2)
                        {
                            var key = new Collapsible(currentSnapshot.CreateTrackingSpan(spans[0], SpanTrackingMode.EdgeExclusive), tagSpan.Tag);
                            if (!sortedList.ContainsKey(key))
                                sortedList.Add(key, null);
                        }
                    }
                }
            }
            return sortedList;
        }

        private IEnumerable<ICollapsible> MergeRegions(IEnumerable<ICollapsed> currentCollapsed, IEnumerable<ICollapsible> newCollapsibles, out IEnumerable<ICollapsed> removedRegions)
        {
            var collapsedList1 = new List<ICollapsed>();
            var collapsedList2 = new List<ICollapsed>(currentCollapsed);
            var collapsibleList1 = new List<ICollapsible>(newCollapsibles);
            var collapsibleList2 = new List<ICollapsible>(collapsedList2.Count + collapsibleList1.Count);
            var index1 = 0;
            var index2 = 0;
            CollapsibleSorter collapsibleSorter = new CollapsibleSorter(_editBuffer);
            while (index1 < collapsedList2.Count || index2 < collapsibleList1.Count)
            {
                if (index1 < collapsedList2.Count && index2 < collapsibleList1.Count)
                {
                    var collapsed = collapsedList2[index1] as Collapsed;
                    var y = collapsibleList1[index2];
                    int num = collapsibleSorter.Compare(collapsed, y);
                    if (num == 0)
                    {
                        collapsed.Tag = y.Tag;
                        collapsibleList2.Add(collapsed);
                        ++index1;
                        ++index2;
                    }
                    else if (num < 0)
                    {
                        collapsedList1.Add(collapsed);
                        ++index1;
                    }
                    else if (num > 0)
                    {
                        collapsibleList2.Add(y);
                        ++index2;
                    }
                }
                else
                {
                    if (index1 < collapsedList2.Count)
                    {
                        collapsedList1.AddRange(collapsedList2.GetRange(index1, collapsedList2.Count - index1));
                        break;
                    }
                    if (index2 < collapsibleList1.Count)
                    {
                        collapsibleList2.AddRange(collapsibleList1.GetRange(index2, collapsibleList1.Count - index2));
                        break;
                    }
                }
            }
            removedRegions = collapsedList1;
            return collapsibleList2;
        }

        public IEnumerable<ICollapsed> GetCollapsedRegions(SnapshotSpan span)
        {
            return GetCollapsedRegionsInternal(new NormalizedSnapshotSpanCollection(span), false);
        }

        public IEnumerable<ICollapsed> GetCollapsedRegions(SnapshotSpan span, bool exposedRegionsOnly)
        {
            EnsureValid(span);
            return GetCollapsedRegionsInternal(new NormalizedSnapshotSpanCollection(span), exposedRegionsOnly);
        }

        public IEnumerable<ICollapsed> GetCollapsedRegions(NormalizedSnapshotSpanCollection spans)
        {
            return GetCollapsedRegionsInternal(spans, false);
        }

        public IEnumerable<ICollapsed> GetCollapsedRegions(NormalizedSnapshotSpanCollection spans, bool exposedRegionsOnly)
        {
            return GetCollapsedRegionsInternal(spans, exposedRegionsOnly);
        }

        internal IList<Collapsed> GetCollapsedRegionsInternal(NormalizedSnapshotSpanCollection spans, bool exposedRegionsOnly)
        {
            EnsureValid(spans);
            if (!_isEnabled)
                return new List<Collapsed>();
            if (exposedRegionsOnly)
                return _collapsedRegionTree.FindTopLevelNodesIntersecting(spans).Select(node => node.Item).ToList();
            return _collapsedRegionTree.FindNodesIntersecting(spans).Select(node => node.Item).ToList();
        }

        public IEnumerable<ICollapsible> GetAllRegions(SnapshotSpan span)
        {
            return GetAllRegions(span, false);
        }

        public IEnumerable<ICollapsible> GetAllRegions(SnapshotSpan span, bool exposedRegionsOnly)
        {
            EnsureValid(span);
            return GetAllRegions(new NormalizedSnapshotSpanCollection(span), exposedRegionsOnly);
        }

        public IEnumerable<ICollapsible> GetAllRegions(NormalizedSnapshotSpanCollection spans)
        {
            return GetAllRegions(spans, false);
        }

        public IEnumerable<ICollapsible> GetAllRegions(NormalizedSnapshotSpanCollection spans, bool exposedRegionsOnly)
        {
            return InternalGetAllRegions(spans, exposedRegionsOnly, new CancellationToken?());
        }

        internal IEnumerable<ICollapsible> InternalGetAllRegions(NormalizedSnapshotSpanCollection spans, bool exposedRegionsOnly, CancellationToken? cancel = null)
        {
            EnsureValid(spans);
            if (!_isEnabled || spans.Count == 0)
                return new List<Collapsible>();
            var snapshot = spans[0].Snapshot;
            var collapsedRegionsInternal = GetCollapsedRegionsInternal(spans, exposedRegionsOnly);
            IEnumerable<ICollapsible> newCollapsibles;
            if (!exposedRegionsOnly || collapsedRegionsInternal.Count == 0)
            {
                newCollapsibles = CollapsiblesFromTags(InternalGetTags(spans, cancel)).Keys;
            }
            else
            {
                var right = new NormalizedSnapshotSpanCollection(collapsedRegionsInternal.Select(c => c.Extent.GetSpan(snapshot)));
                newCollapsibles = CollapsiblesFromTags(InternalGetTags(NormalizedSnapshotSpanCollection.Union(NormalizedSnapshotSpanCollection.Difference(spans, right), new NormalizedSnapshotSpanCollection(new SnapshotSpan[2]
                {
                    new SnapshotSpan(spans[0].Start, 0),
                    new SnapshotSpan(spans[spans.Count - 1].End, 0)
                })), cancel)).Keys.Where(c => IsRegionExposed(c, snapshot));
            }

            var collapsibles = MergeRegions(collapsedRegionsInternal, newCollapsibles, out var removedRegions);
            foreach (var collapsed in removedRegions)
            {
                if (collapsed.IsCollapsed)
                    Expand(collapsed);
            }
            return collapsibles;
        }

        private IEnumerable<IMappingTagSpan<IOutliningRegionTag>> InternalGetTags(NormalizedSnapshotSpanCollection spans, CancellationToken? cancel)
        {
            return cancel.HasValue ? _tagAggregator.GetAllTags(spans, cancel.Value) : _tagAggregator.GetTags(spans);
        }

        private bool IsRegionExposed(ICollapsible region, ITextSnapshot current)
        {
            var span = region.Extent.GetSpan(current);
            if (!_collapsedRegionTree.IsPointContainedInANode(span.Start))
                return !_collapsedRegionTree.IsPointContainedInANode(span.End);
            return false;
        }

        public IEnumerable<ICollapsed> CollapseAll(SnapshotSpan span, Predicate<ICollapsible> match, CancellationToken cancel)
        {
            return InternalCollapseAll(span, match, cancel);
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;
            IsDisposed = true;
            _editBuffer.Changed -= SourceTextChanged;
            _tagAggregator.BatchedTagsChanged -= OutliningRegionTagsChanged;
            _tagAggregator.Dispose();
        }

        private void EnsureValid()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(OutliningManager));
        }

        private void EnsureValid(NormalizedSnapshotSpanCollection spans)
        {
            EnsureValid();
            if (spans == null)
                throw new ArgumentNullException(nameof(spans));
            if (spans.Count == 0)
                throw new ArgumentException("The given span collection is empty.", nameof(spans));
            if (spans[0].Snapshot.TextBuffer != _editBuffer)
                throw new ArgumentException("The given span collection is on an invalid buffer.Spans must be generated against the view model's edit buffer", nameof(spans));
        }

        private void EnsureValid(SnapshotSpan span)
        {
            EnsureValid();
            if (span.Snapshot == null)
                throw new ArgumentException("The given span is uninitialized.");
            if (span.Snapshot.TextBuffer != _editBuffer)
                throw new ArgumentException("The given span is on an invalid buffer.Spans must be generated against the view model's edit buffer", nameof(span));
        }
    }
}