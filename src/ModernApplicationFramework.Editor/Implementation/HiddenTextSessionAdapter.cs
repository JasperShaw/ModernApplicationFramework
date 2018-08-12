using System;
using System.Collections.Generic;
using System.Linq;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Tagging;
using ModernApplicationFramework.Text.Ui.Tagging;
using ModernApplicationFramework.Text.Utilities;

namespace ModernApplicationFramework.Editor.Implementation
{
    internal class HiddenTextSessionAdapter : ITagger<IOutliningRegionTag>, IHiddenTextSession, IHiddenTextSessionExPrivate, IOutliningSession, ICompoundViewChange
    {
        internal bool OutliningStarted = true;
        internal TextDocData TextDocData;
        internal ITextBuffer TextBuffer;
        private TrackingSpanTree<HiddenRegionAdapter> _regions;
        private List<HiddenRegionAdapter> _unboundRegions;
        internal List<HiddenTextSessionCoordinator> HiddenTextSessionCoordinators;
        private int _batchNesting;
        private ITextSnapshot _invalidRegionTextSnapshot;
        private int _invalidStart;
        private int _invalidEnd;
        private List<ITrackingSpan> _batchedCollapsedRegions;
        private List<ITrackingSpan> _batchedCollapsedRegionsUndoable;
        private List<ITrackingSpan> _batchedExpandedRegions;

        internal void Reset()
        {
            TextDocData = null;
            TextBuffer = null;
            _regions = null;
            _unboundRegions = null;
        }

        internal TextDocData DocData
        {
            get => TextDocData;
            set
            {
                if (TextDocData != null || value == null)
                    return;
                TextDocData = value;
            }
        }

        internal ITextBuffer Buffer
        {
            get => TextBuffer;
            set
            {
                if (TextBuffer != null || value == null)
                    return;
                TextBuffer = value;
                if (_unboundRegions == null)
                    return;
                StartBatch();
                try
                {
                    foreach (var unboundRegion in UnboundRegions)
                    {
                        unboundRegion.EnsureTrackingSpan();
                        AddRegion(unboundRegion);
                    }
                    _unboundRegions = null;
                }
                finally
                {
                    EndBatch();
                }
            }
        }

        internal TrackingSpanTree<HiddenRegionAdapter> Regions
        {
            get
            {
                if (_regions == null && Buffer != null)
                    _regions = new TrackingSpanTree<HiddenRegionAdapter>(Buffer, false);
                return _regions;
            }
        }

        internal List<HiddenRegionAdapter> UnboundRegions => _unboundRegions ?? (_unboundRegions = new List<HiddenRegionAdapter>());

        internal IEnumerable<HiddenRegionAdapter> GetCurrentHiddenRegionAdapters()
        {
            if (Regions != null && Buffer != null)
                return Regions.FindNodesIntersecting(new SnapshotSpan(Buffer.CurrentSnapshot, 0, Buffer.CurrentSnapshot.Length)).Select(n => n.Item);
            return UnboundRegions;
        }

        internal IHiddenTextClient HiddenTextClient { get; private set; }

        internal uint Flags { get; private set; }

        internal bool Initialized { get; private set; }

        internal void Init(IHiddenTextClient pClient, uint dwFlags)
        {
            HiddenTextClient = pClient;
            Flags = dwFlags;
            Initialized = true;
        }

        internal void AddCoordinator(HiddenTextSessionCoordinator vsHiddenTextSessionCoordinator)
        {
            if (vsHiddenTextSessionCoordinator == null)
                return;
            if (HiddenTextSessionCoordinators == null)
                HiddenTextSessionCoordinators = new List<HiddenTextSessionCoordinator>();
            var sessionCoordinator = HiddenTextSessionCoordinator;
            HiddenTextSessionCoordinators.Add(vsHiddenTextSessionCoordinator);
            if (sessionCoordinator == null)
                return;
            vsHiddenTextSessionCoordinator.CloneCoordinator(sessionCoordinator);
        }

        internal void RemoveCoordinator(HiddenTextSessionCoordinator vsHiddenTextSessionCoordinator)
        {
            if (vsHiddenTextSessionCoordinator == null || HiddenTextSessionCoordinators == null || !HiddenTextSessionCoordinators.Contains(vsHiddenTextSessionCoordinator))
                return;
            HiddenTextSessionCoordinators.Remove(vsHiddenTextSessionCoordinator);
        }

        internal HiddenTextSessionCoordinator HiddenTextSessionCoordinator
        {
            get
            {
                if (HiddenTextSessionCoordinators == null || !HiddenTextSessionCoordinators.Any())
                    return null;
                return HiddenTextSessionCoordinators.First();
            }
        }

        internal void InvalidateSpanForRegion(HiddenRegionAdapter vsHiddenRegion)
        {
            if (vsHiddenRegion.TrackingSpan == null || Buffer == null)
                return;
            StartBatch();
            try
            {
                var flag = true;
                if (_invalidRegionTextSnapshot == null)
                {
                    _invalidRegionTextSnapshot = Buffer.CurrentSnapshot;
                    flag = false;
                }
                var span = vsHiddenRegion.TrackingSpan.GetSpan(_invalidRegionTextSnapshot);
                if (!flag)
                {
                    _invalidStart = span.Start;
                    _invalidEnd = span.End;
                }
                else
                {
                    if (_invalidStart > span.Start)
                        _invalidStart = span.Start;
                    if (_invalidEnd >= span.End)
                        return;
                    _invalidEnd = span.End;
                }
            }
            finally
            {
                EndBatch();
            }
        }

        public IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (spans == null || spans.Count == 0 || Regions == null)
                return new TagSpan<IOutliningRegionTag>[0];
            var snapshot = spans[0].Snapshot;
            return Regions.FindNodesIntersecting(spans).Select(node => new
            {
                span = node.TrackingSpan.GetSpan(snapshot),
                adapter = node.Item
            }).Where(r => !r.span.IsEmpty).Select(r => new TagSpan<IOutliningRegionTag>(r.span, r.adapter));
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public int AddHiddenRegions(uint dwUpdateFlags, int cRegions, NewHiddenRegion[] rgHidReg, IEnumHiddenRegions[] ppEnum)
        {
            if (cRegions <= 0 || rgHidReg == null)
                return -2147024809;
            var hiddenRegionAdapterList = new List<HiddenRegionAdapter>();
            for (var index = 0; index < cRegions; ++index)
                hiddenRegionAdapterList.Add(new HiddenRegionAdapter(new NewHiddenRegionAccessor(rgHidReg[index]), this));
            AddHiddenRegionAdapters(hiddenRegionAdapterList, ((int)dwUpdateFlags & 1) == 0);
            if (ppEnum != null)
                ppEnum[0] = new EnumHiddenRegionsAdapter(hiddenRegionAdapterList);
            return 0;
        }

        public int EnumHiddenRegions(uint dwFindFlags, uint dwCookie, TextSpan[] ptsRange, out IEnumHiddenRegions ppEnum)
        {
            ppEnum = null;
            if (((int)dwFindFlags & 76) != 0 && (ptsRange == null || ptsRange.Length < 1))
                return -2147024809;
            var vsHiddenRegionAdapters = Enumerable.Empty<HiddenRegionAdapter>();
            if (Buffer != null && Regions != null)
                vsHiddenRegionAdapters = GetFilteredHiddenRegionAdapters(Buffer, Regions, dwFindFlags, dwCookie, ptsRange);
            ppEnum = new EnumHiddenRegionsAdapter(vsHiddenRegionAdapters);
            return 0;
        }

        public int Terminate()
        {
            _regions = null;
            _unboundRegions = null;
            HiddenTextClient = null;
            Flags = 0U;
            Initialized = false;
            return 0;
        }

        public int UnadviseClient()
        {
            HiddenTextClient = null;
            return 0;
        }

        public int AddHiddenRegionsEx(uint dwUpdateFlags, int cRegions, NewHiddenRegionEx[] rgHidReg, IEnumHiddenRegions[] ppEnum)
        {
            if (cRegions <= 0 || rgHidReg == null)
                return -2147024809;
            var hiddenRegionAdapterList = new List<HiddenRegionAdapter>(cRegions);
            for (var index = 0; index < cRegions; ++index)
                hiddenRegionAdapterList.Add(new HiddenRegionAdapter(new NewHiddenRegionExAccessor(rgHidReg[index]), this));
            AddHiddenRegionAdapters(hiddenRegionAdapterList, ((int)dwUpdateFlags & 1) == 0);
            if (ppEnum != null)
                ppEnum[0] = new EnumHiddenRegionsAdapter(hiddenRegionAdapterList);
            return 0;
        }

        internal event EventHandler<EventArgs> AdaptedOutliningStopped;

        public int StopOutlining()
        {
            var outliningStopped = AdaptedOutliningStopped;
            outliningStopped?.Invoke(this, new EventArgs());
            if (OutliningStarted)
            {
                OutliningStarted = false;
                StartBatch();
                try
                {
                    foreach (var vsHiddenRegion in new List<HiddenRegionAdapter>(GetCurrentHiddenRegionAdapters()))
                    {
                        vsHiddenRegion.Valid = false;
                        RemoveRegion(vsHiddenRegion);
                    }
                }
                finally
                {
                    EndBatch();
                }
            }
            return 0;
        }

        internal event EventHandler<AdaptedOutliningStartedEventArgs> AdaptedOutliningStarted;

        public int StartOutlining(int fRemoveAdhoc)
        {
            if (!OutliningStarted)
            {
                OutliningStarted = true;
                var outliningStarted = AdaptedOutliningStarted;
                outliningStarted?.Invoke(this, new AdaptedOutliningStartedEventArgs((uint)fRemoveAdhoc > 0U));
            }
            return 0;
        }

        public int StartBatch()
        {
            ++_batchNesting;
            return 0;
        }

        public int EndBatch()
        {
            if (_batchNesting <= 0)
                return -2147418113;
            --_batchNesting;
            if (_batchNesting == 0)
            {
                if (_invalidRegionTextSnapshot != null)
                {
                    var tagsChanged = TagsChanged;
                    tagsChanged?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(_invalidRegionTextSnapshot, _invalidStart, _invalidEnd - _invalidStart)));
                }
                _invalidRegionTextSnapshot = null;
                if (_batchedCollapsedRegions != null)
                {
                    HandleExpandedOrCollapsed(_batchedCollapsedRegions, true, true, false);
                    _batchedCollapsedRegions = null;
                }
                if (_batchedCollapsedRegionsUndoable != null)
                {
                    HandleExpandedOrCollapsed(_batchedCollapsedRegionsUndoable, true, true, true);
                    _batchedCollapsedRegionsUndoable = null;
                }
                if (_batchedExpandedRegions != null)
                {
                    HandleExpandedOrCollapsed(_batchedExpandedRegions, false, true, true);
                    _batchedExpandedRegions = null;
                }
            }
            return 0;
        }

        public int AddOutlineRegions(uint dwOutliningFlags, int cRegions, NewOutlineRegion[] rgOutlnReg)
        {
            if (cRegions <= 0 || rgOutlnReg == null)
                return -2147024809;
            var hiddenRegionAdapterList = new List<HiddenRegionAdapter>();
            for (var index = 0; index < cRegions; ++index)
                hiddenRegionAdapterList.Add(new HiddenRegionAdapter(new NewOutlineRegionAccessor(rgOutlnReg[index]), this));
            AddHiddenRegionAdapters(hiddenRegionAdapterList, ((int)dwOutliningFlags & 256) == 0);
            return 0;
        }

        public int OpenCompoundViewChange()
        {
            StartBatch();
            return 0;
        }

        public int CloseCompoundViewChange()
        {
            EndBatch();
            return 0;
        }

        internal void AddHiddenRegionAdapters(IEnumerable<HiddenRegionAdapter> vsHiddenRegionAdapters, bool undoable)
        {
            StartOutlining(1);
            StartBatch();
            try
            {
                foreach (var hiddenRegionAdapter in vsHiddenRegionAdapters)
                {
                    AddRegion(hiddenRegionAdapter);
                    if (hiddenRegionAdapter.IsDefaultCollapsed)
                    {
                        if (undoable)
                            BatchedCollapsedRegionsUndoable.Add(hiddenRegionAdapter.TrackingSpan);
                        else
                            BatchedCollapsedRegions.Add(hiddenRegionAdapter.TrackingSpan);
                    }
                }
            }
            finally
            {
                EndBatch();
            }
        }

        internal void AddRegion(HiddenRegionAdapter vsHiddenRegion)
        {
            if (Regions != null)
            {
                if (Regions.TryAddItem(vsHiddenRegion, vsHiddenRegion.TrackingSpan) == null)
                    return;
                InvalidateSpanForRegion(vsHiddenRegion);
            }
            else
                UnboundRegions.Add(vsHiddenRegion);
        }

        internal void RemoveRegion(HiddenRegionAdapter vsHiddenRegion)
        {
            if (Regions != null)
            {
                if (!Regions.RemoveItem(vsHiddenRegion, vsHiddenRegion.TrackingSpan))
                    return;
                InvalidateSpanForRegion(vsHiddenRegion);
            }
            else
                UnboundRegions.Remove(vsHiddenRegion);
        }

        internal static IEnumerable<HiddenRegionAdapter> GetFilteredHiddenRegionAdapters(ITextBuffer buffer, TrackingSpanTree<HiddenRegionAdapter> regions, uint dwFindFlags, uint clientData, TextSpan[] ptsRange)
        {
            var snapshot = buffer.CurrentSnapshot;
            IEnumerable<HiddenRegionAdapter> source;
            if (((int)dwFindFlags & 76) != 0)
            {
                int start = snapshot.GetLineFromLineNumber(ptsRange[0].iStartLine).Start + ptsRange[0].iStartIndex;
                int end = snapshot.GetLineFromLineNumber(ptsRange[0].iEndLine).Start + ptsRange[0].iEndIndex;
                var extent = new SnapshotSpan(snapshot, Span.FromBounds(start, end));
                source = ((int)dwFindFlags & 8) == 0 ? (((int)dwFindFlags & 64) == 0 ? regions.FindNodesIntersecting(extent).Select(n => n.Item) : regions.FindNodesContainedBy(extent).Select(n => n.Item)) : regions.FindNodesContainedBy(extent).Select(n => n.Item).Where(adapter =>
                {
                    if (adapter.Valid)
                        return adapter.TrackingSpan.GetSpan(snapshot) == extent;
                    return false;
                });
            }
            else
            {
                var span = new SnapshotSpan(snapshot, 0, snapshot.Length);
                source = regions.FindNodesIntersecting(span).Select(n => n.Item);
            }
            if (((int)dwFindFlags & 1) != 0)
                source = source.Where(adapter => (int)adapter.ClientData == (int)clientData);
            if (((int)dwFindFlags & 32) != 0)
                source = source.Where(adapter => adapter.Behavior == HiddenRegionBehavior.HrbClientControlled);
            if (((int)dwFindFlags & 16) != 0)
                source = source.Where(adapter => adapter.Behavior == HiddenRegionBehavior.HrbEditorControlled);
            return source.ToList();
        }

        internal event EventHandler<HiddenRegionsExpandedOrCollapsedEventArgs> HiddenRegionExpandedOrCollapsed;

        internal void HandleExpandedOrCollapsed(IEnumerable<ITrackingSpan> trackingSpans, bool collapsed, bool subjectBufferSpans, bool undoable)
        {
            var expandedOrCollapsed = HiddenRegionExpandedOrCollapsed;
            expandedOrCollapsed?.Invoke(this, new HiddenRegionsExpandedOrCollapsedEventArgs(trackingSpans, collapsed, subjectBufferSpans, undoable));
        }

        internal bool GetCollapsed(HiddenRegionAdapter vsHiddenRegionAdapter, out bool collapsed)
        {
            if (HiddenTextSessionCoordinator != null)
            {
                collapsed = HiddenTextSessionCoordinator.IsCollapsed(vsHiddenRegionAdapter);
                return true;
            }
            collapsed = false;
            return false;
        }

        internal List<ITrackingSpan> BatchedCollapsedRegions => _batchedCollapsedRegions ?? (_batchedCollapsedRegions = new List<ITrackingSpan>());

        internal List<ITrackingSpan> BatchedCollapsedRegionsUndoable => _batchedCollapsedRegionsUndoable ??
                                                                        (_batchedCollapsedRegionsUndoable = new List<ITrackingSpan>());

        private List<ITrackingSpan> BatchedExpandedRegions => _batchedExpandedRegions ?? (_batchedExpandedRegions = new List<ITrackingSpan>());

        internal void ExpandOrCollapse(HiddenRegionAdapter vsHiddenRegionAdapter, bool collapse)
        {
            StartBatch();
            try
            {
                var trackingSpan = vsHiddenRegionAdapter.TrackingSpan;
                if (collapse)
                    BatchedCollapsedRegionsUndoable.Add(trackingSpan);
                else
                    BatchedExpandedRegions.Add(trackingSpan);
            }
            finally
            {
                EndBatch();
            }
        }
    }
}