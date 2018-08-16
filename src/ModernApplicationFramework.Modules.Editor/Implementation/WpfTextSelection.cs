using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Formatting;
using ModernApplicationFramework.Text.Ui.Text;
using Selection = ModernApplicationFramework.Text.Ui.Text.Selection;

namespace ModernApplicationFramework.Modules.Editor.Implementation
{
    internal class WpfTextSelection : ITextSelection
    {
        private readonly GuardedOperations _guardedOperations;
        private readonly IMultiSelectionBroker _multiSelectionBroker;
        private IAdornmentLayer _selectionAdornmentLayer;
        public event EventHandler SelectionChanged;
        public bool ActivationTracksFocus
        {
            set => _multiSelectionBroker.ActivationTracksFocus = value;
            get => _multiSelectionBroker.ActivationTracksFocus;
        }
        public VirtualSnapshotPoint ActivePoint
        {
            get
            {
                if (!_multiSelectionBroker.IsBoxSelection)
                    return _multiSelectionBroker.PrimarySelection.ActivePoint;
                if (!_multiSelectionBroker.BoxSelection.IsReversed)
                    return _multiSelectionBroker.SelectionExtent.End;
                return _multiSelectionBroker.SelectionExtent.Start;
            }
        }

        public VirtualSnapshotPoint AnchorPoint
        {
            get
            {
                if (!_multiSelectionBroker.IsBoxSelection)
                    return _multiSelectionBroker.PrimarySelection.AnchorPoint;
                if (!_multiSelectionBroker.BoxSelection.IsReversed)
                    return _multiSelectionBroker.SelectionExtent.Start;
                return _multiSelectionBroker.SelectionExtent.End;
            }
        }
        public VirtualSnapshotPoint End
        {
            get
            {
                if (_multiSelectionBroker.IsBoxSelection)
                    return _multiSelectionBroker.SelectionExtent.End;
                return _multiSelectionBroker.PrimarySelection.End;
            }
        }
        public bool IsActive
        {
            get => _multiSelectionBroker.AreSelectionsActive;
            set => _multiSelectionBroker.AreSelectionsActive = value;
        }

        public bool IsReversed => _multiSelectionBroker.IsBoxSelection
            ? _multiSelectionBroker.BoxSelection.IsReversed
            : _multiSelectionBroker.PrimarySelection.IsReversed;

        public bool IsEmpty
        {
            get
            {
                if (_multiSelectionBroker.PrimarySelection.IsEmpty)
                    return _multiSelectionBroker.AllSelections.Count <= 1;
                return false;
            }
        }
        public TextSelectionMode Mode
        {
            get => !_multiSelectionBroker.IsBoxSelection ? TextSelectionMode.Stream : TextSelectionMode.Box;
            set
            {
                if (Mode == value)
                    return;
                if (value != TextSelectionMode.Stream)
                {
                    if (value != TextSelectionMode.Box)
                        return;
                    _multiSelectionBroker.SetBoxSelection(_multiSelectionBroker.PrimarySelection);
                }
                else
                {
                    var boxSelection = _multiSelectionBroker.BoxSelection;
                    _multiSelectionBroker.ClearSecondarySelections();
                    _multiSelectionBroker.PerformActionOnAllSelections(transformer => transformer.MoveTo(boxSelection.AnchorPoint, boxSelection.ActivePoint, boxSelection.ActivePoint, PositionAffinity.Successor));
                }
            }
        }

        public NormalizedSnapshotSpanCollection SelectedSpans => _multiSelectionBroker.SelectedSpans;
        public VirtualSnapshotPoint Start
        {
            get
            {
                if (_multiSelectionBroker.IsBoxSelection)
                    return _multiSelectionBroker.SelectionExtent.Start;
                return _multiSelectionBroker.PrimarySelection.Start;
            }
        }

        public VirtualSnapshotSpan StreamSelectionSpan => new VirtualSnapshotSpan(Start, End);
        public ITextView TextView { get; }

        public ReadOnlyCollection<VirtualSnapshotSpan> VirtualSelectedSpans => new ReadOnlyCollection<VirtualSnapshotSpan>(_multiSelectionBroker.VirtualSelectedSpans.ToList());

        public WpfTextSelection(ITextView textView, GuardedOperations guardedOperations, IMultiSelectionBroker multiSelectionBroker)
        {
            TextView = textView;
            _guardedOperations = guardedOperations;
            _multiSelectionBroker = multiSelectionBroker;
            ActivationTracksFocus = true;
            _selectionAdornmentLayer = TextView.GetAdornmentLayer("SelectionAndProvisionHighlight");
            CreateAndSetPainter("Selected Text", SystemColors.HighlightColor);
            CreateAndSetPainter("Inactive Selected Text", SystemColors.GrayTextColor);
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
            var nullable = _multiSelectionBroker.GetSelectionsIntersectingSpan(line.Extent).FirstOrNullable();
            ref var local = ref nullable;
            return local?.Extent;
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

        internal void CreateAndSetPainter(string category, Color defaultColor)
        {
            if (category != null)
                return;
            category = string.Empty;
            defaultColor = Colors.Red;
        }

        private void InnerSelect(VirtualSnapshotPoint anchorPoint, VirtualSnapshotPoint activePoint)
        {
            var num = IsEmpty ? 1 : 0;
            ActivationTracksFocus = true;
            var after = _multiSelectionBroker.IsBoxSelection ? _multiSelectionBroker.BoxSelection : _multiSelectionBroker.PrimarySelection;
            _multiSelectionBroker.TryPerformActionOnSelection(after, transformer =>
            {
                var selectionTransformer = transformer;
                var anchorPoint1 = anchorPoint;
                var activePoint1 = activePoint;
                var selection = transformer.Selection;
                var insertionPoint = selection.InsertionPoint;
                selection = transformer.Selection;
                var insertionPointAffinity = (int)selection.InsertionPointAffinity;
                selectionTransformer.MoveTo(anchorPoint1, activePoint1, insertionPoint, (PositionAffinity)insertionPointAffinity);
            }, out after);
        }

        private void Clear(bool resetMode)
        {
            var num = IsEmpty ? 1 : 0;
            if (!resetMode && _multiSelectionBroker.IsBoxSelection)
                _multiSelectionBroker.SetBoxSelection(new Selection(_multiSelectionBroker.BoxSelection.InsertionPoint));
            else if (_multiSelectionBroker.IsBoxSelection)
                _multiSelectionBroker.SetSelection(new Selection(_multiSelectionBroker.BoxSelection.InsertionPoint));
            else
                _multiSelectionBroker.SetSelection(new Selection(_multiSelectionBroker.PrimarySelection.InsertionPoint));
            _multiSelectionBroker.ActivationTracksFocus = true;
        }

        private VirtualSnapshotPoint NormalizePoint(VirtualSnapshotPoint point)
        {
            var containingBufferPosition = TextView.GetTextViewLineContainingBufferPosition(point.Position);
            if (point.Position >= containingBufferPosition.End)
                return new VirtualSnapshotPoint(containingBufferPosition.End, point.VirtualSpaces);
            return new VirtualSnapshotPoint(containingBufferPosition.GetTextElementSpan(point.Position).Start);
        }

        private void SubscribeToEvents()
        {
            _multiSelectionBroker.MultiSelectionSessionChanged += OnMultiSelectionSessionChanged;
            TextView.Options.OptionChanged += OnEditorOptionChanged;
            TextView.Closed += OnViewClosed;
        }

        private void UnsubscribeFromEvents()
        {
            TextView.Options.OptionChanged -= OnEditorOptionChanged;
            _multiSelectionBroker.MultiSelectionSessionChanged -= OnMultiSelectionSessionChanged;
            TextView.Closed -= OnViewClosed;
        }

        private void OnMultiSelectionSessionChanged(object sender, EventArgs e)
        {
            _guardedOperations.RaiseEvent(this, SelectionChanged);
        }

        private void OnEditorOptionChanged(object sender, EditorOptionChangedEventArgs e)
        {
            if (!string.Equals(e.OptionId, DefaultTextViewOptions.UseVirtualSpaceId.Name, StringComparison.Ordinal) || TextView.Options.IsVirtualSpaceEnabled() || Mode == TextSelectionMode.Box)
                return;
            Select(new VirtualSnapshotPoint(AnchorPoint.Position), new VirtualSnapshotPoint(TextView.Caret.Position.BufferPosition));
        }

        private void OnViewClosed(object sender, EventArgs e)
        {
            UnsubscribeFromEvents();
        }

    }
}