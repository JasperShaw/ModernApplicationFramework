using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices;

namespace ModernApplicationFramework.TextEditor
{
    internal sealed class MarkerManager
    {
        internal MarkerStore _markerStore = new MarkerStore();
        private readonly IDictionary<int, MarkerType> _typeMap = new Dictionary<int, MarkerType>();
        private readonly IMafTextManager _textManager;
        private Queue<Action> _externalCallQueue;

        //internal IEnumerable<VsTextMarkerAdapter> AllMarkers => _markerStore.AllMarkers;

        internal MarkerTaggerImplementation MarkerTagger { get; }

        internal GlyphTaggerImplementation GlyphTagger { get; }

        internal OverviewTaggerImplementation OverviewTagger { get; }

        internal ErrorTaggerImplementation ErrorTagger { get; }

        private bool IsQueuingExternalCalls => _externalCallQueue != null;

        private MarkerManager(ITextBuffer documentTextBuffer)
        {
            Buffer = documentTextBuffer;
            //_textManager = Common.GetService<IVsTextManager, SVsTextManager>(_pServiceProvider);
            _externalCallQueue = null;
            MarkerTagger = new MarkerTaggerImplementation(this);
            GlyphTagger = new GlyphTaggerImplementation(this);
            ErrorTagger = new ErrorTaggerImplementation(this);
            OverviewTagger = new OverviewTaggerImplementation(this);
            documentTextBuffer.ChangedHighPriority += OnBufferChanged;
            documentTextBuffer.ContentTypeChanged += OnBufferContentTypeChanged;
        }

        internal ITextSnapshot Snapshot => _markerStore.Snapshot;

        //internal static MarkerManager GetTagger(ITextBuffer buffer)
        //{
        //    buffer.Properties.TryGetProperty(typeof(MarkerManager), out MarkerManager property);
        //    return property;
        //}

        private void QueueExternalCall(Action action)
        {
            _externalCallQueue.Enqueue(action);
        }

        private void QueueExternalCalls()
        {
            if (_externalCallQueue != null)
                FlushExternalCallQueue();
            _externalCallQueue = new Queue<Action>();
        }

        private void FlushExternalCallQueue()
        {
            try
            {
                while (_externalCallQueue.Count > 0)
                    _externalCallQueue.Dequeue()();
            }
            finally
            {
                _externalCallQueue = null;
            }
        }

        private void OnBufferContentTypeChanged(object sender, ContentTypeChangedEventArgs e)
        {
            UpdateSnapshot(e.After, true);
        }

        private void OnBufferChanged(object sender, TextContentChangedEventArgs e)
        {
            var keepMarkers = false;
            if (e.EditTag is ReplaceTextFlags flags)
                keepMarkers = (uint)(flags & ReplaceTextFlags.RtfKeepMarkers) > 0U;
            UpdateSnapshot(e.After, keepMarkers);
        }

        private void UpdateSnapshot(ITextSnapshot snapshot, bool keepMarkers)
        {
            QueueExternalCalls();
            foreach (var textMarkerAdapter in _markerStore.AllMarkersCopy)
                textMarkerAdapter.Tag.SetSnapshot(snapshot, keepMarkers);
            FlushExternalCallQueue();
        }

        public IEnumerable<ITagSpan<VsTextMarkerTag>> GetMarkerTags(NormalizedSnapshotSpanCollection spans)
        {
            var tagSpanList = new List<ITagSpan<VsTextMarkerTag>>();
            if (spans.Count > 0)
            {
                lock (_markerStore)
                {
                    var snapshot = spans[0].Snapshot;
                    foreach (var visibleMarker in _markerStore.VisibleMarkers)
                    {
                        if (visibleMarker.IsVisible)
                        {
                            var span = visibleMarker.Tag.MarkerSpan.SnapshotSpan.TranslateTo(snapshot, visibleMarker.Tag.TextMarker.SpanTrackingMode);
                            if (spans.IntersectsWith(span))
                                tagSpanList.Add(new TagSpan<VsTextMarkerTag>(span, visibleMarker.Tag));
                        }
                    }
                }
            }
            return tagSpanList;
        }

        internal IEnumerable<ITagSpan<IGlyphTag>> GetGlyphTags(NormalizedSnapshotSpanCollection spans)
        {
            var tagSpanList = new List<TagSpan<IGlyphTag>>();
            if (spans.Count > 0)
            {
                lock (_markerStore)
                {
                    var snapshot = spans[0].Snapshot;
                    foreach (var visibleMarker in _markerStore.VisibleMarkers)
                    {
                        if (((int)visibleMarker.VisualStyle & 1) != 0)
                        {
                            var span = visibleMarker.Tag.MarkerSpan.SnapshotSpan.TranslateTo(snapshot, visibleMarker.Tag.TextMarker.SpanTrackingMode);
                            if (spans.IntersectsWith(span))
                            {
                                VsTextMarkerGlyphTag textMarkerGlyphTag = new VsTextMarkerGlyphTag(visibleMarker.Tag);
                                tagSpanList.Add(new TagSpan<IGlyphTag>(visibleMarker.Tag.MarkerSpan.SnapshotSpan, (IGlyphTag)textMarkerGlyphTag));
                            }
                        }
                    }
                    tagSpanList.Sort((tag1, tag2) => ((VsTextMarkerGlyphTag)tag1.Tag).TextMarkerTag.TextMarker.Priority.CompareTo(((VsTextMarkerGlyphTag)tag2.Tag).TextMarkerTag.TextMarker.Priority));
                }
            }
            return tagSpanList;
        }

        public IEnumerable<ITagSpan<ErrorTag>> GetErrorTags(NormalizedSnapshotSpanCollection spans)
        {
            var tagSpanList1 = new List<TagSpan<ErrorTag>>();
            if (spans.Count > 0)
            {
                lock (_markerStore)
                {
                    var snapshot = spans[0].Snapshot;
                    foreach (var visibleMarker in _markerStore.VisibleMarkers)
                    {
                        if (visibleMarker.SquiggleName != null)
                        {
                            var markerSpan = visibleMarker.Tag.MarkerSpan;
                            var span = markerSpan.SnapshotSpan.TranslateTo(snapshot, visibleMarker.Tag.TextMarker.SpanTrackingMode);
                            if (spans.IntersectsWith(span))
                            {
                                var tagSpanList2 = tagSpanList1;
                                markerSpan = visibleMarker.Tag.MarkerSpan;
                                var tagSpan = new TagSpan<ErrorTag>(markerSpan.SnapshotSpan, new ErrorTag(visibleMarker.SquiggleName));
                                tagSpanList2.Add(tagSpan);
                            }
                        }
                    }
                }
            }
            return tagSpanList1;
        }

        public IEnumerable<ITagSpan<IOverviewMarkTag>> GetOverviewTags(NormalizedSnapshotSpanCollection spans)
        {
            var tagSpanList1 = new List<TagSpan<IOverviewMarkTag>>();
            if (spans.Count > 0)
            {
                lock (_markerStore)
                {
                    var snapshot = spans[0].Snapshot;
                    foreach (var visibleMarker in _markerStore.VisibleMarkers)
                    {
                        var overviewMarkType = OverviewTaggerImplementation.MapMarkerToOverviewMarkType(visibleMarker.MarkerType);
                        if (overviewMarkType != null)
                        {
                            var markerSpan = visibleMarker.Tag.MarkerSpan;
                            var span = markerSpan.SnapshotSpan.TranslateTo(snapshot, visibleMarker.Tag.TextMarker.SpanTrackingMode);
                            if (spans.IntersectsWith(span))
                            {
                                var tagSpanList2 = tagSpanList1;
                                markerSpan = visibleMarker.Tag.MarkerSpan;
                                var tagSpan = new TagSpan<IOverviewMarkTag>(markerSpan.SnapshotSpan, new OverviewMarkTag(overviewMarkType));
                                tagSpanList2.Add(tagSpan);
                            }
                        }
                    }
                }
            }
            return tagSpanList1;
        }

        public static MarkerManager Create(ITextBuffer documentTextBuffer)
        {
            if (documentTextBuffer == null)
                throw new ArgumentNullException(nameof(documentTextBuffer));
            return documentTextBuffer.Properties.GetOrCreateSingletonProperty((() => new MarkerManager(documentTextBuffer)));
        }

        public static bool CanAffectClassification(VsTextMarkerAdapter marker)
        {
            return CanAffectClassification(marker.VisualStyle, marker.LineStyle);
        }

        public static bool CanAffectClassification(uint visualStyle, Linestyle lineStyle)
        {
            if (((int)visualStyle & 4) != 0 || ((int)visualStyle & 2) != 0)
                return true;
            if (((int)visualStyle & 256) != 0)
                return lineStyle != Linestyle.LiSquiggly;
            return false;
        }

        public static bool CanAffectGlyphMargin(VsTextMarkerAdapter marker)
        {
            return CanAffectGlyphMargin(marker.VisualStyle);
        }

        public static bool CanAffectGlyphMargin(uint visualStyle)
        {
            return (visualStyle & 1U) > 0U;
        }

        public MarkerChangedMask GetMarkerChangeMask(VsTextMarkerAdapter marker)
        {
            var markerChangedMask = MarkerChangedMask.None;
            if (marker.IsVisible)
            {
                if (marker.SquiggleName != null)
                    markerChangedMask |= MarkerChangedMask.SquiggleName;
                if (OverviewTaggerImplementation.MapMarkerToOverviewMarkType(marker.MarkerType) != null)
                    markerChangedMask |= MarkerChangedMask.OverviewMark;
                if (CanAffectClassification(marker) || _markerStore.MarkersAffectingClassificationExist)
                    markerChangedMask |= MarkerChangedMask.GlyphTags | MarkerChangedMask.Tags | MarkerChangedMask.TextClassification;
                else if (CanAffectGlyphMargin(marker))
                    markerChangedMask |= MarkerChangedMask.GlyphTags;
            }
            return markerChangedMask;
        }

        public static MarkerManager Get(ITextBuffer buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (buffer.Properties.TryGetProperty(typeof(MarkerManager), out MarkerManager property))
                return property;
            return null;
        }

        public IList<VsTextMarkerAdapter> InternalGetMarkers(int markerType, VirtualSnapshotSpan virtualSpan, Enummarkerflags enumMarkerFlags)
        {
            var textMarkerAdapterList = new List<VsTextMarkerAdapter>();
            lock (_markerStore)
            {
                foreach (var allMarker in _markerStore.AllMarkers)
                {
                    if (((enumMarkerFlags & Enummarkerflags.EmAlltypes) != Enummarkerflags.EmDefault || allMarker.Type == markerType) && ((enumMarkerFlags & Enummarkerflags.EmIncludeinvisible) != Enummarkerflags.EmDefault || allMarker.Type > 0))
                    {
                        var markerSpan = allMarker.Tag.MarkerSpan;
                        if ((enumMarkerFlags & Enummarkerflags.EmEntirebuffer) != Enummarkerflags.EmDefault || ((enumMarkerFlags & Enummarkerflags.EmContained) != Enummarkerflags.EmDefault ? (virtualSpan.Contains(markerSpan) ? 1 : 0) : (virtualSpan.IntersectsWith(markerSpan) ? 1 : 0)) != 0)
                            textMarkerAdapterList.Add(allMarker);
                    }
                }
            }
            if ((enumMarkerFlags & Enummarkerflags.EmSortpriority) != Enummarkerflags.EmDefault)
                textMarkerAdapterList.Sort((enumMarkerFlags & Enummarkerflags.EmSortdescending) != Enummarkerflags.EmDefault ? (a, b) => b.Priority - a.Priority : (Comparison<VsTextMarkerAdapter>)((a, b) => a.Priority - b.Priority));
            return textMarkerAdapterList;
        }

        public void CallIntoExternalCode(Action action)
        {
            if (IsQueuingExternalCalls)
                QueueExternalCall(action);
            else
                action();
        }

        public bool AddMarker(VsTextMarkerAdapter marker)
        {
            if (!_markerStore.AddMarker(marker))
                return false;
            RaiseChangedEvents(marker.Tag.TagTextBuffer, marker.Tag.MarkerSpan.SnapshotSpan, GetMarkerChangeMask(marker));
            return true;
        }

        public bool RemoveMarker(VsTextMarkerAdapter marker)
        {
            return RemoveMarker(marker, false);
        }

        public bool RemoveMarker(VsTextMarkerAdapter marker, bool markerSpanDeleted)
        {
            var markerChangeMask = GetMarkerChangeMask(marker);
            var num = _markerStore.RemoveMarker(marker) ? 1 : 0;
            if (num == 0)
                return num != 0;
            var snapshotSpan = marker.Tag.MarkerSpan.SnapshotSpan;
            if (markerSpanDeleted)
                return num != 0;
            RaiseChangedEvents(marker.Tag.TagTextBuffer, snapshotSpan, markerChangeMask);
            return num != 0;
        }

        public MarkerType GetMarkerType(int type)
        {
            lock (_typeMap)
            {
                if (_typeMap.TryGetValue(type, out var markerType))
                    return markerType;
            }

            Marshal.ThrowExceptionForHR(_textManager.GetMarkerTypeInterface(type, out IVsTextMarkerType ppMarkerType));
            var markerType1 = new MarkerType(type, ppMarkerType);
            lock (_typeMap)
            {
                if (_typeMap.TryGetValue(type, out var markerType2))
                    markerType1 = markerType2;
                else
                    _typeMap.Add(type, markerType1);
            }
            return markerType1;
        }

        public ITextBuffer Buffer { get; }

        //public VsTextMarkerAdapter CreateMarker(int markerType, VirtualSnapshotSpan virtualSpan, IVsTextMarkerClient markerClient)
        //{
        //    if (virtualSpan.Snapshot.TextBuffer != Buffer)
        //        throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The virtual span passed to CreateMarker is on the wrong buffer. Expected: {0}, Actual: {1}", Buffer, virtualSpan.Snapshot.TextBuffer), nameof(virtualSpan));
        //    var marker = new VsTextMarkerAdapter(this, markerType, markerClient, virtualSpan);
        //    AddMarker(marker);
        //    return marker;
        //}

        //public IList<VsTextMarkerAdapter> GetMarkers(int markerType, VirtualSnapshotSpan virtualSpan, Enummarkerflags enumMarkerFlags)
        //{
        //    return InternalGetMarkers(markerType, virtualSpan, enumMarkerFlags);
        //}

        //public VsTextMarkerAdapter FindMarker(int markerType, int position, Findmarkerflags findMarkerFlags)
        //{
        //    var textMarkerAdapter1 = (VsTextMarkerAdapter)null;
        //    var num1 = int.MaxValue;
        //    lock (_markerStore)
        //    {
        //        foreach (var textMarkerAdapter2 in _markerStore.AllMarkersOfType(markerType))
        //        {
        //            var markerSpan = textMarkerAdapter2.Tag.MarkerSpan;
        //            VirtualSnapshotPoint start;
        //            SnapshotPoint position1;
        //            int num2;
        //            if ((findMarkerFlags & Findmarkerflags.FmBackward) != Findmarkerflags.FmForward)
        //            {
        //                var num3 = position;
        //                start = markerSpan.Start;
        //                position1 = start.Position;
        //                var position2 = position1.Position;
        //                num2 = num3 - position2;
        //            }
        //            else
        //            {
        //                start = markerSpan.Start;
        //                position1 = start.Position;
        //                num2 = position1.Position - position;
        //            }
        //            var num4 = num2;
        //            if (num4 <= 0)
        //                num4 += markerSpan.Snapshot.Length + 1;
        //            if (num4 < num1)
        //            {
        //                textMarkerAdapter1 = textMarkerAdapter2;
        //                num1 = num4;
        //            }
        //        }
        //    }
        //    return textMarkerAdapter1;
        //}

        public void BufferClosed()
        {
            foreach (var textMarkerAdapter in new List<VsTextMarkerAdapter>(_markerStore.AllMarkers))
                textMarkerAdapter.OnBufferClose();
        }

        public void RaiseChangedEvents(ITextBuffer tagBuffer, SnapshotSpan span, MarkerChangedMask mask)
        {
            CallIntoExternalCode(() => InternalRaiseChangedEvents(tagBuffer, span, mask));
        }

        private void InternalRaiseChangedEvents(ITextBuffer tagBuffer, SnapshotSpan span, MarkerChangedMask mask)
        {
            if ((mask & MarkerChangedMask.GlyphTags) != MarkerChangedMask.None)
                Get(tagBuffer).GlyphTagger.RaiseTagsChanged(span);
            if ((mask & MarkerChangedMask.Tags) != MarkerChangedMask.None)
                Get(tagBuffer).MarkerTagger.RaiseTagsChanged(span);
            if ((mask & MarkerChangedMask.OverviewMark) != MarkerChangedMask.None)
                Get(tagBuffer).OverviewTagger.RaiseTagsChanged(span);
            if ((mask & MarkerChangedMask.TextClassification) != MarkerChangedMask.None)
            {
                foreach (var markerViewTagger in TextMarkerViewTaggerProvider.GetTextMarkerViewTaggers(tagBuffer).Values)
                    markerViewTagger?.RaiseChangedEvent(span);
            }
            if ((mask & MarkerChangedMask.SquiggleName) == MarkerChangedMask.None)
                return;
            Get(tagBuffer).ErrorTagger.RaiseTagsChanged(span);
        }

        public IEnumerable<ITagSpan<ClassificationTag>> GetClassificationTags(SnapshotSpan textSpan, ViewMarkerTypeManager viewMarkerTypeManager)
        {
            var tupleList = new List<Tuple<Span, VsTextMarkerAdapter>>();
            lock (_markerStore)
            {
                tupleList.AddRange(from textMarkerAdapter in _markerStore.VisibleMarkersWithColor
                    where textMarkerAdapter.IsExclusive(viewMarkerTypeManager)
                    let nullable1 =
                        textSpan.Overlap(textMarkerAdapter.Tag.MarkerSpan.SnapshotSpan.TranslateTo(textSpan.Snapshot,
                            SpanTrackingMode.EdgeInclusive))
                    let nullable2 = nullable1
                    where nullable2.HasValue
                    select new Tuple<Span, VsTextMarkerAdapter>(nullable2.Value, textMarkerAdapter));
            }
            foreach (var prioritizedMarkerSpan in GetPrioritizedMarkerSpans(tupleList))
            {
                var type = prioritizedMarkerSpan.Item2.TextClassificationType(viewMarkerTypeManager);
                if (type != null)
                    yield return new TagSpan<ClassificationTag>(new SnapshotSpan(textSpan.Snapshot, prioritizedMarkerSpan.Item1), new ClassificationTag(type));
            }
        }

        public static IList<Tuple<Span, VsTextMarkerAdapter>> GetPrioritizedMarkerSpans(IList<Tuple<Span, VsTextMarkerAdapter>> unsortedMarkers)
        {
            var tupleList = (IList<Tuple<Span, VsTextMarkerAdapter>>)new List<Tuple<Span, VsTextMarkerAdapter>>();
            if (unsortedMarkers.Count > 0)
            {
                var markerEdgeList1 = new List<MarkerEdge>();
                foreach (var unsortedMarker in unsortedMarkers)
                {
                    var markerEdgeList2 = markerEdgeList1;
                    var marker1 = unsortedMarker.Item2;
                    var span = unsortedMarker.Item1;
                    var start = span.Start;
                    var num1 = 1;
                    var markerEdge1 = new MarkerEdge(marker1, start, num1 != 0);
                    markerEdgeList2.Add(markerEdge1);
                    var markerEdgeList3 = markerEdgeList1;
                    var marker2 = unsortedMarker.Item2;
                    span = unsortedMarker.Item1;
                    var end = span.End;
                    var num2 = 0;
                    var markerEdge2 = new MarkerEdge(marker2, end, num2 != 0);
                    markerEdgeList3.Add(markerEdge2);
                }
                if (markerEdgeList1.Count > 0)
                {
                    markerEdgeList1.Sort((a, b) =>
                    {
                        if (a.Position != b.Position)
                            return a.Position - b.Position;
                        if (a.IsStart == b.IsStart)
                        {
                            if (!a.IsStart)
                                return a.Marker.Priority - b.Marker.Priority;
                            return b.Marker.Priority - a.Marker.Priority;
                        }
                        return !a.IsStart ? 1 : -1;
                    });
                    var textMarkerAdapterList = new List<VsTextMarkerAdapter>();
                    var textMarkerAdapter = (VsTextMarkerAdapter)null;
                    var start = int.MaxValue;
                    foreach (var markerEdge in markerEdgeList1)
                    {
                        if (markerEdge.IsStart)
                        {
                            if (markerEdge.Position >= start)
                            {
                                textMarkerAdapterList.Add(markerEdge.Marker);
                                if (markerEdge.Marker.Priority > textMarkerAdapter.Priority)
                                {
                                    if (markerEdge.Position > start)
                                    {
                                        tupleList.Add(new Tuple<Span, VsTextMarkerAdapter>(Span.FromBounds(start, markerEdge.Position), textMarkerAdapter));
                                        start = markerEdge.Position;
                                    }
                                    textMarkerAdapter = markerEdge.Marker;
                                }
                            }
                            else
                            {
                                start = markerEdge.Position;
                                textMarkerAdapterList.Add(markerEdge.Marker);
                                textMarkerAdapter = markerEdge.Marker;
                            }
                        }
                        else
                        {
                            textMarkerAdapterList.Remove(markerEdge.Marker);
                            if (textMarkerAdapter == markerEdge.Marker)
                            {
                                if (markerEdge.Position > start)
                                {
                                    tupleList.Add(new Tuple<Span, VsTextMarkerAdapter>(Span.FromBounds(start, markerEdge.Position), textMarkerAdapter));
                                    start = markerEdge.Position;
                                }
                                if (textMarkerAdapterList.Count == 0)
                                {
                                    textMarkerAdapter = null;
                                    start = int.MaxValue;
                                }
                                else
                                {
                                    textMarkerAdapter = textMarkerAdapterList[0];
                                    for (var index = 1; index < textMarkerAdapterList.Count; ++index)
                                    {
                                        if (textMarkerAdapterList[index].Priority > textMarkerAdapter.Priority)
                                            textMarkerAdapter = textMarkerAdapterList[index];
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return tupleList;
        }

        //private void OnFontsAndColorsChanged(object sender, EventArgs e)
        //{
        //    var snapshot = Snapshot;
        //    if (snapshot == null)
        //        return;
        //    RaiseChangedEvents(snapshot.TextBuffer, new SnapshotSpan(snapshot, 0, snapshot.Length), MarkerChangedMask.All);
        //}

        //internal void DestroyAllReadOnlyMarkers()
        //{
        //    lock (_markerStore)
        //    {
        //        foreach (var textMarkerAdapter in _markerStore.VisibleMarkers.Where(marker =>
        //        {
        //            if (marker.IsValid)
        //                return marker.Type == 1;
        //            return false;
        //        }).ToList())
        //            textMarkerAdapter.Invalidate();
        //    }
        //}

        public class MarkerStore
        {
            private readonly HashSet<VsTextMarkerAdapter> _invisibleMarkers = new HashSet<VsTextMarkerAdapter>();
            private readonly HashSet<VsTextMarkerAdapter> _visibleMarkersNoColor = new HashSet<VsTextMarkerAdapter>();
            private readonly HashSet<VsTextMarkerAdapter> _visibleMarkersWithColor = new HashSet<VsTextMarkerAdapter>();

            public int Count => _invisibleMarkers.Count + _visibleMarkersNoColor.Count + _visibleMarkersWithColor.Count;

            public bool MarkersAffectingClassificationExist => _visibleMarkersWithColor.Count > 0;

            public ITextSnapshot Snapshot
            {
                get
                {
                    lock (this)
                    {
                        using (var enumerator = _invisibleMarkers.GetEnumerator())
                        {
                            if (enumerator.MoveNext())
                                return enumerator.Current.Tag.MarkerSpan.Snapshot;
                        }
                        using (var enumerator = _visibleMarkersNoColor.GetEnumerator())
                        {
                            if (enumerator.MoveNext())
                                return enumerator.Current.Tag.MarkerSpan.Snapshot;
                        }
                        using (var enumerator = _visibleMarkersWithColor.GetEnumerator())
                        {
                            if (enumerator.MoveNext())
                                return enumerator.Current.Tag.MarkerSpan.Snapshot;
                        }
                        return null;
                    }
                }
            }

            public bool AddMarker(VsTextMarkerAdapter marker)
            {
                lock (this)
                {
                    if (marker.MarkerTypeCode == 0)
                    {
                        if (!_invisibleMarkers.Add(marker))
                            return false;
                    }
                    else if (CanAffectClassification(marker))
                    {
                        if (!_visibleMarkersWithColor.Add(marker))
                            return false;
                    }
                    else if (!_visibleMarkersNoColor.Add(marker))
                        return false;
                }
                return true;
            }

            public bool RemoveMarker(VsTextMarkerAdapter marker)
            {
                lock (this)
                {
                    if (marker.MarkerTypeCode == 0)
                        return _invisibleMarkers.Remove(marker);
                    if (CanAffectClassification(marker))
                        return _visibleMarkersWithColor.Remove(marker);
                    return _visibleMarkersNoColor.Remove(marker);
                }
            }

            public void MarkerTypeChanged(VsTextMarkerAdapter marker, MarkerType oldType, MarkerType newType, uint oldVisualStyle, uint newVisualStyle)
            {
                if (oldType.MarkerTypeID == 0 && newType.MarkerTypeID != 0)
                {
                    lock (this)
                    {
                        _invisibleMarkers.Remove(marker);
                        if (CanAffectClassification(marker))
                            _visibleMarkersWithColor.Add(marker);
                        else
                            _visibleMarkersNoColor.Add(marker);
                    }
                }
                else if (oldType.MarkerTypeID != 0 && newType.MarkerTypeID == 0)
                {
                    lock (this)
                    {
                        if (CanAffectClassification(marker))
                            _visibleMarkersWithColor.Remove(marker);
                        else
                            _visibleMarkersNoColor.Remove(marker);
                        _invisibleMarkers.Add(marker);
                    }
                }
                else
                {
                    if (oldType.MarkerTypeID == 0 || newType.MarkerTypeID == 0)
                        return;
                    var flag1 = CanAffectClassification(oldVisualStyle, oldType.LineStyle);
                    var flag2 = CanAffectClassification(newVisualStyle, newType.LineStyle);
                    if (flag1 == flag2)
                        return;
                    lock (this)
                    {
                        if (flag1)
                        {
                            _visibleMarkersWithColor.Remove(marker);
                            _visibleMarkersNoColor.Add(marker);
                        }
                        else
                        {
                            _visibleMarkersNoColor.Remove(marker);
                            _visibleMarkersWithColor.Add(marker);
                        }
                    }
                }
            }

            public IEnumerable<VsTextMarkerAdapter> AllMarkers
            {
                get
                {
                    foreach (var invisibleMarker in _invisibleMarkers)
                        yield return invisibleMarker;
                    foreach (var textMarkerAdapter in _visibleMarkersNoColor)
                        yield return textMarkerAdapter;
                    foreach (var textMarkerAdapter in _visibleMarkersWithColor)
                        yield return textMarkerAdapter;
                }
            }

            public List<VsTextMarkerAdapter> AllMarkersCopy
            {
                get
                {
                    lock (this)
                    {
                        var textMarkerAdapterList = new List<VsTextMarkerAdapter>(_visibleMarkersNoColor.Count + _visibleMarkersWithColor.Count + _invisibleMarkers.Count);
                        textMarkerAdapterList.AddRange(_visibleMarkersNoColor);
                        textMarkerAdapterList.AddRange(_visibleMarkersWithColor);
                        textMarkerAdapterList.AddRange(_invisibleMarkers);
                        return textMarkerAdapterList;
                    }
                }
            }

            public IEnumerable<VsTextMarkerAdapter> AllMarkersOfType(int type)
            {
                if (type == 0)
                {
                    foreach (var invisibleMarker in _invisibleMarkers)
                        yield return invisibleMarker;
                }
                else
                {
                    foreach (var textMarkerAdapter in _visibleMarkersNoColor)
                    {
                        if (textMarkerAdapter.Type == type)
                            yield return textMarkerAdapter;
                    }
                    foreach (var textMarkerAdapter in _visibleMarkersWithColor)
                    {
                        if (textMarkerAdapter.Type == type)
                            yield return textMarkerAdapter;
                    }
                }
            }

            public IEnumerable<VsTextMarkerAdapter> VisibleMarkers
            {
                get
                {
                    foreach (var textMarkerAdapter in _visibleMarkersNoColor)
                        yield return textMarkerAdapter;
                    foreach (var textMarkerAdapter in _visibleMarkersWithColor)
                        yield return textMarkerAdapter;
                }
            }

            public IEnumerable<VsTextMarkerAdapter> VisibleMarkersWithColor
            {
                get
                {
                    foreach (var textMarkerAdapter in _visibleMarkersWithColor)
                        yield return textMarkerAdapter;
                }
            }

            public IEnumerable<VsTextMarkerAdapter> VisibleMarkersNoColor
            {
                get
                {
                    foreach (var textMarkerAdapter in _visibleMarkersNoColor)
                        yield return textMarkerAdapter;
                }
            }

            public IEnumerable<VsTextMarkerAdapter> InvisibleMarkers => _invisibleMarkers;
        }

        internal class MarkerTaggerImplementation : ITagger<VsTextMarkerTag>
        {
            private readonly MarkerManager _manager;

            public MarkerTaggerImplementation(MarkerManager manager)
            {
                _manager = manager;
            }

            public IEnumerable<ITagSpan<VsTextMarkerTag>> GetTags(NormalizedSnapshotSpanCollection spans)
            {
                return _manager.GetMarkerTags(spans);
            }

            public void RaiseTagsChanged(SnapshotSpan span)
            {
                var tagsChanged = TagsChanged;
                tagsChanged?.Invoke(this, new SnapshotSpanEventArgs(span));
            }

            public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
        }

        internal class GlyphTaggerImplementation : ITagger<IGlyphTag>
        {
            private readonly MarkerManager _manager;

            public GlyphTaggerImplementation(MarkerManager manager)
            {
                _manager = manager;
            }

            public IEnumerable<ITagSpan<IGlyphTag>> GetTags(NormalizedSnapshotSpanCollection spans)
            {
                return _manager.GetGlyphTags(spans);
            }

            public void RaiseTagsChanged(SnapshotSpan span)
            {
                var tagsChanged = TagsChanged;
                tagsChanged?.Invoke(this, new SnapshotSpanEventArgs(span));
            }

            public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
        }

        internal class ErrorTaggerImplementation : ITagger<IErrorTag>
        {
            private readonly MarkerManager _manager;

            public ErrorTaggerImplementation(MarkerManager manager)
            {
                _manager = manager;
            }

            public IEnumerable<ITagSpan<IErrorTag>> GetTags(NormalizedSnapshotSpanCollection spans)
            {
                return _manager.GetErrorTags(spans);
            }

            public void RaiseTagsChanged(SnapshotSpan span)
            {
                // ISSUE: reference to a compiler-generated field
                var tagsChanged = TagsChanged;
                tagsChanged?.Invoke(this, new SnapshotSpanEventArgs(span));
            }

            public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
        }

        internal class OverviewTaggerImplementation : ITagger<IOverviewMarkTag>
        {
            private readonly MarkerManager _manager;
            private static HybridDictionary _overviewMarkerMap;

            public static string MapMarkerToOverviewMarkType(MarkerType type)
            {
                var mergeName = type.MergeName;
                if (_overviewMarkerMap == null)
                    _overviewMarkerMap = new HybridDictionary
                    {
            {
              "Tracepoint (Enabled)",
              "MarkerFormatDefinition/BreakpointInScrollBar"
            },
            {
              "Tracepoint (Disabled)",
              "MarkerFormatDefinition/BreakpointInScrollBar"
            },
            {
              "Breakpoint (Enabled)",
              "MarkerFormatDefinition/BreakpointInScrollBar"
            },
            {
              "Breakpoint (Disabled)",
              "MarkerFormatDefinition/BreakpointInScrollBar"
            },
            {
              "Breakpoint - Selected",
              "MarkerFormatDefinition/BreakpointInScrollBar"
            },
            {
              "Breakpoint - Advanced (Enabled)",
              "MarkerFormatDefinition/BreakpointInScrollBar"
            },
            {
              "Breakpoint - Advanced (Disabled)",
              "MarkerFormatDefinition/BreakpointInScrollBar"
            },
            {
              "Bookmark",
              "MarkerFormatDefinition/BookmarkInScrollBar"
            },
            {
              "Bookmark (Disabled)",
              "MarkerFormatDefinition/BookmarkInScrollBar"
            }
          };
                return _overviewMarkerMap[mergeName] as string;
            }

            public OverviewTaggerImplementation(MarkerManager manager)
            {
                _manager = manager;
            }

            public IEnumerable<ITagSpan<IOverviewMarkTag>> GetTags(NormalizedSnapshotSpanCollection spans)
            {
                return _manager.GetOverviewTags(spans);
            }

            public void RaiseTagsChanged(SnapshotSpan span)
            {
                var tagsChanged = TagsChanged;
                tagsChanged?.Invoke(this, new SnapshotSpanEventArgs(span));
            }

            public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
        }

        [Flags]
        public enum MarkerChangedMask
        {
            None = 0,
            GlyphTags = 1,
            Tags = 2,
            TextClassification = 4,
            SquiggleName = 8,
            OverviewMark = 16, // 0x00000010
            All = SquiggleName | TextClassification | Tags | GlyphTags, // 0x0000000F
        }

        private class MarkerEdge
        {
            public readonly VsTextMarkerAdapter Marker;
            public readonly int Position;
            public readonly bool IsStart;

            public MarkerEdge(VsTextMarkerAdapter marker, int position, bool isStart)
            {
                Marker = marker;
                Position = position;
                IsStart = isStart;
            }
        }
    }

    public interface IVsTextMarkerType
    {
        int DrawGlyph(IntPtr hdc, NativeMethods.NativeMethods.RECT[] pRect);
    }

    [Flags]
    public enum ReplaceTextFlags
    {
        RtfDefault = 0,
        RtfKeepMarkers = 1,
        RtfDoNotNormalizeNewlines = 2,
        RtfNormalizeTabsAndSpaces = 4,
        RtfSyntacticReformat = 8,
        RtfClientSuppressFormatting = 16, // 0x00000010
        RtfInterimText = 32, // 0x00000020
    }

    public enum Linestyle
    {
        LiNone,
        LiSolid,
        LiSquiggly,
        LiHatch,
        LiDotted,
    }

    [Flags]
    public enum Findmarkerflags
    {
        FmForward = 0,
        FmBackward = 1,
    }

    public enum Colorindex
    {
        CiSystextFg = 0,
        CiUsertextFg = 0,
        CiSystextBk = 1,
        CiUsertextBk = 1,
        CiBlack = 2,
        CiFirstfixedcolor = 2,
        CiWhite = 3,
        CiMaroon = 4,
        CiDarkgreen = 5,
        CiBrown = 6,
        CiDarkblue = 7,
        CiPurple = 8,
        CiAquamarine = 9,
        CiLightgray = 10, // 0x0000000A
        CiDarkgray = 11, // 0x0000000B
        CiRed = 12, // 0x0000000C
        CiGreen = 13, // 0x0000000D
        CiYellow = 14, // 0x0000000E
        CiBlue = 15, // 0x0000000F
        CiMagenta = 16, // 0x00000010
        CiCyan = 17, // 0x00000011
        CiLastfixedcolor = 17, // 0x00000011
        CiSysselFg = 18, // 0x00000012
        CiSysselBk = 19, // 0x00000013
        CiSysinactselFg = 20, // 0x00000014
        CiSysinactselBk = 21, // 0x00000015
        CiSyswidgetmgnBk = 22, // 0x00000016
        CiSysplaintextFg = 23, // 0x00000017
        CiSysplaintextBk = 24, // 0x00000018
        CiPalettesize = 25, // 0x00000019
        CiForbidcustomization = 26, // 0x0000001A
    }

    [Flags]
    public enum Enummarkerflags
    {
        EmDefault = 0,
        EmEntirebuffer = 1,
        EmAlltypes = 2,
        EmSortdescending = 4,
        EmSortpriority = 8,
        EmGlyphinspan = 16, // 0x00000010
        EmIncludeinvisible = 32, // 0x00000020
        EmContained = 64, // 0x00000040
    }
}