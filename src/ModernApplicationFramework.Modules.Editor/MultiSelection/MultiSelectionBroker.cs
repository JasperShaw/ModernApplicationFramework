using System;
using System.Collections.Generic;
using System.Linq;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Logic.Operations;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Formatting;
using ModernApplicationFramework.Text.Ui.Text;
using Selection = ModernApplicationFramework.Text.Ui.Text.Selection;

namespace ModernApplicationFramework.Modules.Editor.MultiSelection
{
    internal class MultiSelectionBroker : IMultiSelectionBroker
    {
        private readonly List<SelectionTransformer> _selectionTransformers = new List<SelectionTransformer>();
        private int _maxSelections = 1;
        private SelectionTransformer _primaryTransformer;
        private ITextSnapshot _currentSnapshot;
        private IDisposable _batchOperation;
        private bool _fireEvents;
        private bool _isActive;
        private double _boxLeft;
        private double _boxRight;
        private SelectionTransformer _boxSelection;
        internal readonly MultiSelectionBrokerFactory Factory;
        private SelectionTransformer _mergeWinner;
        private bool _activationTracksFocus;
        private IDisposable _completionDisabler;
        private IEditorOptions _editorOptions;
        private SelectionTransformer _standaloneTransformation;

        public MultiSelectionBroker(ITextView textView, MultiSelectionBrokerFactory factory)
        {
            Factory = factory;
            TextView = textView;
            _currentSnapshot = TextView.TextSnapshot;
            _primaryTransformer = new SelectionTransformer(this,
                new Selection(new VirtualSnapshotPoint(TextView.TextSnapshot, 0)));
            _selectionTransformers.Add(_primaryTransformer);
            TextStructureNavigator =
                Factory.TextStructureNavigatorSelectorService.CreateTextStructureNavigator(
                    TextView.TextViewModel.EditBuffer,
                    Factory.ContentTypeRegistryService.GetContentType("text"));
            TextView.LayoutChanged +=
                OnTextViewLayoutChanged;
            TextView.Closed += OnTextViewClosed;
        }

        private IEditorOptions EditorOptions => _editorOptions ?? (_editorOptions =
                                                    Factory.EditorOptionsFactoryService.GetOptions(TextView));

        private void OnTextViewLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            if (CurrentSnapshot == e.NewSnapshot)
                return;
            CurrentSnapshot = e.NewSnapshot;
        }

        private void OnTextViewClosed(object sender, EventArgs e)
        {
        }

        public ITextView TextView { get; }

        public IReadOnlyList<Selection> AllSelections
        {
            get
            {
                var selectionArray = new Selection[_selectionTransformers.Count];
                for (var index = 0; index < _selectionTransformers.Count; ++index)
                    selectionArray[index] = _selectionTransformers[index].Selection;
                return selectionArray;
            }
        }

        public bool HasMultipleSelections => _selectionTransformers.Count > 1;

        internal ITextStructureNavigator TextStructureNavigator { get; }

        public NormalizedSnapshotSpanCollection SelectedSpans
        {
            get
            {
                return new NormalizedSnapshotSpanCollection(
                    _selectionTransformers.Select(
                        c => c.Selection.Extent.SnapshotSpan));
            }
        }

        public IReadOnlyList<VirtualSnapshotSpan> VirtualSelectedSpans
        {
            get
            {
                return _selectionTransformers
                    .Select(
                        c => c.Selection.Extent)
                    .ToArray();
            }
        }

        public bool AreSelectionsActive
        {
            get => _isActive;
            set
            {
                if (value == _isActive)
                    return;
                _isActive = value;
            }
        }

        public bool ActivationTracksFocus
        {
            get => _activationTracksFocus;
            set
            {
                if (_activationTracksFocus == value)
                    return;
                _activationTracksFocus = value;
                if (!_activationTracksFocus)
                    return;
                AreSelectionsActive = TextView.HasAggregateFocus;
            }
        }

        public ITextSnapshot CurrentSnapshot
        {
            get => _currentSnapshot;
            set
            {
                if (_currentSnapshot == value)
                    return;
                using (BeginBatchOperation())
                {
                    _currentSnapshot = value;
                    if (IsBoxSelection)
                    {
                        _boxSelection.CurrentSnapshot = _currentSnapshot;
                        InnerSetBoxSelection();
                    }
                    else
                    {
                        foreach (var t in _selectionTransformers)
                            t.CurrentSnapshot = _currentSnapshot;
                    }
                }
            }
        }

        public Selection PrimarySelection => _primaryTransformer.Selection;

        public VirtualSnapshotSpan SelectionExtent
        {
            get
            {
                if (IsBoxSelection)
                    return _boxSelection.Selection.Extent;
                var selection = _selectionTransformers[0].Selection;
                var start = selection.Start;
                selection = _selectionTransformers[_selectionTransformers.Count - 1].Selection;
                var end = selection.End;
                return new VirtualSnapshotSpan(start, end);
            }
        }

        internal void QueueCaretUpdatedEvent(SelectionTransformer selection)
        {
            if (selection == _standaloneTransformation)
                return;
            if (selection == _boxSelection)
                InnerSetBoxSelection();
            _fireEvents = true;
            FireSessionUpdatedIfNotBatched();
        }

        public event EventHandler MultiSelectionSessionChanged;

        public void SetSelection(Selection selection)
        {
            if (selection.InsertionPoint.Position.Snapshot != _primaryTransformer.CurrentSnapshot)
                throw new ArgumentOutOfRangeException(nameof(selection), "Selection is on an incompatible snapshot");
            if (_boxSelection == null && _selectionTransformers.Count <= 1 &&
                !(_primaryTransformer.Selection != selection))
                return;
            using (BeginBatchOperation())
            {
                ClearSecondarySelections();
                _primaryTransformer.Selection = selection;
                _primaryTransformer.CapturePreferredReferencePoint();
                _fireEvents = true;
            }
        }

        public void SetSelectionRange(IEnumerable<Selection> range, Selection primary)
        {
            using (BeginBatchOperation())
            {
                if (range == null)
                {
                    SetSelection(primary);
                }
                else
                {
                    ClearSecondarySelections();
                    _selectionTransformers.Clear();
                    foreach (var selection in range)
                        InsertSelectionInOrder(selection);
                    var selectionTransformer = (SelectionTransformer) null;
                    foreach (var t in _selectionTransformers)
                    {
                        if (t.Selection == primary)
                        {
                            selectionTransformer = t;
                            break;
                        }
                    }

                    if (selectionTransformer == null)
                        InsertSelectionInOrder(primary);
                    _primaryTransformer = selectionTransformer;
                    _fireEvents = true;
                }
            }
        }

        public void AddSelection(Selection selection)
        {
            using (BeginBatchOperation())
            {
                if (IsBoxSelection)
                    BreakBoxSelection();
                InsertSelectionInOrder(selection);
                _fireEvents = true;
            }
        }

        public void AddSelectionRange(IEnumerable<Selection> range)
        {
            using (BeginBatchOperation())
            {
                if (IsBoxSelection)
                    BreakBoxSelection();
                foreach (var selection in range)
                    InsertSelectionInOrder(selection);
                _fireEvents = true;
            }
        }

        private void InsertSelectionInOrder(Selection selection)
        {
            var selectionTransformer = new SelectionTransformer(this, selection);
            _selectionTransformers.Insert(GenerateInsertionIndex(selection), selectionTransformer);
        }

        private int GenerateInsertionIndex(Selection inserted)
        {
            var index = 0;
            while (index < _selectionTransformers.Count &&
                   !(_selectionTransformers[index].Selection.InsertionPoint >= inserted.InsertionPoint))
                ++index;
            return index;
        }

        public bool IsBoxSelection => _boxSelection != null;

        public Selection BoxSelection => _boxSelection?.Selection ?? Selection.Invalid;

        //public Selection BoxSelection
        //{
        //    get
        //    {
        //        SelectionTransformer boxSelection = this._boxSelection;
        //        if (boxSelection == null)
        //            return Selection.Invalid;
        //        return boxSelection?.Selection;
        //    }
        //}

        public IDisposable BeginBatchOperation()
        {
            if (_batchOperation != null)
            {
                var oldBatchOp = _batchOperation;
                return _batchOperation =
                    new DelegateDisposable((() => _batchOperation = oldBatchOp));
            }

            var highFidelityOperations = new HashSet<IDisposable>();
            foreach (var t in _selectionTransformers)
                highFidelityOperations.Add(t.HighFidelityOperation());

            return _batchOperation = new DelegateDisposable(() =>
            {
                foreach (var disposable in highFidelityOperations)
                    disposable.Dispose();
                _batchOperation = null;
                FireSessionUpdated();
            });
        }

        private void FireSessionUpdatedIfNotBatched()
        {
            if (_batchOperation != null)
                return;
            FireSessionUpdated();
        }

        private void FireSessionUpdated()
        {
            MergeSelections();
            UpdateTelemetryCounters();
            SetCompletionEnableState();
            if (!_fireEvents)
                return;
            _fireEvents = false;
            // ISSUE: reference to a compiler-generated field
            var selectionSessionChanged = MultiSelectionSessionChanged;
            if (selectionSessionChanged == null)
                return;
            Factory.GuardedOperations.RaiseEvent(this, selectionSessionChanged);
        }

        private IFeatureService FeatureService => Factory.FeatureServiceFactory.GetOrCreate(TextView);

        private void SetCompletionEnableState()
        {
            if (HasMultipleSelections)
            {
                if (_completionDisabler != null)
                    return;
                _completionDisabler =
                     FeatureService.Disable("Completion", Factory);
            }
            else
            {
                if (_completionDisabler == null)
                    return;
                _completionDisabler.Dispose();
                _completionDisabler = null;
            }
        }

        private void UpdateTelemetryCounters()
        {
            _maxSelections = Math.Max(_maxSelections, _selectionTransformers.Count);
        }

        private void MergeSelections()
        {
            _selectionTransformers.Sort(
                CompareSelections);
            var index = 1;
            while (index < _selectionTransformers.Count)
            {
                var selectionTransformer1 = _selectionTransformers[index - 1];
                var selectionTransformer2 = _selectionTransformers[index];
                var selection = selectionTransformer1.Selection;
                var insertionPoint1 = selection.InsertionPoint;
                selection = selectionTransformer2.Selection;
                var insertionPoint2 = selection.InsertionPoint;
                if (!(insertionPoint1 == insertionPoint2))
                {
                    selection = selectionTransformer1.Selection;
                    var extent1 = selection.Extent;
                    ref var local1 = ref extent1;
                    selection = selectionTransformer2.Selection;
                    var extent2 = selection.Extent;
                    if (!local1.OverlapsWith(extent2))
                    {
                        selection = selectionTransformer1.Selection;
                        var extent3 = selection.Extent;
                        ref var local2 = ref extent3;
                        selection = selectionTransformer2.Selection;
                        var activePoint1 = selection.ActivePoint;
                        if (!local2.Contains(activePoint1))
                        {
                            selection = selectionTransformer2.Selection;
                            var extent4 = selection.Extent;
                            ref var local3 = ref extent4;
                            selection = selectionTransformer1.Selection;
                            var activePoint2 = selection.ActivePoint;
                            if (!local3.Contains(activePoint2))
                            {
                                ++index;
                                continue;
                            }
                        }
                    }
                }

                SelectionTransformer selectionTransformer3;
                SelectionTransformer selectionTransformer4;
                if (_mergeWinner == selectionTransformer2)
                {
                    selectionTransformer3 = selectionTransformer1;
                    selectionTransformer4 = selectionTransformer2;
                    _selectionTransformers.RemoveAt(index - 1);
                }
                else
                {
                    selectionTransformer3 = selectionTransformer2;
                    selectionTransformer4 = selectionTransformer1;
                    _selectionTransformers.RemoveAt(index);
                }

                VirtualSnapshotPoint anchorPoint;
                VirtualSnapshotPoint activePoint;
                VirtualSnapshotPoint insertionPoint3;
                PositionAffinity insertionPointAffinity;
                if (_mergeWinner == selectionTransformer1 || _mergeWinner == selectionTransformer2)
                {
                    selection = _mergeWinner.Selection;
                    anchorPoint = selection.AnchorPoint;
                    selection = _mergeWinner.Selection;
                    activePoint = selection.ActivePoint;
                    selection = _mergeWinner.Selection;
                    insertionPoint3 = selection.InsertionPoint;
                    selection = _mergeWinner.Selection;
                    insertionPointAffinity = selection.InsertionPointAffinity;
                }
                else
                {
                    var
                        valueTuple =
                            InnerMergeSelections(selectionTransformer1, selectionTransformer2);
                    anchorPoint = valueTuple.Item1;
                    activePoint = valueTuple.Item2;
                    insertionPoint3 = valueTuple.Item3;
                    insertionPointAffinity = valueTuple.Item4;
                }

                selectionTransformer4.Selection =
                    new Selection(insertionPoint3, anchorPoint, activePoint, insertionPointAffinity);
                if (selectionTransformer3 == _primaryTransformer)
                {
                    _primaryTransformer = selectionTransformer4;
                    _fireEvents = true;
                }

                selectionTransformer3.Dispose();
            }

            _mergeWinner = null;
        }

        private static int CompareSelections(SelectionTransformer left, SelectionTransformer right)
        {
            var start1 = left.Selection.Start;
            var start2 = right.Selection.Start;
            if (start1 < start2)
                return -1;
            return !(start1 > start2) ? 0 : 1;
        }

        private static ValueTuple<VirtualSnapshotPoint, VirtualSnapshotPoint, VirtualSnapshotPoint, PositionAffinity>
            InnerMergeSelections(SelectionTransformer first, SelectionTransformer second)
        {
            var positionAffinity = PositionAffinity.Successor;
            var selection1 = first.Selection;
            var start1 = selection1.Start;
            selection1 = second.Selection;
            var start2 = selection1.Start;
            var virtualSnapshotPoint1 =
                start1 < start2 ? first.Selection.Start : second.Selection.Start;
            var selection2 = first.Selection;
            var end1 = selection2.End;
            selection2 = second.Selection;
            var end2 = selection2.End;
            VirtualSnapshotPoint end3;
            if (!(end1 > end2))
            {
                selection2 = second.Selection;
                end3 = selection2.End;
            }
            else
            {
                selection2 = first.Selection;
                end3 = selection2.End;
            }

            var virtualSnapshotPoint2 = end3;
            selection2 = first.Selection;
            VirtualSnapshotPoint virtualSnapshotPoint3;
            VirtualSnapshotPoint virtualSnapshotPoint4;
            VirtualSnapshotPoint virtualSnapshotPoint5;
            if (!(selection2.InsertionPoint == virtualSnapshotPoint1))
            {
                selection2 = second.Selection;
                if (!(selection2.InsertionPoint == virtualSnapshotPoint1))
                {
                    selection2 = first.Selection;
                    if (!(selection2.InsertionPoint == virtualSnapshotPoint2))
                    {
                        selection2 = second.Selection;
                        if (!(selection2.InsertionPoint == virtualSnapshotPoint2))
                        {
                            virtualSnapshotPoint5 = virtualSnapshotPoint1;
                            virtualSnapshotPoint4 = virtualSnapshotPoint2;
                            selection2 = first.Selection;
                            virtualSnapshotPoint3 = selection2.InsertionPoint;
                            goto label_10;
                        }
                    }

                    virtualSnapshotPoint3 = virtualSnapshotPoint2;
                    virtualSnapshotPoint4 = virtualSnapshotPoint2;
                    virtualSnapshotPoint5 = virtualSnapshotPoint1;
                    goto label_10;
                }
            }

            virtualSnapshotPoint3 = virtualSnapshotPoint1;
            virtualSnapshotPoint4 = virtualSnapshotPoint1;
            virtualSnapshotPoint5 = virtualSnapshotPoint2;
            label_10:
            return new ValueTuple<VirtualSnapshotPoint, VirtualSnapshotPoint, VirtualSnapshotPoint, PositionAffinity>(
                virtualSnapshotPoint5, virtualSnapshotPoint4, virtualSnapshotPoint3, positionAffinity);
        }

        public void ClearSecondarySelections()
        {
            if (_boxSelection == null && _selectionTransformers.Count <= 1)
                return;
            using (BeginBatchOperation())
            {
                if (IsBoxSelection)
                {
                    _boxSelection.Dispose();
                    _boxSelection = null;
                }

                foreach (var t in _selectionTransformers)
                {
                    if (t != _primaryTransformer)
                        t.Dispose();
                }

                _selectionTransformers.Clear();
                _selectionTransformers.Add(_primaryTransformer);
                _fireEvents = true;
            }
        }

        public void SetBoxSelection(Selection selection)
        {
            if (_boxSelection != null)
            {
                if (_boxSelection.Selection == selection)
                    return;
                _boxSelection?.Dispose();
            }

            _boxSelection = new SelectionTransformer(this, selection);
            InnerSetBoxSelection();
        }

        private void InnerSetBoxSelection()
        {
            using (BeginBatchOperation())
            {
                var containingBufferPosition1 =
                    TextView.GetTextViewLineContainingBufferPosition(SelectionExtent.Start.Position);
                var containingBufferPosition2 =
                    TextView.GetTextViewLineContainingBufferPosition(SelectionExtent.End.Position);
                foreach (var t in _selectionTransformers)
                    t.Dispose();

                _selectionTransformers.Clear();
                _primaryTransformer = null;
                var extendedCharacterBounds =
                    containingBufferPosition1.GetExtendedCharacterBounds(SelectionExtent.Start);
                var leading1 = extendedCharacterBounds.Leading;
                extendedCharacterBounds =
                    containingBufferPosition2.GetExtendedCharacterBounds(SelectionExtent.End);
                var leading2 = extendedCharacterBounds.Leading;
                _boxLeft = Math.Min(leading1, leading2);
                _boxRight = Math.Max(leading1, leading2);
                extendedCharacterBounds = TextView
                    .GetTextViewLineContainingBufferPosition(_boxSelection.Selection.AnchorPoint.Position)
                    .GetExtendedCharacterBounds(_boxSelection.Selection.AnchorPoint);
                var isReversed = _boxRight == extendedCharacterBounds.Leading;
                var bufferPosition = _boxSelection.Selection.Start.Position;
                var end = _boxSelection.Selection.End;
                var num1 = -1;
                var num2 = -1;
                do
                {
                    var containingBufferPosition3 =
                        TextView.GetTextViewLineContainingBufferPosition(bufferPosition);
                    var selectionSpanOnLine =
                        GetBoxSelectionSpanOnLine(containingBufferPosition3);
                    if (selectionSpanOnLine.HasValue)
                    {
                        var extent = selectionSpanOnLine.Value;
                        var anchorPoint = isReversed ? extent.End : extent.Start;
                        var activePoint = isReversed ? extent.Start : extent.End;
                        var selection = new Selection(extent, isReversed);
                        if (num1 == -1)
                        {
                            if (extent.IntersectsWith(new VirtualSnapshotSpan(
                                _boxSelection.Selection.InsertionPoint,
                                _boxSelection.Selection.InsertionPoint)))
                            {
                                selection = new Selection(_boxSelection.Selection.InsertionPoint, anchorPoint,
                                    activePoint);
                                num1 = _selectionTransformers.Count;
                            }
                            else if (num2 == -1 && extent.IntersectsWith(new VirtualSnapshotSpan(
                                         _boxSelection.Selection.ActivePoint,
                                         _boxSelection.Selection.ActivePoint)))
                                num2 = _selectionTransformers.Count;
                        }

                        _selectionTransformers.Add(new SelectionTransformer(this, selection));
                    }

                    if (containingBufferPosition3.LineBreakLength != 0 ||
                        !containingBufferPosition3.IsLastTextViewLineForSnapshotLine)
                        bufferPosition = containingBufferPosition3.EndIncludingLineBreak;
                    else
                        break;
                } while (bufferPosition.Position <= end.Position.Position ||
                         end.IsInVirtualSpace && bufferPosition.Position == end.Position.Position);

                _primaryTransformer = _selectionTransformers[num1 != -1 ? num1 : num2];
                _fireEvents = true;
            }
        }

        private VirtualSnapshotSpan? GetBoxSelectionSpanOnLine(ITextViewLine line)
        {
            if (!IsBoxSelection)
                return new VirtualSnapshotSpan?();
            if (line == null)
                throw new ArgumentNullException(nameof(line));
            if (line.Snapshot != _currentSnapshot)
                throw new ArgumentException("The supplied ITextViewLine is on an incorrect snapshot.", nameof(line));
            if (SelectionExtent.IsEmpty)
            {
                var activePoint = _boxSelection.Selection.ActivePoint;
                if (line.ContainsBufferPosition(activePoint.Position))
                    return new VirtualSnapshotSpan(activePoint, activePoint);
            }
            else
            {
                var start = SelectionExtent.Start;
                if (SelectionExtent.End.Position.Position >= line.Start &&
                    start.Position.Position <= line.End)
                {
                    var positionFromXcoordinate1 =
                        line.GetInsertionBufferPositionFromXCoordinate(_boxLeft);
                    var positionFromXcoordinate2 =
                        line.GetInsertionBufferPositionFromXCoordinate(_boxRight);
                    if (positionFromXcoordinate1 <= positionFromXcoordinate2)
                        return new VirtualSnapshotSpan(positionFromXcoordinate1,
                            positionFromXcoordinate2);
                    return new VirtualSnapshotSpan(positionFromXcoordinate2,
                        positionFromXcoordinate1);
                }
            }

            return new VirtualSnapshotSpan?();
        }

        public IReadOnlyList<VirtualSnapshotSpan> GetSelectionsOnTextViewLine(ITextViewLine line)
        {
            var extent = line.Extent;
            var start = new VirtualSnapshotPoint(extent.Start);
            extent = line.Extent;
            var end = new VirtualSnapshotPoint(extent.End, int.MaxValue);
            var lineSpan = new VirtualSnapshotSpan(start, end);
            return GetSelectionsIntersectingSpan(line.Extent)
                .Where(caret => caret.Extent.IntersectsWith(lineSpan))
                .Select(
                    caret => caret.Extent.Intersection(lineSpan).Value)
                .ToArray();
        }

        public bool TryRemoveSelection(Selection region)
        {
            if (!HasMultipleSelections)
                return false;
            using (BeginBatchOperation())
            {
                if (IsBoxSelection)
                    BreakBoxSelection();
                var selectionTransformer =
                    _selectionTransformers.FirstOrDefault(
                        transformer => transformer.Selection == region);
                if (selectionTransformer != null)
                {
                    _selectionTransformers.Remove(selectionTransformer);
                    if (_primaryTransformer.Selection == region)
                        _primaryTransformer = _selectionTransformers.First();
                    _fireEvents = true;
                    selectionTransformer.Dispose();
                }

                return selectionTransformer != null;
            }
        }

        public IReadOnlyList<Selection> GetSelectionsIntersectingSpans(NormalizedSnapshotSpanCollection spanCollection)
        {
            return _selectionTransformers
                .Where(transformer =>
                    spanCollection.IntersectsWith(transformer.Selection.Extent.SnapshotSpan))
                .Select(
                    transformer => transformer.Selection)
                .ToArray();
        }

        public IReadOnlyList<Selection> GetSelectionsIntersectingSpan(SnapshotSpan span)
        {
            return _selectionTransformers
                .Where(transformer =>
                    span.IntersectsWith(transformer.Selection.Extent.SnapshotSpan))
                .Select(
                    transformer => transformer.Selection)
                .ToArray();
        }

        public void BreakBoxSelection()
        {
            _boxSelection.Dispose();
            _boxSelection = null;
        }

        private bool TryBrokerHandledManipulation(PredefinedSelectionTransformations action)
        {
            switch (action)
            {
                case PredefinedSelectionTransformations.MoveToNextCaretPosition:
                    return TryMoveToNextCaretPosision();
                case PredefinedSelectionTransformations.MoveToPreviousCaretPosition:
                    return TryMoveToPreviousCaretPosition();
                case PredefinedSelectionTransformations.MovePageUp:
                case PredefinedSelectionTransformations.SelectPageUp:
                case PredefinedSelectionTransformations.MovePageDown:
                case PredefinedSelectionTransformations.SelectPageDown:
                    ClearSecondarySelections();
                    _batchOperation.Dispose();
                    return false;
                case PredefinedSelectionTransformations.MoveToStartOfDocument:
                case PredefinedSelectionTransformations.SelectToStartOfDocument:
                case PredefinedSelectionTransformations.MoveToEndOfDocument:
                case PredefinedSelectionTransformations.SelectToEndOfDocument:
                    ClearSecondarySelections();
                    return false;
                default:
                    return false;
            }
        }

        private bool TryMoveToPreviousCaretPosition()
        {
            if (!IsBoxSelection)
                return false;
            var start = _boxSelection.Selection.Start;
            ClearSecondarySelections();
            _primaryTransformer.MoveTo(start, false, PositionAffinity.Successor);
            return true;
        }

        private bool TryMoveToNextCaretPosision()
        {
            if (!IsBoxSelection)
                return false;
            var end = _boxSelection.Selection.End;
            ClearSecondarySelections();
            _primaryTransformer.MoveTo(end, false, PositionAffinity.Successor);
            return true;
        }

        public void PerformActionOnAllSelections(PredefinedSelectionTransformations action)
        {
            using (BeginBatchOperation())
            {
                if (TryBrokerHandledManipulation(action))
                    return;
                if (IsBoxSelection)
                {
                    _boxSelection.PerformAction(action);
                    if (!IsDestructiveToBoxSelection(action))
                        return;
                    ClearSecondarySelections();
                }
                else
                {
                    foreach (var t in _selectionTransformers)
                        t.PerformAction(action);
                }
            }
        }

        private static bool IsDestructiveToBoxSelection(PredefinedSelectionTransformations action)
        {
            switch (action)
            {
                case PredefinedSelectionTransformations.MoveToNextCaretPosition:
                case PredefinedSelectionTransformations.MoveToPreviousCaretPosition:
                case PredefinedSelectionTransformations.MoveToNextWord:
                case PredefinedSelectionTransformations.MoveToPreviousWord:
                case PredefinedSelectionTransformations.MoveToBeginningOfLine:
                case PredefinedSelectionTransformations.MoveToHome:
                case PredefinedSelectionTransformations.MoveToEndOfLine:
                case PredefinedSelectionTransformations.MoveToNextLine:
                case PredefinedSelectionTransformations.MoveToPreviousLine:
                case PredefinedSelectionTransformations.MovePageUp:
                case PredefinedSelectionTransformations.MovePageDown:
                case PredefinedSelectionTransformations.MoveToStartOfDocument:
                case PredefinedSelectionTransformations.MoveToEndOfDocument:
                    return true;
                default:
                    return false;
            }
        }

        public void PerformActionOnAllSelections(Action<ISelectionTransformer> action)
        {
            using (BeginBatchOperation())
            {
                if (IsBoxSelection)
                {
                    action(_boxSelection);
                }
                else
                {
                    foreach (var t in _selectionTransformers)
                        action( t);
                }
            }
        }

        public bool TryPerformActionOnSelection(Selection before, PredefinedSelectionTransformations action,
            out Selection after)
        {
            var selectionTransformer = (SelectionTransformer) null;
            if (_boxSelection != null && before == _boxSelection.Selection)
                selectionTransformer = _boxSelection;
            if (selectionTransformer == null)
            {
                foreach (var t in _selectionTransformers)
                {
                    if (t.Selection == before || t
                            .HistoricalRegions.Contains(before))
                    {
                        selectionTransformer = t;
                        break;
                    }
                }

                _mergeWinner = selectionTransformer;
            }

            using (BeginBatchOperation())
            {
                selectionTransformer.PerformAction(action);
                after = selectionTransformer.Selection;
                return true;
            }
        }

        public bool TryPerformActionOnSelection(Selection before, Action<ISelectionTransformer> action,
            out Selection after)
        {
            var selectionTransformer = (SelectionTransformer) null;
            if (_boxSelection != null && before == _boxSelection.Selection)
                selectionTransformer = _boxSelection;
            if (selectionTransformer == null)
            {
                selectionTransformer = _selectionTransformers.FirstOrDefault(
                    m =>
                    {
                        if (!(m.Selection == before))
                            return m.HistoricalRegions.Contains(before);
                        return true;
                    });
                _mergeWinner = selectionTransformer;
            }

            if (selectionTransformer == null)
            {
                after = new Selection();
                return false;
            }

            using (BeginBatchOperation())
                action(selectionTransformer);
            after = selectionTransformer.Selection;
            return true;
        }

        public bool TrySetAsPrimarySelection(Selection candidate)
        {
            var selectionTransformer =
                _selectionTransformers.FirstOrDefault(
                    m => m.Selection == candidate);
            if (selectionTransformer == null)
                return false;
            using (BeginBatchOperation())
            {
                if (_primaryTransformer != selectionTransformer)
                    _fireEvents = true;
                _primaryTransformer = selectionTransformer;
                return true;
            }
        }

        public bool TryEnsureVisible(Selection region, EnsureSpanVisibleOptions options)
        {
            if (_selectionTransformers.FirstOrDefault(
                    m => m.Selection == region) == null)
                return false;
            var options1 =
                options & (EnsureSpanVisibleOptions.MinimumScroll | EnsureSpanVisibleOptions.AlwaysCenter) |
                (region.IsReversed ? EnsureSpanVisibleOptions.ShowStart : EnsureSpanVisibleOptions.None);
            TextView.ViewScroller.EnsureSpanVisible(region.Extent, options1);
            if (region.InsertionPoint != region.ActivePoint)
                TextView.ViewScroller.EnsureSpanVisible(
                    new VirtualSnapshotSpan(region.InsertionPoint, region.InsertionPoint),
                    EnsureSpanVisibleOptions.MinimumScroll);
            return true;
        }

        public bool TryGetSelectionPresentationProperties(Selection region,
            out AbstractSelectionPresentationProperties properties)
        {
            var selectionTransformer = (SelectionTransformer) null;
            if (_boxSelection != null && region == _boxSelection.Selection)
                selectionTransformer = _boxSelection;
            if (selectionTransformer == null)
                selectionTransformer = _selectionTransformers.FirstOrDefault(
                    m => m.Selection == region || m.HistoricalRegions.Contains(region));
            if (selectionTransformer == null)
            {
                properties = null;
                return false;
            }

            properties = selectionTransformer.UiProperties;
            return true;
        }

        public Selection TransformSelection(Selection source, PredefinedSelectionTransformations transformation)
        {
            var selectionTransformer = new SelectionTransformer(this, source);
            _standaloneTransformation = selectionTransformer;
            selectionTransformer.PerformAction(transformation);
            _standaloneTransformation = null;
            return selectionTransformer.Selection;
        }
    }
}