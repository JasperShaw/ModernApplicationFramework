using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Formatting;

namespace ModernApplicationFramework.Modules.Editor.EditorPrimitives
{
    internal sealed class DefaultTextViewPrimitive : PrimitiveTextView
    {
        private readonly IViewPrimitivesFactoryService _viewPrimitivesFactory;

        internal DefaultTextViewPrimitive(ITextView textView, IViewPrimitivesFactoryService viewPrimitivesFactory, IBufferPrimitivesFactoryService bufferPrimitivesFactory)
        {
            AdvancedTextView = textView;
            _viewPrimitivesFactory = viewPrimitivesFactory;
            TextBuffer = bufferPrimitivesFactory.CreateTextBuffer(textView.TextBuffer);
            Caret = _viewPrimitivesFactory.CreateCaret(this);
            Selection = _viewPrimitivesFactory.CreateSelection(this);
        }

        public override void MoveLineToTop(int lineNumber)
        {
            AdvancedTextView.DisplayTextLineContainingBufferPosition(AdvancedTextView.TextBuffer.CurrentSnapshot.GetLineFromLineNumber(lineNumber).Start, 0.0, ViewRelativePosition.Top);
        }

        public override void MoveLineToBottom(int lineNumber)
        {
            AdvancedTextView.DisplayTextLineContainingBufferPosition(AdvancedTextView.TextBuffer.CurrentSnapshot.GetLineFromLineNumber(lineNumber).Start, 0.0, ViewRelativePosition.Bottom);
        }

        public override void ScrollUp(int lines)
        {
            AdvancedTextView.ViewScroller.ScrollViewportVerticallyByPixels(lines * AdvancedTextView.LineHeight);
        }

        public override void ScrollDown(int lines)
        {
            AdvancedTextView.ViewScroller.ScrollViewportVerticallyByPixels(-(double)lines * AdvancedTextView.LineHeight);
        }

        public override void ScrollPageDown()
        {
            AdvancedTextView.ViewScroller.ScrollViewportVerticallyByPage(ScrollDirection.Down);
        }

        public override void ScrollPageUp()
        {
            AdvancedTextView.ViewScroller.ScrollViewportVerticallyByPage(ScrollDirection.Up);
        }

        public override bool Show(DisplayTextPoint point, HowToShow howToShow)
        {
            switch (howToShow)
            {
                case HowToShow.AsIs:
                    AdvancedTextView.ViewScroller.EnsureSpanVisible(new SnapshotSpan(point.AdvancedTextPoint, 0), EnsureSpanVisibleOptions.MinimumScroll);
                    break;
                case HowToShow.Centered:
                    AdvancedTextView.ViewScroller.EnsureSpanVisible(new SnapshotSpan(point.AdvancedTextPoint, 0), EnsureSpanVisibleOptions.AlwaysCenter);
                    break;
                case HowToShow.OnFirstLineOfView:
                    AdvancedTextView.DisplayTextLineContainingBufferPosition(point.AdvancedTextPoint, 0.0, ViewRelativePosition.Top);
                    break;
            }
            return point.IsVisible;
        }

        public override VisibilityState Show(DisplayTextRange textRange, HowToShow howToShow)
        {
            switch (howToShow)
            {
                case HowToShow.AsIs:
                    AdvancedTextView.ViewScroller.EnsureSpanVisible(textRange.AdvancedTextRange, EnsureSpanVisibleOptions.MinimumScroll);
                    break;
                case HowToShow.Centered:
                    AdvancedTextView.ViewScroller.EnsureSpanVisible(textRange.AdvancedTextRange, EnsureSpanVisibleOptions.AlwaysCenter);
                    break;
                case HowToShow.OnFirstLineOfView:
                    AdvancedTextView.DisplayTextLineContainingBufferPosition(textRange.AdvancedTextRange.Start, 0.0, ViewRelativePosition.Top);
                    break;
            }
            return textRange.Visibility;
        }

        public override DisplayTextPoint GetTextPoint(int position)
        {
            return _viewPrimitivesFactory.CreateDisplayTextPoint(this, position);
        }

        public override DisplayTextPoint GetTextPoint(TextPoint textPoint)
        {
            return GetTextPoint(textPoint.CurrentPosition);
        }

        public override DisplayTextPoint GetTextPoint(int line, int column)
        {
            return GetTextPoint(AdvancedTextView.TextBuffer.CurrentSnapshot.GetLineFromLineNumber(line).Start + column);
        }

        public override DisplayTextRange GetTextRange(TextPoint startPoint, TextPoint endPoint)
        {
            return GetTextRange(TextBuffer.GetTextRange(startPoint, endPoint));
        }

        public override DisplayTextRange GetTextRange(TextRange textRange)
        {
            return _viewPrimitivesFactory.CreateDisplayTextRange(this, textRange);
        }

        public override DisplayTextRange GetTextRange(int startPosition, int endPosition)
        {
            return GetTextRange(TextBuffer.GetTextPoint(startPosition), TextBuffer.GetTextPoint(endPosition));
        }

        public override DisplayTextRange VisibleSpan => GetTextRange(AdvancedTextView.TextViewLines.FirstVisibleLine.Start, AdvancedTextView.TextViewLines.LastVisibleLine.EndIncludingLineBreak);

        public override ITextView AdvancedTextView { get; }

        public override Caret Caret { get; }

        public override Selection Selection { get; }

        public override PrimitiveTextBuffer TextBuffer { get; }
    }
}