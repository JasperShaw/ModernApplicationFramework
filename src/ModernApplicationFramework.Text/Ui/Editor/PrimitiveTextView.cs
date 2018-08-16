using System.ComponentModel;
using ModernApplicationFramework.Text.Ui.Formatting;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public abstract class PrimitiveTextView
    {
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public abstract ITextView AdvancedTextView { get; }

        public abstract Caret Caret { get; }

        public abstract LegacySelection Selection { get; }

        public abstract PrimitiveTextBuffer TextBuffer { get; }

        public abstract DisplayTextRange VisibleSpan { get; }

        public abstract DisplayTextPoint GetTextPoint(int position);

        public abstract DisplayTextPoint GetTextPoint(TextPoint textPoint);

        public abstract DisplayTextPoint GetTextPoint(int line, int column);

        public abstract DisplayTextRange GetTextRange(TextPoint startPoint, TextPoint endPoint);

        public abstract DisplayTextRange GetTextRange(TextRange textRange);

        public abstract DisplayTextRange GetTextRange(int startPosition, int endPosition);

        public abstract void MoveLineToBottom(int lineNumber);
        public abstract void MoveLineToTop(int lineNumber);

        public abstract void ScrollDown(int lines);

        public abstract void ScrollPageDown();

        public abstract void ScrollPageUp();

        public abstract void ScrollUp(int lines);

        public abstract bool Show(DisplayTextPoint point, HowToShow howToShow);

        public abstract VisibilityState Show(DisplayTextRange textRange, HowToShow howToShow);
    }
}