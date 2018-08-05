using System;
using System.Windows;
using System.Windows.Media;

namespace ModernApplicationFramework.TextEditor
{
    public interface ITextView : IPropertyOwner
    {
        event EventHandler<BackgroundBrushChangedEventArgs> BackgroundBrushChanged;

        event EventHandler ViewportLeftChanged;

        event EventHandler ViewportHeightChanged;

        event EventHandler ViewportWidthChanged;

        event EventHandler Closed;

        event EventHandler LostAggregateFocus;

        event EventHandler GotAggregateFocus;

        event EventHandler<MouseHoverEventArgs> MouseHover;

        event EventHandler MaxTextRightCoordinateChanged;

        FrameworkElement ManipulationLayer { get; }

        double MaxTextRightCoordinate { get; }

        SnapshotSpan GetTextElementSpan(SnapshotPoint point);

        FrameworkElement VisualElement { get; }

        ITextSelection Selection { get; }

        IFormattedLineSource FormattedLineSource { get; }

        ITextViewLineCollection TextViewLines { get; }

        ITextSnapshot VisualSnapshot { get; }

        ITrackingSpan ProvisionalTextHighlight { get; set; }

        double LineHeight { get; }

        bool InOuterLayout { get; }

        Brush Background { get; set; }

        bool IsClosed { get; }

        double ViewportLeft { get; set; }

        double ViewportTop { get; }

        double ViewportRight { get; }

        double ViewportBottom { get; }

        double ViewportWidth { get; }

        double ViewportHeight { get; }

        bool InLayout { get; }

        ITextViewRoleSet Roles { get; }

        ITextBuffer TextBuffer { get; }

        double ZoomLevel { get; set; }

        event EventHandler<ZoomLevelChangedEventArgs> ZoomLevelChanged;

        event EventHandler<TextViewLayoutChangedEventArgs> LayoutChanged;

        void Close();

        ITextSnapshot TextSnapshot { get; }

        ITextViewModel TextViewModel { get; }

        ITextDataModel TextDataModel { get; }

        IBufferGraph BufferGraph { get; }

        IEditorOptions Options { get; }

        IViewScroller ViewScroller { get; }

        ITextViewLine GetTextViewLineContainingBufferPosition(SnapshotPoint bufferPosition);

        IAdornmentLayer GetAdornmentLayer(string name);

        void DisplayTextLineContainingBufferPosition(SnapshotPoint bufferPosition, double verticalDistance, ViewRelativePosition relativeTo);

        void DisplayTextLineContainingBufferPosition(SnapshotPoint bufferPosition, double verticalDistance, ViewRelativePosition relativeTo, double? viewportWidthOverride, double? viewportHeightOverride);

        bool HasAggregateFocus { get; }

        ITextCaret Caret { get; }
    }
}