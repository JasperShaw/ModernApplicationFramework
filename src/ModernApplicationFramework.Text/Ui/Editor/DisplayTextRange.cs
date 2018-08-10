using System.Collections;
using System.Collections.Generic;
using ModernApplicationFramework.Text.Ui.Formatting;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public abstract class DisplayTextRange : TextRange, IEnumerable<DisplayTextPoint>
    {
        public abstract PrimitiveTextView TextView { get; }

        public abstract VisibilityState Visibility { get; }

        public new DisplayTextRange Clone()
        {
            return CloneDisplayTextRangeInternal();
        }

        public abstract DisplayTextPoint GetDisplayEndPoint();

        public abstract DisplayTextPoint GetDisplayStartPoint();

        public new IEnumerator<DisplayTextPoint> GetEnumerator()
        {
            return GetDisplayPointEnumeratorInternal();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected abstract DisplayTextRange CloneDisplayTextRangeInternal();

        protected override TextRange CloneInternal()
        {
            return CloneDisplayTextRangeInternal();
        }

        protected abstract IEnumerator<DisplayTextPoint> GetDisplayPointEnumeratorInternal();
    }
}