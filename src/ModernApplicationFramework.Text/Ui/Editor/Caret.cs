using System.ComponentModel;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public abstract class Caret : DisplayTextPoint
    {
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public abstract ITextCaret AdvancedCaret { get; }

        public abstract void EnsureVisible();

        public abstract void MovePageDown();

        public abstract void MovePageDown(bool extendSelection);

        public abstract void MovePageUp();

        public abstract void MovePageUp(bool extendSelection);

        public abstract void MoveTo(int position, bool extendSelection);

        public abstract void MoveToBeginningOfNextLine(bool extendSelection);

        public abstract void MoveToBeginningOfNextViewLine(bool extendSelection);

        public abstract void MoveToBeginningOfPreviousLine(bool extendSelection);

        public abstract void MoveToBeginningOfPreviousViewLine(bool extendSelection);

        public abstract void MoveToEndOfDocument(bool extendSelection);

        public abstract void MoveToEndOfLine(bool extendSelection);

        public abstract void MoveToEndOfViewLine(bool extendSelection);

        public abstract void MoveToLine(int lineNumber, bool extendSelection);

        public abstract void MoveToLine(int lineNumber, int offset, bool extendSelection);
        public abstract void MoveToNextCharacter(bool extendSelection);

        public abstract void MoveToNextLine(bool extendSelection);

        public abstract void MoveToNextWord(bool extendSelection);

        public abstract void MoveToPreviousCharacter(bool extendSelection);

        public abstract void MoveToPreviousLine(bool extendSelection);

        public abstract void MoveToPreviousWord(bool extendSelection);

        public abstract void MoveToStartOfDocument(bool extendSelection);

        public abstract void MoveToStartOfLine(bool extendSelection);

        public abstract void MoveToStartOfViewLine(bool extendSelection);
    }
}