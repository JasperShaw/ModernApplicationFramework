using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Differencing;
using ModernApplicationFramework.Text.Data.Projection;
using ModernApplicationFramework.TextEditor.Text;
using ModernApplicationFramework.TextEditor.Text.Differencing;
using ModernApplicationFramework.TextEditor.Utilities;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.TextEditor
{
    internal sealed class ProjectionBuffer : BaseProjectionBuffer, IProjectionBuffer
    {
        internal ITextBuffer LiteralBuffer;
        private readonly ProjectionBufferOptions _bufferOptions;
        private readonly IDifferenceService _differenceService;
        private readonly List<WeakEventHook> _eventHooks = new List<WeakEventHook>();
        private readonly SourceBufferSet _sourceBufferSet = new SourceBufferSet();
        private readonly List<ITrackingSpan> _sourceSpans = new List<ITrackingSpan>();
        private readonly IInternalTextBufferFactory _textBufferFactory;
        private ProjectionSnapshot _currentProjectionSnapshot;
        private IReadOnlyRegion _literalBufferRor;

        public event EventHandler<ProjectionSourceBuffersChangedEventArgs> SourceBuffersChanged;

        public event EventHandler<ProjectionSourceSpansChangedEventArgs> SourceSpansChanged;

        internal event EventHandler<ProjectionSourceBuffersChangedEventArgs> SourceBuffersChangedImmediate;

        internal event EventHandler<ProjectionSourceSpansChangedEventArgs> SourceSpansChangedImmediate;

        public override IProjectionSnapshot CurrentSnapshot => _currentProjectionSnapshot;

        public override IList<ITextBuffer> SourceBuffers => _sourceBufferSet.SourceBuffers;

        protected override BaseProjectionSnapshot CurrentBaseSnapshot => _currentProjectionSnapshot;

        public ProjectionBuffer(IInternalTextBufferFactory textBufferFactory, IProjectionEditResolver resolver,
            IContentType contentType, IList<object> initialSourceSpans, IDifferenceService differenceService,
            ITextDifferencingService textDifferencingService, ProjectionBufferOptions options,
            GuardedOperations guardedOperations)
            : base(resolver, contentType, textDifferencingService, guardedOperations)
        {
            _textBufferFactory = textBufferFactory;
            _differenceService = differenceService;
            _bufferOptions = options;
            var spanManager = new SpanManager(this, 0, 0, initialSourceSpans, false, false);
            spanManager.PerformChecks();
            spanManager.ProcessLiteralSpans();
            var num = 0;
            _sourceBufferSet.StartTransaction();
            var length = 0;
            var snapshotSpanList = new List<SnapshotSpan>();
            foreach (var sourceSpan in spanManager.SpansToInsert)
            {
                AddSpan(num++, sourceSpan);
                var snapshotSpan = new SnapshotSpan(sourceSpan.TextBuffer.CurrentSnapshot,
                    sourceSpan.GetSpan(sourceSpan.TextBuffer.CurrentSnapshot));
                length += snapshotSpan.Length;
                snapshotSpanList.Add(snapshotSpan);
            }

            var stringRebuilder = StringRebuilder.Empty;
            foreach (var t in snapshotSpanList)
                stringRebuilder = stringRebuilder.Append(BufferFactoryService.StringRebuilderFromSnapshotSpan(t));

            Builder = stringRebuilder;
            CurrentVersion.SetLength(length);
            _sourceBufferSet.FinishTransaction(out var addedBuffers, out _);
            var flag = true;
            BufferGroup bufferGroup = null;
            foreach (BaseBuffer sourceBuffer in addedBuffers)
            {
                if (flag)
                {
                    flag = false;
                    bufferGroup = sourceBuffer.Group;
                    bufferGroup.AddMember(this);
                }
                else
                {
                    bufferGroup.Swallow(sourceBuffer.Group);
                }

                if (sourceBuffer != LiteralBuffer || (_bufferOptions & ProjectionBufferOptions.WritableLiteralSpans) !=
                    ProjectionBufferOptions.None)
                    _eventHooks.Add(new WeakEventHook(this, sourceBuffer));
            }

            Group = bufferGroup ?? new BufferGroup(this);
            _currentProjectionSnapshot = new ProjectionSnapshot(this, CurrentVersion, Builder, snapshotSpanList);
            CurrentSnapshotProtected = _currentProjectionSnapshot;
        }

        public override ITextEdit CreateEdit(EditOptions options, int? reiteratedVersionNumber, object editTag)
        {
            return new ProjectionEdit(this, _currentProjectionSnapshot, options, reiteratedVersionNumber, editTag);
        }

        public IProjectionSnapshot DeleteSpans(int position, int spansToDelete)
        {
            return ReplaceSpans(position, spansToDelete, new List<object>(0), EditOptions.None, null);
        }

        public IProjectionSnapshot InsertSpan(int position, ITrackingSpan spanToInsert)
        {
            var position1 = position;
            var objectList = new List<object>(1) {spanToInsert};
            var none = EditOptions.None;
            return ReplaceSpans(position1, 0, objectList, none, null);
        }

        public IProjectionSnapshot InsertSpan(int position, string literalSpanToInsert)
        {
            var position1 = position;
            var objectList = new List<object>(1) {literalSpanToInsert};
            var none = EditOptions.None;
            return ReplaceSpans(position1, 0, objectList, none, null);
        }

        public IProjectionSnapshot InsertSpans(int position, IList<object> spansToInsert)
        {
            return ReplaceSpans(position, 0, spansToInsert, EditOptions.None, null);
        }

        public override ITextEventRaiser PropagateSourceChanges(EditOptions options, object editTag)
        {
            var textEventRaiserList = InterpretSourceChanges(options, editTag);
            textEventRaiserList[0].RaiseEvent(this, true);
            return textEventRaiserList[0];
        }

        public IProjectionSnapshot ReplaceSpans(int position, int spansToReplace, IList<object> spansToInsert,
            EditOptions options, object editTag)
        {
            using (var spanEdit = new SpanEdit(this))
            {
                return spanEdit.ReplaceSpans(position, spansToReplace, spansToInsert, options, editTag);
            }
        }

        internal override void OnSourceTextChanged(object sender, TextContentChangedEventArgs e)
        {
            PendingContentChangedEventArgs.Add(e);
            if (EditApplicationInProgress)
                return;
            Group.ScheduleIndependentEdit(this);
        }

        protected internal override ISubordinateTextEdit CreateSubordinateEdit(EditOptions options,
            int? reiteratedVersionNumber, object editTag)
        {
            return new ProjectionEdit(this, _currentProjectionSnapshot, options, reiteratedVersionNumber, editTag);
        }

        protected override StringRebuilder GetDoppelgangerBuilder()
        {
            if (Properties.TryGetProperty<ITextBuffer>("IdentityMapping", out var property))
            {
                var currentSnapshot = property.CurrentSnapshot;
                return BufferFactoryService.StringRebuilderFromSnapshotAndSpan(currentSnapshot,
                    new Span(0, currentSnapshot.Length));
            }

            if (_sourceSpans.Count == 1)
            {
                var sourceSpan = _sourceSpans[0];
                var span = sourceSpan.GetSpan(sourceSpan.TextBuffer.CurrentSnapshot);
                if (span.Length == CurrentVersion.Length && span.Snapshot is BaseSnapshot)
                    return BufferFactoryService.StringRebuilderFromSnapshotSpan(span);
            }

            return null;
        }

        protected override BaseSnapshot TakeSnapshot()
        {
            var newSourceSpans = new List<SnapshotSpan>(_sourceSpans.Count);
            foreach (var sourceSpan in _sourceSpans)
                newSourceSpans.Add(sourceSpan.GetSpan(sourceSpan.TextBuffer.CurrentSnapshot));
            _currentProjectionSnapshot = MakeSnapshot(newSourceSpans);
            return _currentProjectionSnapshot;
        }

        private static void CorrectSpanAdjustments(List<TextChange> ordinaryChanges,
            SortedDictionary<int, SpanAdjustment> spanPreAdjustments,
            SortedDictionary<int, SpanAdjustment> spanPostAdjustments)
        {
            var frugalList1 = new FrugalList<TextChange>(TextUtilities.StableSort(ordinaryChanges,
                (left, right) => left.OldPosition - right.OldPosition));
            var frugalList2 = Load(spanPreAdjustments);
            var frugalList3 = Load(spanPostAdjustments);
            var num1 = frugalList1.Count + frugalList2.Count + frugalList3.Count;
            var textChange = new TextChange(int.MaxValue, StringRebuilder.Empty, StringRebuilder.Empty,
                LineBreakBoundaryConditions.None);
            frugalList1.Add(textChange);
            frugalList2.Add(new Tuple<TextChange, int>(textChange, 0));
            frugalList3.Add(new Tuple<TextChange, int>(textChange, 0));
            var num2 = 0;
            var num3 = 0;
            var num4 = 0;
            var index1 = 0;
            var index2 = 0;
            var index3 = 0;
            var oldPosition1 = frugalList1[0].OldPosition;
            var oldPosition2 = frugalList2[0].Item1.OldPosition;
            var oldPosition3 = frugalList3[0].Item1.OldPosition;
            for (var index4 = 0; index4 < num1; ++index4)
                if (oldPosition3 <= oldPosition2 && oldPosition3 <= oldPosition1)
                {
                    frugalList3[index3].Item1.OldPosition += num3 + num2 + num4 - frugalList3[index3].Item2;
                    frugalList3[index3].Item1.NewPosition = frugalList3[index3].Item1.OldPosition;
                    num4 += frugalList3[index3].Item1.Delta;
                    oldPosition3 = frugalList3[++index3].Item1.OldPosition;
                }
                else if (oldPosition1 <= oldPosition2)
                {
                    frugalList1[index1].OldPosition += num3;
                    frugalList1[index1].NewPosition = frugalList1[index1].OldPosition;
                    num2 += frugalList1[index1].Delta;
                    oldPosition1 = frugalList1[++index1].OldPosition;
                }
                else
                {
                    frugalList2[index2].Item1.OldPosition += num3 - frugalList2[index2].Item2;
                    frugalList2[index2].Item1.NewPosition = frugalList2[index2].Item1.OldPosition;
                    num3 += frugalList2[index2].Item1.Delta;
                    oldPosition2 = frugalList2[++index2].Item1.OldPosition;
                }
        }

        private static SpanAdjustment GetAdjustment(SortedDictionary<int, SpanAdjustment> spanAdjustments,
            int spanPosition)
        {
            if (!spanAdjustments.TryGetValue(spanPosition, out var spanAdjustment))
            {
                spanAdjustment = new SpanAdjustment();
                spanAdjustments.Add(spanPosition, spanAdjustment);
            }

            return spanAdjustment;
        }

        private static StringRebuilder InsertionLiesInPermissiveInclusiveSpan(ITextSnapshot afterSourceSnapshot,
            SnapshotSpan rawSpan, Span deletionSpan, int sourcePosition, int renormalizedSourcePosition,
            ITextChange incomingChange, ISet<SnapshotPoint> urPoints)
        {
            var flag = sourcePosition < rawSpan.Start && deletionSpan.End >= rawSpan.Start;
            if ((sourcePosition == rawSpan.Start) | flag)
            {
                var position = new SnapshotPoint(afterSourceSnapshot, renormalizedSourcePosition);
                var firstMatchNoTrack = MappingHelper.MapDownToFirstMatchNoTrack(position,
                    buffer => buffer is TextBuffer, (PositionAffinity) 1);
                urPoints.Add(firstMatchNoTrack.Value);
                return TextChange.NewStringRebuilder(incomingChange);
            }

            if (sourcePosition == rawSpan.End)
            {
                var position = new SnapshotPoint(afterSourceSnapshot, renormalizedSourcePosition);
                var firstMatchNoTrack =
                    MappingHelper.MapDownToFirstMatchNoTrack(position, buffer => buffer is TextBuffer, 0);
                urPoints.Add(firstMatchNoTrack.Value);
                return TextChange.NewStringRebuilder(incomingChange);
            }

            if (!rawSpan.Contains(sourcePosition))
                return StringRebuilder.Empty;
            return TextChange.NewStringRebuilder(incomingChange);
        }

        private static FrugalList<Tuple<TextChange, int>> Load(SortedDictionary<int, SpanAdjustment> adjustments)
        {
            var frugalList = new FrugalList<Tuple<TextChange, int>>();
            foreach (var spanAdjustment in adjustments.Values)
            {
                var num = 0;
                if (spanAdjustment.LeadingChange != null)
                {
                    frugalList.Add(new Tuple<TextChange, int>(spanAdjustment.LeadingChange, 0));
                    num = spanAdjustment.LeadingChange.Delta;
                }

                if (spanAdjustment.TrailingChange != null)
                    frugalList.Add(new Tuple<TextChange, int>(spanAdjustment.TrailingChange, num));
            }

            return frugalList;
        }

        private void AddSpan(int position, ITrackingSpan sourceSpan)
        {
            _sourceSpans.Insert(position, sourceSpan);
            _sourceBufferSet.AddSpan(sourceSpan);
        }

        private ProjectionSourceSpansChangedEventArgs ApplySpanChanges(int position, int spansToDelete,
            IList<ITrackingSpan> spansToInsert, EditOptions options, object editTag)
        {
            var projectionSnapshot = _currentProjectionSnapshot;
            var trackingSpanList = new List<ITrackingSpan>();
            var insertedSnapSpans = new List<SnapshotSpan>();
            var deletedSnapSpans = new List<SnapshotSpan>();
            _sourceBufferSet.StartTransaction();
            for (var position1 = position + spansToDelete - 1; position1 >= position; --position1)
            {
                var trackingSpan = RemoveSpan(position1);
                trackingSpanList.Insert(0, trackingSpan);
                deletedSnapSpans.Insert(0, _currentProjectionSnapshot.GetSourceSpan(position1));
            }

            var num = position;
            foreach (var sourceSpan in spansToInsert)
            {
                AddSpan(num++, sourceSpan);
                insertedSnapSpans.Add(sourceSpan.GetSpan(sourceSpan.TextBuffer.CurrentSnapshot));
            }

            _sourceBufferSet.FinishTransaction(out var addedBuffers, out var removedBuffers);
            foreach (BaseBuffer baseBuffer in addedBuffers)
                Group.Swallow(baseBuffer.Group);
            var textPosition = 0;
            for (var position1 = 0; position1 < position; ++position1)
                textPosition += _currentProjectionSnapshot.GetSourceSpan(position1).Length;
            SetCurrentVersionAndSnapshot(
                !options.ComputeMinimalChange
                    ? ComputeTextChangesBySpanDiffing(textPosition, deletedSnapSpans, insertedSnapSpans)
                    : ComputeTextChangesByStringDiffing(options.DifferenceOptions, textPosition, deletedSnapSpans,
                        insertedSnapSpans), -1);
            ProjectionSourceSpansChangedEventArgs changedEventArgs;
            if (addedBuffers.Count > 0 || removedBuffers.Count > 0)
            {
                foreach (var textBuffer in addedBuffers)
                    if (textBuffer != LiteralBuffer ||
                        (_bufferOptions & ProjectionBufferOptions.WritableLiteralSpans) != ProjectionBufferOptions.None)
                        _eventHooks.Add(new WeakEventHook(this, (BaseBuffer) textBuffer));
                foreach (var textBuffer in removedBuffers)
                    if (textBuffer != LiteralBuffer ||
                        (_bufferOptions & ProjectionBufferOptions.WritableLiteralSpans) != ProjectionBufferOptions.None)
                    {
                        var baseBuffer = (BaseBuffer) textBuffer;
                        for (var index = 0; index < _eventHooks.Count; ++index)
                        {
                            var eventHook = _eventHooks[index];
                            if (eventHook.SourceBuffer == baseBuffer)
                            {
                                eventHook.UnsubscribeFromSourceBuffer();
                                _eventHooks.RemoveAt(index);
                                break;
                            }
                        }
                    }

                changedEventArgs = new ProjectionSourceBuffersChangedEventArgs(projectionSnapshot,
                    _currentProjectionSnapshot, spansToInsert, trackingSpanList, position, addedBuffers, removedBuffers,
                    options, editTag);
            }
            else
            {
                changedEventArgs = new ProjectionSourceSpansChangedEventArgs(projectionSnapshot,
                    _currentProjectionSnapshot, spansToInsert, trackingSpanList, position, options, editTag);
            }

            return changedEventArgs;
        }

        private INormalizedTextChangeCollection ComputeProjectedChanges(
            SortedDictionary<int, SpanAdjustment> spanPreAdjustments,
            SortedDictionary<int, SpanAdjustment> spanPostAdjustments)
        {
            var tupleList = PreparePendingChanges();
            var textChangeList1 = new List<TextChange>();
            var urPoints = new HashSet<SnapshotPoint>();
            for (var index1 = tupleList.Count - 1; index1 >= 0; --index1)
            {
                var tuple = tupleList[index1];
                var textChangeList2 = tuple.Item2;
                var accumulatedDelta = 0;
                for (var index2 = 0; index2 < textChangeList2.Count - 1; ++index2)
                {
                    InterpretSourceBufferChange(tuple.Item1, textChangeList2[index2], textChangeList1, urPoints,
                        spanPreAdjustments, spanPostAdjustments, accumulatedDelta);
                    accumulatedDelta += textChangeList2[index2].Delta;
                }
            }

            if (EditApplicationInProgress && (spanPreAdjustments.Count > 0 || spanPostAdjustments.Count > 0))
                CorrectSpanAdjustments(textChangeList1, spanPreAdjustments, spanPostAdjustments);
            return NormalizedTextChangeCollection.Create(textChangeList1);
        }

        private void ComputeSourceEdits(IEnumerable<TextChange> changes)
        {
            foreach (var change in changes)
                if (change.OldLength > 0 && change.NewLength == 0)
                {
                    foreach (var sourceSnapshot in (IEnumerable<SnapshotSpan>) _currentProjectionSnapshot
                        .MapToSourceSnapshots(new Span(change.NewPosition, change.OldLength)))
                        DeleteFromSource(sourceSnapshot);
                }
                else if (change.OldLength > 0 && change.NewLength > 0)
                {
                    var sourceSnapshots = _currentProjectionSnapshot.MapReplacementSpanToSourceSnapshots(
                        new Span(change.OldPosition, change.OldLength),
                        (_bufferOptions & ProjectionBufferOptions.WritableLiteralSpans) == ProjectionBufferOptions.None
                            ? LiteralBuffer
                            : null);
                    var frugalList = new FrugalList<SnapshotSpan>();
                    foreach (var snapshotSpan in sourceSnapshots)
                        if (!snapshotSpan.Snapshot.TextBuffer.IsReadOnly(snapshotSpan.Span, true))
                            frugalList.Add(snapshotSpan);
                    if (frugalList.Count == 1)
                    {
                        ReplaceInSource(frugalList[0], change.NewText, change.MasterChangeOffset);
                    }
                    else
                    {
                        var numArray = new int[frugalList.Count];
                        if (Resolver != null)
                        {
                            Resolver.FillInReplacementSizes(
                                new SnapshotSpan(_currentProjectionSnapshot, change.OldPosition, change.OldLength),
                                new ReadOnlyCollection<SnapshotSpan>(frugalList), change.NewText, numArray);
                            if (BufferGroup.Tracing)
                            {
                                var num = 0;
                                while (num < frugalList.Count)
                                    ++num;
                            }
                        }

                        numArray[numArray.Length - 1] = int.MaxValue;
                        var start = 0;
                        for (var index = 0; index < numArray.Length; ++index)
                        {
                            var length = Math.Min(numArray[index], change.NewLength - start);
                            if (length > 0)
                            {
                                ReplaceInSource(frugalList[index], TextChange.ChangeNewSubstring(change, start, length),
                                    start + change.MasterChangeOffset);
                                start += length;
                            }
                            else if (frugalList[index].Length > 0)
                            {
                                DeleteFromSource(frugalList[index]);
                            }
                        }
                    }
                }
                else
                {
                    var sourceSnapshots = _currentProjectionSnapshot.MapInsertionPointToSourceSnapshots(
                        change.NewPosition,
                        (_bufferOptions & ProjectionBufferOptions.WritableLiteralSpans) == ProjectionBufferOptions.None
                            ? LiteralBuffer
                            : null);
                    var frugalList = new FrugalList<SnapshotPoint>();
                    foreach (var snapshotPoint in sourceSnapshots)
                        if (!snapshotPoint.Snapshot.TextBuffer.IsReadOnly(snapshotPoint.Position, true))
                            frugalList.Add(snapshotPoint);
                    if (frugalList.Count == 1)
                    {
                        InsertInSource(frugalList[0], change.NewText, change.MasterChangeOffset);
                    }
                    else
                    {
                        var numArray = new int[frugalList.Count];
                        Resolver?.FillInInsertionSizes(
                            new SnapshotPoint(_currentProjectionSnapshot, change.NewPosition),
                            new ReadOnlyCollection<SnapshotPoint>(frugalList), change.NewText, numArray);
                        numArray[numArray.Length - 1] = int.MaxValue;
                        var start = 0;
                        for (var index = 0; index < numArray.Length; ++index)
                        {
                            var length = Math.Min(numArray[index], change.NewLength - start);
                            if (length > 0)
                            {
                                InsertInSource(frugalList[index], change._newText.GetText(new Span(start, length)),
                                    start + change.MasterChangeOffset);
                                start += length;
                                if (start == change.NewLength)
                                    break;
                            }
                        }
                    }
                }

            EditApplicationInProgress = true;
        }

        private INormalizedTextChangeCollection ComputeTextChangesBySpanDiffing(int textPosition,
            List<SnapshotSpan> deletedSnapSpans, List<SnapshotSpan> insertedSnapSpans)
        {
            return new ProjectionSpanToNormalizedChangeConverter(
                new ProjectionSpanDiffer(_differenceService, deletedSnapSpans.AsReadOnly(),
                    insertedSnapSpans.AsReadOnly()), textPosition, _currentProjectionSnapshot).NormalizedChanges;
        }

        private INormalizedTextChangeCollection ComputeTextChangesByStringDiffing(
            StringDifferenceOptions differenceOptions, int textPosition, List<SnapshotSpan> deletedSnapSpans,
            List<SnapshotSpan> insertedSnapSpans)
        {
            var stringBuilder1 = new StringBuilder();
            foreach (var deletedSnapSpan in deletedSnapSpans)
                stringBuilder1.Append(deletedSnapSpan.GetText());
            var stringBuilder2 = new StringBuilder();
            foreach (var insertedSnapSpan in insertedSnapSpans)
                stringBuilder2.Append(insertedSnapSpan.GetText());
            var textChangeList = new List<TextChange>();
            if (stringBuilder1.Length > 0 || stringBuilder2.Length > 0)
                textChangeList.Add(TextChange.Create(textPosition, stringBuilder1.ToString(), stringBuilder2.ToString(),
                    _currentProjectionSnapshot));
            return NormalizedTextChangeCollection.Create(textChangeList, differenceOptions, TextDifferencingService);
        }

        private void DeleteFromSource(SnapshotSpan deletionSpan)
        {
            Group.GetEdit((BaseBuffer) deletionSpan.Snapshot.TextBuffer).Delete(deletionSpan);
        }

        private void InsertInSource(SnapshotPoint sourceInsertionPoint, string text, int masterChangeOffset)
        {
            var edit = Group.GetEdit((BaseBuffer) sourceInsertionPoint.Snapshot.TextBuffer);
            edit.Insert(sourceInsertionPoint.Position, text);
            ((ISubordinateTextEdit) edit).RecordMasterChangeOffset(masterChangeOffset);
        }

        private StringRebuilder InsertionLiesInCustomSpan(ITextSnapshot afterSourceSnapshot, int spanPosition,
            ITextChange incomingChange, int accumulatedDelta)
        {
            var span = _sourceSpans[spanPosition].GetSpan(afterSourceSnapshot);
            var nullable =
                new Span(incomingChange.NewPosition + accumulatedDelta, incomingChange.NewLength).Overlap(span);
            return !nullable.HasValue
                ? StringRebuilder.Empty
                : BufferFactoryService.StringRebuilderFromSnapshotAndSpan(afterSourceSnapshot, nullable.Value);
        }

        private StringRebuilder InsertionLiesInSpan(ITextSnapshot afterSourceSnapshot, int projectedPosition,
            int spanPosition, SnapshotSpan rawSpan, Span deletionSpan, int sourcePosition, SpanTrackingMode mode,
            ITextChange incomingChange, HashSet<SnapshotPoint> urPoints,
            SortedDictionary<int, SpanAdjustment> spanAdjustments, int accumulatedDelta)
        {
            var num1 = sourcePosition + accumulatedDelta;
            if (mode == SpanTrackingMode.Custom)
                return InsertionLiesInCustomSpan(afterSourceSnapshot, spanPosition, incomingChange, accumulatedDelta);
            var flag1 = rawSpan.Contains(sourcePosition);
            if (mode == SpanTrackingMode.EdgeInclusive)
            {
                if ((_bufferOptions & ProjectionBufferOptions.PermissiveEdgeInclusiveSourceSpans) !=
                    ProjectionBufferOptions.None)
                    return InsertionLiesInPermissiveInclusiveSpan(afterSourceSnapshot, rawSpan, deletionSpan,
                        sourcePosition, num1, incomingChange, urPoints);
                if (!flag1 && sourcePosition != rawSpan.End)
                    return StringRebuilder.Empty;
                return TextChange.NewStringRebuilder(incomingChange);
            }

            if (!EditInProgressProtected)
            {
                bool flag2;
                switch (mode)
                {
                    case SpanTrackingMode.EdgePositive:
                        flag2 = sourcePosition != rawSpan.Start && (flag1 || sourcePosition == rawSpan.End);
                        break;
                    case SpanTrackingMode.EdgeNegative:
                        flag2 = flag1;
                        break;
                    default:
                        flag2 = flag1 && sourcePosition != rawSpan.Start;
                        break;
                }

                if (!flag2)
                    return StringRebuilder.Empty;
                return TextChange.NewStringRebuilder(incomingChange);
            }

            if (sourcePosition == rawSpan.Start && mode != SpanTrackingMode.EdgeNegative)
            {
                var position = new SnapshotPoint(afterSourceSnapshot, num1);
                var firstMatchNoTrack = MappingHelper.MapDownToFirstMatchNoTrack(position,
                    buffer => buffer is TextBuffer, (PositionAffinity) 1);
                if (urPoints.Add(firstMatchNoTrack.Value))
                    GetAdjustment(spanAdjustments, spanPosition).LeadingChange = TextChange.Create(projectedPosition,
                        StringRebuilder.Empty, TextChange.NewStringRebuilder(incomingChange),
                        _currentProjectionSnapshot);
                return StringRebuilder.Empty;
            }

            if (sourcePosition == rawSpan.End && mode != SpanTrackingMode.EdgePositive)
            {
                var position = new SnapshotPoint(afterSourceSnapshot, num1);
                var firstMatchNoTrack = MappingHelper.MapDownToFirstMatchNoTrack(position,
                    buffer => buffer is TextBuffer, 0);
                if (urPoints.Add(firstMatchNoTrack.Value))
                    GetAdjustment(spanAdjustments, spanPosition).TrailingChange = TextChange.Create(projectedPosition,
                        StringRebuilder.Empty, TextChange.NewStringRebuilder(incomingChange),
                        _currentProjectionSnapshot);
                return StringRebuilder.Empty;
            }

            if (!flag1 && (mode != SpanTrackingMode.EdgePositive || sourcePosition != rawSpan.End))
                return StringRebuilder.Empty;
            return TextChange.NewStringRebuilder(incomingChange);
        }

        private void InterpretSourceBufferChange(ITextBuffer changedBuffer, ITextChange change,
            List<TextChange> projectedChanges, HashSet<SnapshotPoint> urPoints,
            SortedDictionary<int, SpanAdjustment> spanPreAdjustments,
            SortedDictionary<int, SpanAdjustment> spanPostAdjustments, int accumulatedDelta)
        {
            var projectionSnapshot = _currentProjectionSnapshot;
            var newPosition = change.NewPosition;
            var deletionSpan = new Span(newPosition, change.OldLength);
            var num1 = change.NewLength;
            var num2 = 0;
            var num3 = 0;
            var currentSnapshot = changedBuffer.CurrentSnapshot;
            foreach (var sourceSpan1 in _sourceSpans)
            {
                var sourceSpan2 = projectionSnapshot.GetSourceSpan(num3);
                if (sourceSpan1.TextBuffer == changedBuffer)
                {
                    var trackingMode = sourceSpan1.TrackingMode;
                    var nullable = deletionSpan.Overlap(sourceSpan2);
                    if (nullable.HasValue && nullable.Value.Length > 0)
                    {
                        var num4 = num2 + nullable.Value.Start - sourceSpan2.Start;
                        var oldText = TextChange.ChangeOldSubText(change,
                            Math.Max(sourceSpan2.Start.Position - deletionSpan.Start, 0), nullable.Value.Length);
                        var newText = StringRebuilder.Empty;
                        var rawSpan = new SnapshotSpan(sourceSpan2.Snapshot, sourceSpan2.Start,
                            sourceSpan2.Length - oldText.Length);
                        if (sourceSpan1.TrackingMode != SpanTrackingMode.EdgeInclusive &&
                            sourceSpan1.TrackingMode != SpanTrackingMode.Custom && EditInProgressProtected)
                        {
                            if (sourceSpan1.TrackingMode != SpanTrackingMode.EdgeNegative &&
                                nullable.Value.Start == sourceSpan2.Start)
                            {
                                GetAdjustment(spanPreAdjustments, num3).LeadingChange = TextChange.Create(num4, oldText,
                                    string.Empty, _currentProjectionSnapshot);
                                oldText = StringRebuilder.Empty;
                            }
                            else if (sourceSpan1.TrackingMode != SpanTrackingMode.EdgePositive &&
                                     nullable.Value.End == sourceSpan2.End)
                            {
                                GetAdjustment(spanPreAdjustments, num3).TrailingChange = TextChange.Create(num4,
                                    oldText, string.Empty, _currentProjectionSnapshot);
                                oldText = StringRebuilder.Empty;
                            }
                        }

                        if (change.NewLength > 0)
                        {
                            newText = InsertionLiesInSpan(currentSnapshot, num4, num3, rawSpan, deletionSpan,
                                newPosition, trackingMode, change, urPoints, spanPostAdjustments, accumulatedDelta);
                            if (newText.Length > 0)
                                num1 = change.NewLength - newText.Length;
                        }

                        if (oldText.Length > 0 || newText.Length > 0)
                        {
                            var textChange = TextChange.Create(num4, oldText, newText, _currentProjectionSnapshot);
                            projectedChanges.Add(textChange);
                        }
                    }
                    else if (num1 > 0)
                    {
                        var num4 = num2 + Math.Max(newPosition - sourceSpan2.Start, 0);
                        var num5 = spanPostAdjustments?.Count ?? 0;
                        var newText = InsertionLiesInSpan(currentSnapshot, num4, num3, sourceSpan2, deletionSpan,
                            newPosition, trackingMode, change, urPoints, spanPostAdjustments, accumulatedDelta);
                        if (newText.Length > 0)
                        {
                            var textChange = TextChange.Create(num4, string.Empty, newText, _currentProjectionSnapshot);
                            projectedChanges.Add(textChange);
                        }

                        if (spanPostAdjustments != null && spanPostAdjustments.Count != num5)
                            num1 = 0;
                    }
                }

                num2 += sourceSpan2.Length;
                ++num3;
            }
        }

        private IList<ITextEventRaiser> InterpretSourceChanges(EditOptions options, object editTag)
        {
            var textEventRaiserList = new List<ITextEventRaiser>();
            var spanPreAdjustments = new SortedDictionary<int, SpanAdjustment>();
            var spanPostAdjustments = new SortedDictionary<int, SpanAdjustment>();
            var projectedChanges = ComputeProjectedChanges(spanPreAdjustments, spanPostAdjustments);
            foreach (var keyValuePair in spanPreAdjustments)
                textEventRaiserList.Add(PerformSpanAdjustments(keyValuePair.Key, keyValuePair.Value, true, editTag));
            var projectionSnapshot = (ITextSnapshot) _currentProjectionSnapshot;
            SetCurrentVersionAndSnapshot(projectedChanges);
            textEventRaiserList.Add(new TextContentChangedEventRaiser(projectionSnapshot, _currentProjectionSnapshot,
                options, editTag));
            foreach (var keyValuePair in spanPostAdjustments)
                textEventRaiserList.Add(PerformSpanAdjustments(keyValuePair.Key, keyValuePair.Value, false, editTag));
            EditApplicationInProgress = false;
            return textEventRaiserList;
        }

        private ProjectionSnapshot MakeSnapshot(IList<SnapshotSpan> newSourceSpans)
        {
            return new ProjectionSnapshot(this, CurrentVersion, Builder, newSourceSpans);
        }

        private SourceSpansChangedEventRaiser PerformSpanAdjustments(int spanPosition, SpanAdjustment adjustment,
            bool beforeBaseSnapshot, object editTag)
        {
            var projectionSnapshot = _currentProjectionSnapshot;
            var newSourceSpans = new List<SnapshotSpan>(projectionSnapshot.GetSourceSpans());
            _sourceBufferSet.StartTransaction();
            var trackingSpan1 = RemoveSpan(spanPosition);
            var sourceSpan = projectionSnapshot.GetSourceSpans(spanPosition, 1)[0];
            var start = (int) sourceSpan.Start;
            var end = (int) sourceSpan.End;
            var textChangeList = new List<TextChange>();
            if (adjustment.LeadingChange != null)
            {
                textChangeList.Add(adjustment.LeadingChange);
                if (beforeBaseSnapshot)
                    start += adjustment.LeadingChange.OldLength;
                else
                    start -= adjustment.LeadingChange.NewLength;
            }

            if (adjustment.TrailingChange != null)
            {
                textChangeList.Add(adjustment.TrailingChange);
                if (beforeBaseSnapshot)
                    end -= adjustment.TrailingChange.OldLength;
                else
                    end += adjustment.TrailingChange.NewLength;
            }

            var trackingSpan2 =
                sourceSpan.Snapshot.CreateTrackingSpan(Span.FromBounds(start, end), trackingSpan1.TrackingMode);
            AddSpan(spanPosition, trackingSpan2);
            newSourceSpans[spanPosition] = new SnapshotSpan(sourceSpan.Snapshot, Span.FromBounds(start, end));
            _sourceBufferSet.FinishTransaction(out _, out _);
            var changeCollection = NormalizedTextChangeCollection.Create(textChangeList);
            CurrentVersion = CurrentVersion.CreateNext(changeCollection);
            Builder = ApplyChangesToStringRebuilder(changeCollection, Builder);
            if (beforeBaseSnapshot)
                _currentProjectionSnapshot = TakeStaticSnapshot(newSourceSpans);
            else
                CurrentSnapshotProtected = TakeSnapshot();
            return new SourceSpansChangedEventRaiser(new ProjectionSourceSpansChangedEventArgs(projectionSnapshot,
                _currentProjectionSnapshot, new[]
                {
                    trackingSpan2
                }, new[]
                {
                    trackingSpan1
                }, spanPosition, EditOptions.None, editTag));
        }

        private List<Tuple<ITextBuffer, List<TextChange>>> PreparePendingChanges()
        {
            var tupleList = new List<Tuple<ITextBuffer, List<TextChange>>>();
            foreach (var contentChangedEventArg in PendingContentChangedEventArgs)
            {
                var sourceBuffer = contentChangedEventArg.Before.TextBuffer;
                var tuple = tupleList.Find(p => p.Item1 == sourceBuffer);
                List<TextChange> denormChangesWithSentinel;
                if (tuple == null)
                {
                    denormChangesWithSentinel = new List<TextChange>(1)
                    {
                        new TextChange(int.MaxValue, StringRebuilder.Empty, StringRebuilder.Empty,
                            LineBreakBoundaryConditions.None)
                    };
                    tupleList.Add(new Tuple<ITextBuffer, List<TextChange>>(sourceBuffer, denormChangesWithSentinel));
                }
                else
                {
                    denormChangesWithSentinel = tuple.Item2;
                }

                NormalizedTextChangeCollection.Denormalize(contentChangedEventArg.Changes, denormChangesWithSentinel);
            }

            PendingContentChangedEventArgs.Clear();
            return tupleList;
        }

        private ITrackingSpan RemoveSpan(int position)
        {
            var sourceSpan = _sourceSpans[position];
            _sourceSpans.RemoveAt(position);
            _sourceBufferSet.RemoveSpan(sourceSpan);
            return sourceSpan;
        }

        private void ReplaceInSource(SnapshotSpan deletionSpan, string insertionText, int masterChangeOffset)
        {
            var edit = Group.GetEdit((BaseBuffer) deletionSpan.Snapshot.TextBuffer);
            edit.Replace(deletionSpan, insertionText);
            ((ISubordinateTextEdit) edit).RecordMasterChangeOffset(masterChangeOffset);
        }

        private ProjectionSnapshot TakeStaticSnapshot(List<SnapshotSpan> newSourceSpans)
        {
            return MakeSnapshot(newSourceSpans);
        }

        private class LiteralBufferHelper
        {
            private readonly bool _groupEdit;
            private readonly ProjectionBuffer _projBuffer;
            private readonly bool _readOnly;
            private int _insertionPoint;
            private bool _performedEdit;
            private int _totalDeletions;
            private int _totalInsertions;

            public LiteralBufferHelper(ProjectionBuffer projBuffer, bool readOnly, bool groupEdit)
            {
                _projBuffer = projBuffer;
                _readOnly = readOnly;
                _groupEdit = groupEdit;
                if (_projBuffer.LiteralBuffer == null)
                    return;
                _insertionPoint = _projBuffer.LiteralBuffer.CurrentSnapshot.Length;
            }

            public Span Append(string text)
            {
                PrepareEdit();
                Span span;
                if (_groupEdit)
                {
                    _projBuffer.Group.GetEdit((BaseBuffer) _projBuffer.LiteralBuffer, EditOptions.None)
                        .Insert(_insertionPoint, text);
                    span = new Span(_insertionPoint + _totalInsertions - _totalDeletions, text.Length);
                    _totalInsertions += text.Length;
                }
                else
                {
                    var length = _projBuffer.LiteralBuffer.CurrentSnapshot.Length;
                    _projBuffer.LiteralBuffer.Insert(length, text);
                    span = new Span(length, text.Length);
                }

                return span;
            }

            public void Delete(ITrackingSpan trackingSpan)
            {
                PrepareEdit();
                var span = (Span) trackingSpan.GetSpan(_projBuffer.LiteralBuffer.CurrentSnapshot);
                if (_groupEdit)
                {
                    _projBuffer.Group.GetEdit((BaseBuffer) _projBuffer.LiteralBuffer, EditOptions.None).Delete(span);
                    _totalDeletions += span.Length;
                }
                else
                {
                    _projBuffer.LiteralBuffer.Delete(span);
                }
            }

            public void FinishEdit()
            {
                if (!_performedEdit || !_readOnly || _projBuffer.LiteralBuffer == null)
                    return;
                using (var readOnlyRegionEdit = _projBuffer.LiteralBuffer.CreateReadOnlyRegionEdit())
                {
                    _projBuffer._literalBufferRor = readOnlyRegionEdit.CreateReadOnlyRegion(
                        new Span(0, readOnlyRegionEdit.Snapshot.Length), SpanTrackingMode.EdgeInclusive,
                        EdgeInsertionMode.Deny);
                    readOnlyRegionEdit.Apply();
                }
            }

            private void PrepareEdit()
            {
                if (_projBuffer.LiteralBuffer == null)
                {
                    _projBuffer.LiteralBuffer = _projBuffer._textBufferFactory.CreateTextBuffer(string.Empty,
                        _projBuffer._textBufferFactory.InertContentType, _readOnly);
                    _insertionPoint = 0;
                }
                else if (_projBuffer._literalBufferRor != null)
                {
                    using (var readOnlyRegionEdit = _projBuffer.LiteralBuffer.CreateReadOnlyRegionEdit())
                    {
                        readOnlyRegionEdit.RemoveReadOnlyRegion(_projBuffer._literalBufferRor);
                        readOnlyRegionEdit.Apply();
                    }

                    _projBuffer._literalBufferRor = null;
                }

                _performedEdit = true;
            }
        }

        private class ProjectionEdit : Edit, ISubordinateTextEdit
        {
            private readonly ProjectionBuffer _projectionBuffer;
            private bool _subordinate;

            public ITextBuffer TextBuffer => _projectionBuffer;

            public ProjectionEdit(ProjectionBuffer projectionBuffer, ITextSnapshot originSnapshot, EditOptions options,
                int? reiteratedVersionNumber, object editTag)
                : base(projectionBuffer, originSnapshot, options, reiteratedVersionNumber, editTag)
            {
                _projectionBuffer = projectionBuffer;
                _subordinate = true;
            }

            public override void CancelApplication()
            {
                if (Canceled)
                    return;
                base.CancelApplication();
                _projectionBuffer.EditApplicationInProgress = false;
                _projectionBuffer.PendingContentChangedEventArgs.Clear();
            }

            public void FinalApply()
            {
                if (Changes.Count > 0 || _projectionBuffer.PendingContentChangedEventArgs.Count > 0)
                {
                    _projectionBuffer.Group.CancelIndependentEdit(_projectionBuffer);
                    var textEventRaiserList = _projectionBuffer.InterpretSourceChanges(Options, EditTag);
                    _projectionBuffer.Group.EnqueueEvents(textEventRaiserList, BaseBuffer);
                    foreach (var textEventRaiser in textEventRaiserList)
                        textEventRaiser.RaiseEvent(BaseBuffer, true);
                }

                _projectionBuffer.EditInProgressProtected = false;
                if (!_subordinate)
                    return;
                BaseBuffer.Group.FinishEdit();
            }

            public void PreApply()
            {
                if (Changes.Count <= 0)
                    return;
                _projectionBuffer.ComputeSourceEdits(Changes);
            }

            protected override ITextSnapshot PerformApply()
            {
                CheckActive();
                Applied = true;
                _subordinate = false;
                var currentSnapshot = (ITextSnapshot) BaseBuffer.CurrentSnapshotProtected;
                if (Changes.Count > 0)
                {
                    _projectionBuffer.Group.PerformMasterEdit(_projectionBuffer, this, Options, EditTag);
                    if (!Canceled)
                        currentSnapshot = BaseBuffer.CurrentSnapshotProtected;
                }
                else
                {
                    BaseBuffer.EditInProgressProtected = false;
                }

                return currentSnapshot;
            }
        }

        private class SourceBufferSet
        {
            private readonly List<BufferTracker> _sourceBufferTrackers = new List<BufferTracker>();
            private FrugalList<ITextBuffer> _addedBuffers;
            private FrugalList<ITextBuffer> _removedBuffers;

            public List<ITextBuffer> SourceBuffers
            {
                get
                {
                    var textBufferList = new List<ITextBuffer>();
                    foreach (var sourceBufferTracker in _sourceBufferTrackers)
                        if (!textBufferList.Contains(sourceBufferTracker.Buffer))
                            textBufferList.Add(sourceBufferTracker.Buffer);
                    return textBufferList;
                }
            }

            public void AddSpan(ITrackingSpan span)
            {
                var index = Find(span.TextBuffer);
                if (index < 0)
                {
                    _sourceBufferTrackers.Add(new BufferTracker(span.TextBuffer));
                    _addedBuffers.Add(span.TextBuffer);
                }
                else
                {
                    ++_sourceBufferTrackers[index].SpanCount;
                }
            }

            public void FinishTransaction(out FrugalList<ITextBuffer> addedBuffers,
                out FrugalList<ITextBuffer> removedBuffers)
            {
                var frugalList = new FrugalList<ITextBuffer>();
                foreach (var addedBuffer in _addedBuffers)
                    if (_removedBuffers.Contains(addedBuffer))
                        frugalList.Add(addedBuffer);
                foreach (var textBuffer in frugalList)
                {
                    _addedBuffers.Remove(textBuffer);
                    _removedBuffers.Remove(textBuffer);
                }

                addedBuffers = _addedBuffers;
                removedBuffers = _removedBuffers;
                _addedBuffers = null;
                _removedBuffers = null;
            }

            public void RemoveSpan(ITrackingSpan span)
            {
                var index = Find(span.TextBuffer);
                if (--_sourceBufferTrackers[index].SpanCount != 0)
                    return;
                _sourceBufferTrackers.RemoveAt(index);
                _removedBuffers.Add(span.TextBuffer);
            }

            public void StartTransaction()
            {
                _addedBuffers = new FrugalList<ITextBuffer>();
                _removedBuffers = new FrugalList<ITextBuffer>();
            }

            private int Find(ITextBuffer buffer)
            {
                for (var index = 0; index < _sourceBufferTrackers.Count; ++index)
                    if (buffer == _sourceBufferTrackers[index].Buffer)
                        return index;
                return -1;
            }

            private class BufferTracker
            {
                public readonly ITextBuffer Buffer;
                public int SpanCount;

                public BufferTracker(ITextBuffer buffer)
                {
                    Buffer = buffer;
                    SpanCount = 1;
                }
            }
        }

        private class SourceSpansChangedEventRaiser : ITextEventRaiser
        {
            private readonly ProjectionSourceSpansChangedEventArgs _args;

            public bool HasPostEvent => true;

            public SourceSpansChangedEventRaiser(ProjectionSourceSpansChangedEventArgs args)
            {
                _args = args;
            }

            public void RaiseEvent(BaseBuffer baseBuffer, bool immediate)
            {
                var projectionBuffer = (ProjectionBuffer) baseBuffer;
                if (_args is ProjectionSourceBuffersChangedEventArgs args)
                {
                    var eventHandler = immediate
                        ? projectionBuffer.SourceBuffersChangedImmediate
                        : projectionBuffer.SourceBuffersChanged;
                    eventHandler?.Invoke(baseBuffer, args);
                }

                var eventHandler1 = immediate
                    ? projectionBuffer.SourceSpansChangedImmediate
                    : projectionBuffer.SourceSpansChanged;
                eventHandler1?.Invoke(baseBuffer, _args);
                baseBuffer.RawRaiseEvent(_args, immediate);
            }
        }

        private class SpanAdjustment
        {
            public TextChange LeadingChange;
            public TextChange TrailingChange;
        }

        private class SpanEdit : TextBufferBaseEdit, ISubordinateTextEdit
        {
            private readonly ProjectionBuffer _projBuffer;
            private EditOptions _editOptions = EditOptions.None;
            private SpanManager _spanManager;
            private object _tag;

            public ITextBuffer TextBuffer => _projBuffer;

            public SpanEdit(ProjectionBuffer projBuffer)
                : base(projBuffer)
            {
                _projBuffer = projBuffer;
            }

            public bool CheckForCancellation(Action cancelAction)
            {
                return true;
            }

            public void FinalApply()
            {
                var changedEventRaiser = new SourceSpansChangedEventRaiser(
                    _projBuffer.ApplySpanChanges(_spanManager.Position, _spanManager.SpansToDelete,
                        _spanManager.SpansToInsert, _editOptions, _tag));
                BaseBuffer.Group.EnqueueEvents(changedEventRaiser, BaseBuffer);
                changedEventRaiser.RaiseEvent(BaseBuffer, true);
                BaseBuffer.EditInProgressProtected = false;
                _projBuffer.EditApplicationInProgress = false;
                if ((_projBuffer._bufferOptions & ProjectionBufferOptions.WritableLiteralSpans) ==
                    ProjectionBufferOptions.None)
                    return;
                _projBuffer.PendingContentChangedEventArgs.Clear();
            }

            public void PreApply()
            {
                _projBuffer.EditApplicationInProgress = true;
                _spanManager.ProcessLiteralSpans();
            }

            public void RecordMasterChangeOffset(int masterChangeOffset)
            {
                throw new InvalidOperationException("Projection span edits shouldn't have change offsets.");
            }

            public IProjectionSnapshot ReplaceSpans(int position, int spansToReplace, IList<object> spansToInsert,
                EditOptions options, object editTag)
            {
                if (position < 0 || position > _projBuffer._sourceSpans.Count)
                    throw new ArgumentOutOfRangeException(nameof(position));
                if (spansToReplace < 0 || position + spansToReplace > _projBuffer._sourceSpans.Count)
                    throw new ArgumentOutOfRangeException(nameof(spansToReplace));
                if (spansToInsert == null)
                    throw new ArgumentNullException(nameof(spansToInsert));
                _spanManager = new SpanManager(_projBuffer, position, spansToReplace, spansToInsert, true,
                    (uint) (_projBuffer._bufferOptions & ProjectionBufferOptions.WritableLiteralSpans) > 0U);
                _editOptions = options;
                _tag = editTag;
                _spanManager.PerformChecks();
                Applied = true;
                if (_spanManager.SpansToDelete > 0 || _spanManager.RawSpansToInsert.Count > 0)
                    _projBuffer.Group.PerformMasterEdit(_projBuffer, this, _editOptions, _tag);
                BaseBuffer.Group.FinishEdit();
                BaseBuffer.EditInProgressProtected = false;
                return _projBuffer._currentProjectionSnapshot;
            }

            public override string ToString()
            {
                var stringBuilder = new StringBuilder();
                for (var index = 0; index < _spanManager.RawSpansToInsert.Count; ++index)
                {
                    if (_spanManager.RawSpansToInsert[index] is ITrackingSpan trackingSpan)
                        stringBuilder.Append(trackingSpan);
                    else
                        stringBuilder.Append("(Literal)");
                    if (index < _spanManager.RawSpansToInsert.Count - 1)
                        stringBuilder.Append(",");
                }

                return string.Format(CultureInfo.CurrentCulture, "pos: {0}, delete: {1}, insert: {2}",
                    _spanManager.Position, _spanManager.SpansToDelete, stringBuilder.ToString());
            }
        }

        private class SpanManager
        {
            private readonly bool _checkForCycles;
            private readonly LiteralBufferHelper _lit;
            private readonly ProjectionBuffer _projBuffer;
            private readonly Dictionary<ITextBuffer, bool> _visitedBufferSet = new Dictionary<ITextBuffer, bool>();
            private List<ITrackingSpan> _spansToInsert;

            public int Position { get; }

            public List<object> RawSpansToInsert { get; }

            public int SpansToDelete { get; }

            public List<ITrackingSpan> SpansToInsert
            {
                get
                {
                    if (_spansToInsert != null) return _spansToInsert;
                    _lit.FinishEdit();
                    _spansToInsert = new List<ITrackingSpan>();
                    foreach (var obj in RawSpansToInsert)
                        _spansToInsert.Add(obj as ITrackingSpan ??
                                           _projBuffer.LiteralBuffer.CurrentSnapshot.CreateTrackingSpan((Span) obj,
                                               SpanTrackingMode.EdgeExclusive, TrackingFidelityMode.Forward));
                    return _spansToInsert;
                }
            }

            public SpanManager(ProjectionBuffer projBuffer, int position, int spansToDelete,
                IList<object> rawSpansToInsert, bool checkForCycles, bool groupEdit)
            {
                _projBuffer = projBuffer;
                _checkForCycles = checkForCycles;
                _lit = new LiteralBufferHelper(projBuffer,
                    (_projBuffer._bufferOptions & ProjectionBufferOptions.WritableLiteralSpans) ==
                    ProjectionBufferOptions.None, groupEdit);
                Position = position;
                SpansToDelete = spansToDelete;
                RawSpansToInsert = new List<object>(rawSpansToInsert);
            }

            public void PerformChecks()
            {
                foreach (var obj in RawSpansToInsert)
                {
                    if (obj == null)
                        throw new ArgumentNullException("spansToInsert");
                    if (obj is ITrackingSpan spanToInsert)
                    {
                        if (_checkForCycles)
                            try
                            {
                                CheckForSourceBufferCycle(spanToInsert.TextBuffer);
                            }
                            catch (ArgumentException)
                            {
                                throw new ArgumentException();
                            }

                        if ((_projBuffer._bufferOptions & ProjectionBufferOptions.PermissiveEdgeInclusiveSourceSpans) ==
                            ProjectionBufferOptions.None)
                            CheckTrackingMode(spanToInsert);
                    }
                    else if (!(obj is string))
                    {
                        throw new ArgumentException();
                    }
                }

                CheckOverlap();
            }

            public void ProcessLiteralSpans()
            {
                for (var position = Position; position < Position + SpansToDelete; ++position)
                {
                    var sourceSpan = _projBuffer._sourceSpans[position];
                    if (sourceSpan.TextBuffer == _projBuffer.LiteralBuffer)
                        _lit.Delete(sourceSpan);
                }

                for (var index = 0; index < RawSpansToInsert.Count; ++index)
                    if (RawSpansToInsert[index] is string text)
                        RawSpansToInsert[index] = _lit.Append(text);
            }

            private static IEnumerable<SnapshotSpan> BaseSourceSpans(SnapshotSpan span)
            {
                var snapshotSpanList = new List<SnapshotSpan>();
                if (span.Snapshot.TextBuffer is IProjectionBuffer)
                    foreach (var sourceSnapshot in ((IProjectionSnapshot) span.Snapshot).MapToSourceSnapshots(span))
                        snapshotSpanList.AddRange(BaseSourceSpans(sourceSnapshot));
                else
                    snapshotSpanList.Add(span);
                return snapshotSpanList;
            }

            private static void CheckTrackingMode(ITrackingSpan spanToInsert)
            {
                if (spanToInsert.TrackingMode == SpanTrackingMode.EdgeExclusive ||
                    spanToInsert.TrackingMode == SpanTrackingMode.Custom)
                    return;
                var currentSnapshot = spanToInsert.TextBuffer.CurrentSnapshot;
                var span = (Span) spanToInsert.GetSpan(currentSnapshot);
                if (spanToInsert.TrackingMode == SpanTrackingMode.EdgeInclusive)
                {
                    if (span.Start > 0 || span.End < currentSnapshot.Length)
                        throw new ArgumentException();
                }
                else if (spanToInsert.TrackingMode == SpanTrackingMode.EdgePositive)
                {
                    if (span.End < currentSnapshot.Length)
                        throw new ArgumentException();
                }
                else if (span.Start > 0)
                {
                    throw new ArgumentException();
                }
            }

            private void CheckForSourceBufferCycle(ITextBuffer buffer)
            {
                if (_visitedBufferSet.ContainsKey(buffer))
                    return;
                if (buffer == _projBuffer)
                    throw new ArgumentException();
                _visitedBufferSet.Add(buffer, true);
                var projectionBuffer = (IProjectionBuffer) (buffer as ProjectionBuffer);
                if (projectionBuffer == null)
                    return;
                foreach (var sourceBuffer in projectionBuffer.SourceBuffers)
                    CheckForSourceBufferCycle(sourceBuffer);
            }

            private void CheckOverlap()
            {
                var dictionary = new Dictionary<ITextBuffer, List<Span>>();
                foreach (var proposedSpan in GetProposedSpans())
                foreach (var baseSourceSpan in BaseSourceSpans(
                    proposedSpan.GetSpan(proposedSpan.TextBuffer.CurrentSnapshot)))
                {
                    var textBuffer = baseSourceSpan.Snapshot.TextBuffer;
                    if (!dictionary.TryGetValue(textBuffer, out var spanList))
                    {
                        spanList = new List<Span>();
                        dictionary.Add(textBuffer, spanList);
                    }

                    spanList.Add(baseSourceSpan);
                }

                foreach (var keyValuePair in dictionary)
                    if (keyValuePair.Value.Count > 1)
                    {
                        keyValuePair.Value.Sort((x, y) =>
                        {
                            if (x.Start != y.Start)
                                return x.Start - y.Start;
                            return x.End - y.End;
                        });
                        for (var index = 1; index < keyValuePair.Value.Count; ++index)
                        {
                            var span = keyValuePair.Value[index];
                            var start = span.Start;
                            span = keyValuePair.Value[index - 1];
                            var end = span.End;
                            if (start < end)
                                throw new ArgumentException();
                        }
                    }
            }

            private IEnumerable<ITrackingSpan> GetProposedSpans()
            {
                int s;
                for (s = 0; s < Position; ++s)
                    yield return _projBuffer._sourceSpans[s];
                for (s = Position + SpansToDelete; s < _projBuffer._sourceSpans.Count; ++s)
                    yield return _projBuffer._sourceSpans[s];
                for (s = 0; s < RawSpansToInsert.Count; ++s)
                    if (RawSpansToInsert[s] is ITrackingSpan trackingSpan)
                        yield return trackingSpan;
            }
        }
    }
}