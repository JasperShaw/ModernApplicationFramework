using System;
using System.Windows;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Differencing;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Text.Ui.Differencing
{
    public interface IDifferenceViewer : IPropertyOwner
    {
        void Initialize(IDifferenceBuffer differenceBuffer, CreateTextViewHostCallback createTextViewHost, IEditorOptions parentOptions = null);

        bool IsInitialized { get; }

        ITextView InlineView { get; }

        ITextView LeftView { get; }

        ITextView RightView { get; }

        ITextViewHost InlineHost { get; }

        ITextViewHost LeftHost { get; }

        ITextViewHost RightHost { get; }

        FrameworkElement VisualElement { get; }

        IDifferenceBuffer DifferenceBuffer { get; }

        DifferenceViewMode ViewMode { get; set; }

        event EventHandler<EventArgs> ViewModeChanged;

        DifferenceViewType ActiveViewType { get; }

        IEditorOptions Options { get; }

        bool AreViewsSynchronized { get; }

        void Close();

        bool IsClosed { get; }

        event EventHandler<EventArgs> Closed;

        bool ScrollToNextChange(bool wrap = false);

        bool ScrollToNextChange(SnapshotPoint point, bool wrap = false);

        bool ScrollToPreviousChange(bool wrap = false);

        bool ScrollToPreviousChange(SnapshotPoint point, bool wrap = false);

        void ScrollToChange(Difference difference);

        void ScrollToMatch(Match match);
    }
}