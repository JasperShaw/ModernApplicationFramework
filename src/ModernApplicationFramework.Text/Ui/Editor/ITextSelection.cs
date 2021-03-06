﻿using System;
using System.Collections.ObjectModel;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic;
using ModernApplicationFramework.Text.Ui.Formatting;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public interface ITextSelection
    {
        ITextView TextView { get; }

        void Select(SnapshotSpan selectionSpan, bool isReversed);

        void Select(VirtualSnapshotPoint anchorPoint, VirtualSnapshotPoint activePoint);

        NormalizedSnapshotSpanCollection SelectedSpans { get; }

        ReadOnlyCollection<VirtualSnapshotSpan> VirtualSelectedSpans { get; }

        VirtualSnapshotSpan? GetSelectionOnTextViewLine(ITextViewLine line);

        VirtualSnapshotSpan StreamSelectionSpan { get; }

        TextSelectionMode Mode { get; set; }

        bool IsReversed { get; }

        void Clear();

        bool IsEmpty { get; }

        bool IsActive { get; set; }

        bool ActivationTracksFocus { get; set; }

        event EventHandler SelectionChanged;

        VirtualSnapshotPoint ActivePoint { get; }

        VirtualSnapshotPoint AnchorPoint { get; }

        VirtualSnapshotPoint Start { get; }

        VirtualSnapshotPoint End { get; }
    }
}