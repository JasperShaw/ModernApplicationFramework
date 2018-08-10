using System.Collections.ObjectModel;
using System.ComponentModel;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Operations;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public abstract class TextPoint
    {
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public abstract SnapshotPoint AdvancedTextPoint { get; }

        public abstract int Column { get; }

        public abstract int CurrentPosition { get; }

        public abstract int EndOfLine { get; }

        public abstract int LineNumber { get; }

        public abstract int StartOfLine { get; }
        public abstract PrimitiveTextBuffer TextBuffer { get; }

        public TextPoint Clone()
        {
            return CloneInternal();
        }

        public abstract bool DeleteNext();

        public abstract bool DeletePrevious();

        public abstract TextRange Find(string pattern, FindOptions findOptions, TextPoint endPoint);

        public abstract TextRange Find(string pattern, TextPoint endPoint);

        public abstract TextRange Find(string pattern, FindOptions findOptions);

        public abstract TextRange Find(string pattern);

        public abstract Collection<TextRange> FindAll(string pattern, TextPoint endPoint);

        public abstract Collection<TextRange> FindAll(string pattern, FindOptions findOptions, TextPoint endPoint);

        public abstract Collection<TextRange> FindAll(string pattern);

        public abstract Collection<TextRange> FindAll(string pattern, FindOptions findOptions);

        public abstract TextRange GetCurrentWord();

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public abstract TextPoint GetFirstNonWhiteSpaceCharacterOnLine();

        public abstract string GetNextCharacter();

        public abstract TextRange GetNextWord();

        public abstract string GetPreviousCharacter();

        public abstract TextRange GetPreviousWord();

        public abstract TextRange GetTextRange(TextPoint otherPoint);

        public abstract TextRange GetTextRange(int otherPosition);

        public abstract bool InsertIndent();

        public abstract bool InsertNewLine();

        public abstract bool InsertText(string text);

        public abstract void MoveTo(int position);

        public abstract void MoveToBeginningOfNextLine();

        public abstract void MoveToBeginningOfPreviousLine();

        public abstract void MoveToEndOfDocument();

        public abstract void MoveToEndOfLine();

        public abstract void MoveToLine(int lineNumber);

        public abstract void MoveToNextCharacter();

        public abstract void MoveToNextWord();

        public abstract void MoveToPreviousCharacter();

        public abstract void MoveToPreviousWord();

        public abstract void MoveToStartOfDocument();

        public abstract void MoveToStartOfLine();

        public abstract bool RemovePreviousIndent();

        public abstract bool TransposeCharacter();

        public abstract bool TransposeLine();

        public abstract bool TransposeLine(int lineNumber);

        protected abstract TextPoint CloneInternal();
    }
}