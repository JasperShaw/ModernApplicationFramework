using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Logic.Tagging;
using ModernApplicationFramework.Text.Ui.Classification;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Formatting;

namespace ModernApplicationFramework.Modules.Editor.Implementation
{
    internal class TextSelection : ITextSelection
    {
        internal NormalizedSnapshotSpanCollection _selectedSpans;
        internal List<VirtualSnapshotSpan> _virtualSelectedSpans;
        internal SimpleTagger<ClassificationTag> BoxTagger;
        internal ISelectionPainter FocusedPainter;
        internal IAdornmentLayer SelectionAdornmentLayer;
        internal ISelectionPainter UnfocusedPainter;
        private readonly IEditorFormatMap _editorFormatMap;
        private readonly GuardedOperations _guardedOperations;
        private bool _activationTracksFocus;
        private VirtualSnapshotPoint _activePoint;
        private VirtualSnapshotPoint _anchorPoint;
        private bool _isActive;
        private double _leftX;
        private double _rightX;
        private TextSelectionMode _selectionMode;

        public event EventHandler SelectionChanged;

        public bool ActivationTracksFocus
        {
            set
            {
                if (_activationTracksFocus == value)
                    return;
                _activationTracksFocus = value;
                if (!_activationTracksFocus)
                    return;
                IsActive = TextView.HasAggregateFocus;
            }
            get => _activationTracksFocus;
        }

        public VirtualSnapshotPoint ActivePoint
        {
            get
            {
                if (!IsEmpty)
                    return _activePoint;
                return TextView.Caret.Position.VirtualBufferPosition;
            }
        }

        public VirtualSnapshotPoint AnchorPoint
        {
            get
            {
                if (!IsEmpty)
                    return _anchorPoint;
                return TextView.Caret.Position.VirtualBufferPosition;
            }
        }

        public VirtualSnapshotPoint End
        {
            get
            {
                if (!IsReversed)
                    return ActivePoint;
                return AnchorPoint;
            }
        }

        public bool IsActive
        {
            set
            {
                if (_isActive == value)
                    return;
                Painter?.Clear();
                _isActive = value;
                Painter?.Activate();
            }
            get => _isActive;
        }

        public bool IsEmpty => _activePoint == _anchorPoint;

        public bool IsReversed => _activePoint < _anchorPoint;

        public TextSelectionMode Mode
        {
            get => _selectionMode;
            set
            {
                if (_selectionMode == value)
                    return;
                _selectionMode = value;
                if (IsEmpty)
                    return;
                Select(AnchorPoint, ActivePoint);
            }
        }

        public NormalizedSnapshotSpanCollection SelectedSpans
        {
            get
            {
                EnsureSelectedSpans();
                return _selectedSpans;
            }
        }

        public VirtualSnapshotPoint Start
        {
            get
            {
                if (!IsReversed)
                    return AnchorPoint;
                return ActivePoint;
            }
        }

        public VirtualSnapshotSpan StreamSelectionSpan => new VirtualSnapshotSpan(Start, End);

        public ITextView TextView { get; }

        public ReadOnlyCollection<VirtualSnapshotSpan> VirtualSelectedSpans
        {
            get
            {
                EnsureVirtualSelectedSpans();
                return new ReadOnlyCollection<VirtualSnapshotSpan>(_virtualSelectedSpans);
            }
        }

        internal ISelectionPainter Painter
        {
            get
            {
                if (TextView.IsClosed)
                    throw new InvalidOperationException();
                if (!_isActive)
                    return UnfocusedPainter;
                return FocusedPainter;
            }
        }

        public TextSelection(ITextView wpfTextView, IEditorFormatMap editorFormatMap,
            GuardedOperations guardedOperations)
        {
            TextView = wpfTextView;
            _editorFormatMap = editorFormatMap;
            ActivationTracksFocus = true;
            _activePoint = _anchorPoint = new VirtualSnapshotPoint(TextView.TextSnapshot, 0);
            _selectionMode = TextSelectionMode.Stream;
            SelectionAdornmentLayer = TextView.GetAdornmentLayer("SelectionAndProvisionHighlight");
            _guardedOperations = guardedOperations;
            CreateAndSetPainter("Selected Text", ref FocusedPainter, SystemColors.HighlightColor);
            CreateAndSetPainter("Inactive Selected Text", ref UnfocusedPainter, SystemColors.GrayTextColor);
            Painter.Activate();
            SubscribeToEvents();
        }

        public void Clear()
        {
            Clear(true);
        }

        public VirtualSnapshotSpan? GetSelectionOnTextViewLine(ITextViewLine line)
        {
            if (line == null)
                throw new ArgumentNullException(nameof(line));
            if (line.Snapshot != TextView.TextSnapshot)
                throw new ArgumentException("The supplied ITextViewLine is on an incorrect snapshot.", nameof(line));
            if (IsEmpty)
            {
                var activePoint = ActivePoint;
                if (line.ContainsBufferPosition(activePoint.Position))
                    return new VirtualSnapshotSpan(activePoint, activePoint);
            }
            else
            {
                var start = Start;
                var end = End;
                if (end.Position.Position >= line.Start && start.Position.Position <= line.End)
                {
                    if (Mode == TextSelectionMode.Box)
                    {
                        var positionFromXcoordinate1 = line.GetInsertionBufferPositionFromXCoordinate(_leftX);
                        var positionFromXcoordinate2 = line.GetInsertionBufferPositionFromXCoordinate(_rightX);
                        if (positionFromXcoordinate1 <= positionFromXcoordinate2)
                            return new VirtualSnapshotSpan(positionFromXcoordinate1, positionFromXcoordinate2);
                        return new VirtualSnapshotSpan(positionFromXcoordinate2, positionFromXcoordinate1);
                    }

                    if (start.Position.Position < line.Start)
                        start = new VirtualSnapshotPoint(line.Start);
                    if (end.Position.Position > line.End)
                        end = new VirtualSnapshotPoint(line.EndIncludingLineBreak);
                    if (start != end)
                        return new VirtualSnapshotSpan(start, end);
                }
            }

            return new VirtualSnapshotSpan?();
        }

        public void Select(SnapshotSpan selectionSpan, bool isReversed)
        {
            var virtualSnapshotPoint1 = new VirtualSnapshotPoint(selectionSpan.Start);
            var virtualSnapshotPoint2 = new VirtualSnapshotPoint(selectionSpan.End);
            if (isReversed)
                Select(virtualSnapshotPoint2, virtualSnapshotPoint1);
            else
                Select(virtualSnapshotPoint1, virtualSnapshotPoint2);
        }

        public void Select(VirtualSnapshotPoint anchorPoint, VirtualSnapshotPoint activePoint)
        {
            if (anchorPoint.Position.Snapshot != TextView.TextSnapshot)
                throw new ArgumentException();
            if (activePoint.Position.Snapshot != TextView.TextSnapshot)
                throw new ArgumentException();
            if (anchorPoint == activePoint)
            {
                Clear(false);
            }
            else
            {
                var anchorPoint1 = NormalizePoint(anchorPoint);
                var activePoint1 = NormalizePoint(activePoint);
                if (anchorPoint1 == activePoint1)
                    Clear(false);
                else
                    InnerSelect(anchorPoint1, activePoint1);
            }
        }

        internal void CreateAndSetPainter(string category, ref ISelectionPainter painter, Color defaultColor)
        {
            painter?.Dispose();
            painter = BrushSelectionPainter.CreatePainter(this, SelectionAdornmentLayer,
                _editorFormatMap.GetProperties(category), defaultColor);
        }

        internal void LayoutChanged(bool visualSnapshotChange, ITextSnapshot newEditSnapshot)
        {
            if (IsEmpty)
            {
                _activePoint = new VirtualSnapshotPoint(newEditSnapshot, 0);
                _anchorPoint = _activePoint;
                if (visualSnapshotChange)
                {
                    _virtualSelectedSpans = null;
                    _selectedSpans = null;
                }
            }
            else if (visualSnapshotChange)
            {
                var point1 = _activePoint.TranslateTo(newEditSnapshot);
                var point2 = _anchorPoint.TranslateTo(newEditSnapshot);
                var anchorPoint = NormalizePoint(point2);
                var activePoint = NormalizePoint(point1);
                if (activePoint == anchorPoint)
                {
                    Clear(false);
                    return;
                }

                if (activePoint != point1 || anchorPoint != point2 || Mode == TextSelectionMode.Box)
                {
                    InnerSelect(anchorPoint, activePoint);
                    return;
                }

                _anchorPoint = anchorPoint;
                _activePoint = activePoint;
                _virtualSelectedSpans = null;
                _selectedSpans = null;
            }

            Painter.Update(false);
        }

        internal void RaiseChangedEvent(bool emptyBefore, bool emptyAfter, bool moved)
        {
            if (moved || emptyBefore != emptyAfter)
            {
                _virtualSelectedSpans = null;
                _selectedSpans = null;
            }

            if (!moved && emptyBefore & emptyAfter)
                return;
            // ISSUE: reference to a compiler-generated field
            _guardedOperations.RaiseEvent(this, SelectionChanged);
        }

        private void Clear(bool resetMode)
        {
            var isEmpty = IsEmpty;
            _anchorPoint = _activePoint;
            ActivationTracksFocus = true;
            if (resetMode)
                Mode = TextSelectionMode.Stream;
            Painter.Clear();
            RaiseChangedEvent(isEmpty, true, false);
        }

        private void EnsureSelectedSpans()
        {
            if (!(_selectedSpans == null))
                return;
            EnsureVirtualSelectedSpans();
            if (_virtualSelectedSpans.Count == 1)
            {
                _selectedSpans = new NormalizedSnapshotSpanCollection(_virtualSelectedSpans[0].SnapshotSpan);
            }
            else
            {
                IList<SnapshotSpan> snapshotSpans = new List<SnapshotSpan>(_virtualSelectedSpans.Count);
                foreach (var virtualSelectedSpan in _virtualSelectedSpans)
                    snapshotSpans.Add(virtualSelectedSpan.SnapshotSpan);
                _selectedSpans = new NormalizedSnapshotSpanCollection(snapshotSpans);
            }
        }

        private void EnsureVirtualSelectedSpans()
        {
            if (_virtualSelectedSpans != null)
                return;
            _virtualSelectedSpans = new List<VirtualSnapshotSpan>();
            if (IsEmpty)
            {
                var activePoint = ActivePoint;
                _virtualSelectedSpans.Add(new VirtualSnapshotSpan(activePoint, activePoint));
            }
            else if (Mode == TextSelectionMode.Box)
            {
                var bufferPosition = Start.Position;
                var end = End;
                int position1;
                int position2;
                do
                {
                    SnapshotPoint position3;
                    int position4;
                    int position5;
                    do
                    {
                        var containingBufferPosition = TextView.GetTextViewLineContainingBufferPosition(bufferPosition);
                        var selectionOnTextViewLine = GetSelectionOnTextViewLine(containingBufferPosition);
                        if (selectionOnTextViewLine.HasValue)
                            _virtualSelectedSpans.Add(selectionOnTextViewLine.Value);
                        if (containingBufferPosition.LineBreakLength != 0 ||
                            !containingBufferPosition.IsLastTextViewLineForSnapshotLine)
                        {
                            bufferPosition = containingBufferPosition.EndIncludingLineBreak;
                            position4 = bufferPosition.Position;
                            position3 = end.Position;
                            position5 = position3.Position;
                        }
                        else
                        {
                            goto label_14;
                        }
                    } while (position4 <= position5);

                    if (end.IsInVirtualSpace)
                    {
                        position1 = bufferPosition.Position;
                        position3 = end.Position;
                        position2 = position3.Position;
                    }
                    else
                    {
                        goto label_15;
                    }
                } while (position1 == position2);

                goto label_13;
                label_14:
                return;
                label_15:
                return;
                label_13:;
            }
            else
            {
                _virtualSelectedSpans.Add(new VirtualSnapshotSpan(Start, End));
            }
        }

        private void InnerSelect(VirtualSnapshotPoint anchorPoint, VirtualSnapshotPoint activePoint)
        {
            var isEmpty = IsEmpty;
            ActivationTracksFocus = true;
            _anchorPoint = anchorPoint;
            _activePoint = activePoint;
            var bufferPosition1 = _anchorPoint;
            var bufferPosition2 = _activePoint;
            if (_anchorPoint > _activePoint)
            {
                bufferPosition1 = _activePoint;
                bufferPosition2 = _anchorPoint;
            }

            if (Mode == TextSelectionMode.Box)
            {
                var containingBufferPosition1 =
                    TextView.GetTextViewLineContainingBufferPosition(bufferPosition1.Position);
                var containingBufferPosition2 =
                    TextView.GetTextViewLineContainingBufferPosition(bufferPosition2.Position);
                var extendedCharacterBounds = containingBufferPosition1.GetExtendedCharacterBounds(bufferPosition1);
                _leftX = extendedCharacterBounds.Leading;
                extendedCharacterBounds = containingBufferPosition2.GetExtendedCharacterBounds(bufferPosition2);
                _rightX = extendedCharacterBounds.Leading;
                if (_rightX < _leftX)
                {
                    var leftX = _leftX;
                    _leftX = _rightX;
                    _rightX = leftX;
                }
            }

            Painter.Update(true);
            RaiseChangedEvent(isEmpty, IsEmpty, true);
        }

        private VirtualSnapshotPoint NormalizePoint(VirtualSnapshotPoint point)
        {
            var containingBufferPosition = TextView.GetTextViewLineContainingBufferPosition(point.Position);
            if (point.Position >= containingBufferPosition.End)
                return new VirtualSnapshotPoint(containingBufferPosition.End, point.VirtualSpaces);
            return new VirtualSnapshotPoint(containingBufferPosition.GetTextElementSpan(point.Position).Start);
        }

        private void OnEditorOptionChanged(object sender, EditorOptionChangedEventArgs e)
        {
            if (e.OptionId == DefaultTextViewOptions.UseVirtualSpaceId.Name)
            {
                if (TextView.Options.IsVirtualSpaceEnabled() || Mode == TextSelectionMode.Box)
                    return;
                Select(new VirtualSnapshotPoint(AnchorPoint.Position),
                    new VirtualSnapshotPoint(TextView.Caret.Position.BufferPosition));
            }
            else
            {
                if (e.OptionId != DefaultViewOptions.EnableSimpleGraphicsId.Name)
                    return;
                CreateAndSetPainter("Selected Text", ref FocusedPainter, SystemColors.HighlightColor);
                CreateAndSetPainter("Inactive Selected Text", ref UnfocusedPainter, SystemColors.GrayTextColor);
                Painter.Activate();
            }
        }

        private void OnFormatMappingChanged(object sender, FormatItemsEventArgs e)
        {
            if (e.ChangedItems.Contains("Selected Text"))
            {
                CreateAndSetPainter("Selected Text", ref FocusedPainter, SystemColors.HighlightColor);
                if (_isActive)
                    FocusedPainter.Activate();
            }

            if (!e.ChangedItems.Contains("Inactive Selected Text"))
                return;
            CreateAndSetPainter("Inactive Selected Text", ref UnfocusedPainter, SystemColors.GrayTextColor);
            if (_isActive)
                return;
            UnfocusedPainter.Activate();
        }

        private void OnViewClosed(object sender, EventArgs e)
        {
            UnsubscribeFromEvents();
            if (FocusedPainter != null)
            {
                FocusedPainter.Dispose();
                FocusedPainter = null;
            }

            if (UnfocusedPainter == null)
                return;
            UnfocusedPainter.Dispose();
            UnfocusedPainter = null;
        }

        private void SubscribeToEvents()
        {
            TextView.Options.OptionChanged += OnEditorOptionChanged;
            _editorFormatMap.FormatMappingChanged += OnFormatMappingChanged;
            TextView.Closed += OnViewClosed;
        }

        private void UnsubscribeFromEvents()
        {
            TextView.Options.OptionChanged -= OnEditorOptionChanged;
            _editorFormatMap.FormatMappingChanged -= OnFormatMappingChanged;
            TextView.Closed -= OnViewClosed;
        }
    }
}