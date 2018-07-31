using System.Collections;
using System.Collections.Generic;

namespace ModernApplicationFramework.TextEditor
{
    public abstract class DisplayTextRange : TextRange, IEnumerable<DisplayTextPoint>
    {
        public abstract PrimitiveTextView TextView { get; }

        public new DisplayTextRange Clone()
        {
            return CloneDisplayTextRangeInternal();
        }

        public abstract DisplayTextPoint GetDisplayStartPoint();

        public abstract DisplayTextPoint GetDisplayEndPoint();

        public abstract VisibilityState Visibility { get; }

        protected override TextRange CloneInternal()
        {
            return CloneDisplayTextRangeInternal();
        }

        protected abstract DisplayTextRange CloneDisplayTextRangeInternal();

        protected abstract IEnumerator<DisplayTextPoint> GetDisplayPointEnumeratorInternal();

        public new IEnumerator<DisplayTextPoint> GetEnumerator()
        {
            return GetDisplayPointEnumeratorInternal();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}