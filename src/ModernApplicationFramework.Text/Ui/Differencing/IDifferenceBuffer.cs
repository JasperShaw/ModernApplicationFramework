using System;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Differencing;
using ModernApplicationFramework.Text.Data.Projection;
using ModernApplicationFramework.Text.Logic.Differencing;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Text.Ui.Differencing
{
    public interface IDifferenceBuffer : IDisposable, IPropertyOwner
    {
        ITextBuffer BaseLeftBuffer { get; }

        ITextBuffer LeftBuffer { get; }

        ITextBuffer BaseRightBuffer { get; }

        ITextBuffer RightBuffer { get; }

        IProjectionBuffer InlineBuffer { get; }

        ISnapshotDifference CurrentSnapshotDifference { get; }

        IProjectionSnapshot CurrentInlineBufferSnapshot { get; }

        event EventHandler<SnapshotDifferenceChangeEventArgs> SnapshotDifferenceChanging;

        event EventHandler<SnapshotDifferenceChangeEventArgs> SnapshotDifferenceChanged;

        IEditorOptions Options { get; }

        StringDifferenceOptions DifferenceOptions { get; set; }

        bool IsEditingDisabled { get; }

        void AddIgnoreDifferencePredicate(IgnoreDifferencePredicate predicate);

        bool RemoveIgnoreDifferencePredicate(IgnoreDifferencePredicate predicate);

        void AddSnapshotLineTransform(SnapshotLineTransform transform);

        bool RemoveSnapshotLineTransform(SnapshotLineTransform transform);
    }
}