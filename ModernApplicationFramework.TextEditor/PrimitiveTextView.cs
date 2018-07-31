using System.ComponentModel;

namespace ModernApplicationFramework.TextEditor
{
    public abstract class PrimitiveTextView
    {
        public abstract void MoveLineToTop(int lineNumber);

        public abstract void MoveLineToBottom(int lineNumber);

        public abstract void ScrollUp(int lines);

        public abstract void ScrollDown(int lines);

        public abstract void ScrollPageDown();

        public abstract void ScrollPageUp();

        public abstract bool Show(DisplayTextPoint point, HowToShow howToShow);

        public abstract VisibilityState Show(DisplayTextRange textRange, HowToShow howToShow);

        public abstract DisplayTextPoint GetTextPoint(int position);

        public abstract DisplayTextPoint GetTextPoint(TextPoint textPoint);

        public abstract DisplayTextPoint GetTextPoint(int line, int column);

        public abstract DisplayTextRange GetTextRange(TextPoint startPoint, TextPoint endPoint);

        public abstract DisplayTextRange GetTextRange(TextRange textRange);

        public abstract DisplayTextRange GetTextRange(int startPosition, int endPosition);

        public abstract DisplayTextRange VisibleSpan { get; }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public abstract ITextView AdvancedTextView { get; }

        public abstract Caret Caret { get; }

        public abstract Selection Selection { get; }

        public abstract PrimitiveTextBuffer TextBuffer { get; }
    }
}