using System;
using System.Collections.Generic;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Text.Ui.Text
{
    public interface IMultiSelectionBroker
    {
        ITextView TextView { get; }

        ITextSnapshot CurrentSnapshot { get; }

        IReadOnlyList<Selection> AllSelections { get; }

        bool HasMultipleSelections { get; }

        IReadOnlyList<Selection> GetSelectionsIntersectingSpan(SnapshotSpan span);

        IReadOnlyList<Selection> GetSelectionsIntersectingSpans(NormalizedSnapshotSpanCollection spanCollection);

        void AddSelection(Selection selection);

        void AddSelectionRange(IEnumerable<Selection> range);

        void SetSelection(Selection selection);

        void SetSelectionRange(IEnumerable<Selection> range, Selection primary);

        bool TryRemoveSelection(Selection selection);

        Selection PrimarySelection { get; }

        bool TrySetAsPrimarySelection(Selection candidate);

        void ClearSecondarySelections();

        void PerformActionOnAllSelections(PredefinedSelectionTransformations action);

        void PerformActionOnAllSelections(Action<ISelectionTransformer> action);

        bool TryPerformActionOnSelection(Selection before, PredefinedSelectionTransformations action, out Selection after);

        bool TryPerformActionOnSelection(Selection before, Action<ISelectionTransformer> action, out Selection after);

        bool TryEnsureVisible(Selection selection, EnsureSpanVisibleOptions options);

        void SetBoxSelection(Selection selection);

        Selection BoxSelection { get; }

        bool IsBoxSelection { get; }

        void BreakBoxSelection();

        NormalizedSnapshotSpanCollection SelectedSpans { get; }

        IReadOnlyList<VirtualSnapshotSpan> VirtualSelectedSpans { get; }

        VirtualSnapshotSpan SelectionExtent { get; }

        bool AreSelectionsActive { get; set; }

        bool ActivationTracksFocus { get; set; }

        event EventHandler MultiSelectionSessionChanged;

        IDisposable BeginBatchOperation();

        bool TryGetSelectionPresentationProperties(Selection selection, out AbstractSelectionPresentationProperties properties);

        Selection TransformSelection(Selection source, PredefinedSelectionTransformations transformation);
    }
}
