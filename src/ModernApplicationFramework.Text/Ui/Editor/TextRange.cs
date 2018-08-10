using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Operations;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public abstract class TextRange : IEnumerable<TextPoint>
    {
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public abstract SnapshotSpan AdvancedTextRange { get; }

        public abstract bool IsEmpty { get; }

        public abstract PrimitiveTextBuffer TextBuffer { get; }

        public abstract bool Capitalize();

        public TextRange Clone()
        {
            return CloneInternal();
        }

        public abstract bool Delete();

        public abstract TextRange Find(string pattern);

        public abstract TextRange Find(string pattern, FindOptions findOptions);

        public abstract Collection<TextRange> FindAll(string pattern);

        public abstract Collection<TextRange> FindAll(string pattern, FindOptions findOptions);

        public abstract TextPoint GetEndPoint();

        public IEnumerator<TextPoint> GetEnumerator()
        {
            return GetEnumeratorInternal();
        }

        public abstract TextPoint GetStartPoint();

        public abstract string GetText();

        public abstract bool Indent();

        public abstract bool MakeLowercase();

        public abstract bool MakeUppercase();

        public abstract void MoveTo(TextRange newRange);

        public abstract bool ReplaceText(string newText);

        public abstract void SetEnd(TextPoint endPoint);

        public abstract void SetStart(TextPoint startPoint);

        public abstract bool ToggleCase();

        public abstract bool Unindent();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected abstract TextRange CloneInternal();

        protected abstract IEnumerator<TextPoint> GetEnumeratorInternal();
    }
}