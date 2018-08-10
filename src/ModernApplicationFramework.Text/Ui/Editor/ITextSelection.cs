using System;
using System.Collections.ObjectModel;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic;
using ModernApplicationFramework.Text.Ui.Formatting;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public interface ITextSelection
    {
        event EventHandler SelectionChanged;

        bool ActivationTracksFocus { get; set; }

        VirtualSnapshotPoint ActivePoint { get; }

        VirtualSnapshotPoint AnchorPoint { get; }

        VirtualSnapshotPoint End { get; }

        bool IsActive { get; set; }

        bool IsEmpty { get; }

        bool IsReversed { get; }

        TextSelectionMode Mode { get; set; }

        NormalizedSnapshotSpanCollection SelectedSpans { get; }

        VirtualSnapshotPoint Start { get; }

        VirtualSnapshotSpan StreamSelectionSpan { get; }
        ITextView TextView { get; }

        ReadOnlyCollection<VirtualSnapshotSpan> VirtualSelectedSpans { get; }

        void Clear();

        VirtualSnapshotSpan? GetSelectionOnTextViewLine(ITextViewLine line);

        void Select(SnapshotSpan selectionSpan, bool isReversed);

        void Select(VirtualSnapshotPoint anchorPoint, VirtualSnapshotPoint activePoint);
    }
}