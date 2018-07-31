using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ModernApplicationFramework.TextEditor
{
    public abstract class TextRange : IEnumerable<TextPoint>
    {
        public abstract TextPoint GetStartPoint();

        public abstract TextPoint GetEndPoint();

        public abstract PrimitiveTextBuffer TextBuffer { get; }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public abstract SnapshotSpan AdvancedTextRange { get; }

        public abstract bool MakeUppercase();

        public abstract bool MakeLowercase();

        public abstract bool Capitalize();

        public abstract bool ToggleCase();

        public abstract bool Delete();

        public abstract bool Indent();

        public abstract bool Unindent();

        public abstract bool IsEmpty { get; }

        public abstract TextRange Find(string pattern);

        public abstract TextRange Find(string pattern, FindOptions findOptions);

        public abstract Collection<TextRange> FindAll(string pattern);

        public abstract Collection<TextRange> FindAll(string pattern, FindOptions findOptions);

        public abstract bool ReplaceText(string newText);

        public abstract string GetText();

        public TextRange Clone()
        {
            return CloneInternal();
        }

        protected abstract TextRange CloneInternal();

        public abstract void SetStart(TextPoint startPoint);

        public abstract void SetEnd(TextPoint endPoint);

        public abstract void MoveTo(TextRange newRange);

        protected abstract IEnumerator<TextPoint> GetEnumeratorInternal();

        public IEnumerator<TextPoint> GetEnumerator()
        {
            return GetEnumeratorInternal();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}