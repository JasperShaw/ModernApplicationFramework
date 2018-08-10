using System.ComponentModel;
using ModernApplicationFramework.Text.Ui.Formatting;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public abstract class DisplayTextPoint : TextPoint
    {
        public abstract PrimitiveTextView TextView { get; }

        public abstract ITextViewLine AdvancedTextViewLine { get; }

        public abstract int StartOfViewLine { get; }

        public abstract int EndOfViewLine { get; }

        public abstract void MoveToEndOfViewLine();

        public abstract void MoveToStartOfViewLine();

        public abstract void MoveToBeginningOfNextViewLine();

        public abstract void MoveToBeginningOfPreviousViewLine();

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public abstract DisplayTextPoint GetFirstNonWhiteSpaceCharacterOnViewLine();

        public abstract int DisplayColumn { get; }

        public abstract bool IsVisible { get; }

        public new DisplayTextPoint Clone()
        {
            return CloneDisplayTextPointInternal();
        }

        public abstract DisplayTextRange GetDisplayTextRange(DisplayTextPoint otherPoint);

        public abstract DisplayTextRange GetDisplayTextRange(int otherPosition);

        protected sealed override TextPoint CloneInternal()
        {
            return CloneDisplayTextPointInternal();
        }

        protected abstract DisplayTextPoint CloneDisplayTextPointInternal();
    }
}