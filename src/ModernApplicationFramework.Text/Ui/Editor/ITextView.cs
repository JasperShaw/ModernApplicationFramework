using System;
using System.Windows;
using System.Windows.Media;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Projection;
using ModernApplicationFramework.Text.Logic;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Ui.Formatting;
using ModernApplicationFramework.Text.Ui.Text;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public interface ITextView : IPropertyOwner
    {
        event EventHandler<BackgroundBrushChangedEventArgs> BackgroundBrushChanged;

        event EventHandler Closed;

        event EventHandler GotAggregateFocus;

        event EventHandler<TextViewLayoutChangedEventArgs> LayoutChanged;

        event EventHandler LostAggregateFocus;

        event EventHandler MaxTextRightCoordinateChanged;

        event EventHandler<MouseHoverEventArgs> MouseHover;

        event EventHandler ViewportHeightChanged;

        event EventHandler ViewportLeftChanged;

        event EventHandler ViewportWidthChanged;

        event EventHandler<ZoomLevelChangedEventArgs> ZoomLevelChanged;

        IMultiSelectionBroker MultiSelectionBroker { get; }

        Brush Background { get; set; }

        IBufferGraph BufferGraph { get; }

        ITextCaret Caret { get; }

        IFormattedLineSource FormattedLineSource { get; }

        bool HasAggregateFocus { get; }

        bool InLayout { get; }

        bool InOuterLayout { get; }

        bool IsClosed { get; }

        double LineHeight { get; }

        FrameworkElement ManipulationLayer { get; }

        double MaxTextRightCoordinate { get; }

        IEditorOptions Options { get; }

        ITrackingSpan ProvisionalTextHighlight { get; set; }

        ITextViewRoleSet Roles { get; }

        ITextSelection Selection { get; }

        ITextBuffer TextBuffer { get; }

        ITextDataModel TextDataModel { get; }

        ITextSnapshot TextSnapshot { get; }

        ITextViewLineCollection TextViewLines { get; }

        ITextViewModel TextViewModel { get; }

        double ViewportBottom { get; }

        double ViewportHeight { get; }

        double ViewportLeft { get; set; }

        double ViewportRight { get; }

        double ViewportTop { get; }

        double ViewportWidth { get; }

        IViewScroller ViewScroller { get; }

        FrameworkElement VisualElement { get; }

        ITextSnapshot VisualSnapshot { get; }

        double ZoomLevel { get; set; }

        void Close();

        void DisplayTextLineContainingBufferPosition(SnapshotPoint bufferPosition, double verticalDistance,
            ViewRelativePosition relativeTo);

        void DisplayTextLineContainingBufferPosition(SnapshotPoint bufferPosition, double verticalDistance,
            ViewRelativePosition relativeTo, double? viewportWidthOverride, double? viewportHeightOverride);

        IAdornmentLayer GetAdornmentLayer(string name);

        SnapshotSpan GetTextElementSpan(SnapshotPoint point);

        ITextViewLine GetTextViewLineContainingBufferPosition(SnapshotPoint bufferPosition);
    }
}