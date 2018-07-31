using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ModernApplicationFramework.TextEditor
{
    public abstract class TextPoint
    {
        public abstract PrimitiveTextBuffer TextBuffer { get; }

        public abstract int CurrentPosition { get; }

        public abstract int Column { get; }

        public abstract bool DeleteNext();

        public abstract bool DeletePrevious();

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public abstract TextPoint GetFirstNonWhiteSpaceCharacterOnLine();

        public abstract TextRange GetCurrentWord();

        public abstract TextRange GetNextWord();

        public abstract TextRange GetPreviousWord();

        public abstract TextRange GetTextRange(TextPoint otherPoint);

        public abstract TextRange GetTextRange(int otherPosition);

        public abstract bool InsertNewLine();

        public abstract bool InsertIndent();

        public abstract bool InsertText(string text);

        public abstract int LineNumber { get; }

        public abstract int StartOfLine { get; }

        public abstract int EndOfLine { get; }

        public abstract bool RemovePreviousIndent();

        public abstract bool TransposeCharacter();

        public abstract bool TransposeLine();

        public abstract bool TransposeLine(int lineNumber);

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public abstract SnapshotPoint AdvancedTextPoint { get; }

        public abstract string GetNextCharacter();

        public abstract string GetPreviousCharacter();

        public abstract TextRange Find(string pattern, FindOptions findOptions, TextPoint endPoint);

        public abstract TextRange Find(string pattern, TextPoint endPoint);

        public abstract TextRange Find(string pattern, FindOptions findOptions);

        public abstract TextRange Find(string pattern);

        public abstract Collection<TextRange> FindAll(string pattern, TextPoint endPoint);

        public abstract Collection<TextRange> FindAll(string pattern, FindOptions findOptions, TextPoint endPoint);

        public abstract Collection<TextRange> FindAll(string pattern);

        public abstract Collection<TextRange> FindAll(string pattern, FindOptions findOptions);

        public abstract void MoveTo(int position);

        public abstract void MoveToNextCharacter();

        public abstract void MoveToPreviousCharacter();

        public TextPoint Clone()
        {
            return CloneInternal();
        }

        protected abstract TextPoint CloneInternal();

        public abstract void MoveToLine(int lineNumber);

        public abstract void MoveToEndOfLine();

        public abstract void MoveToStartOfLine();

        public abstract void MoveToEndOfDocument();

        public abstract void MoveToStartOfDocument();

        public abstract void MoveToBeginningOfNextLine();

        public abstract void MoveToBeginningOfPreviousLine();

        public abstract void MoveToNextWord();

        public abstract void MoveToPreviousWord();
    }
}