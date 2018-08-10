using System.Collections.Generic;
using System.Collections.ObjectModel;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Operations;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Formatting;

namespace ModernApplicationFramework.TextEditor
{
    internal sealed class DefaultDisplayTextRangePrimitive : DisplayTextRange
    {
        private readonly TextRange _bufferRange;

        internal DefaultDisplayTextRangePrimitive(PrimitiveTextView textView, TextRange bufferRange)
        {
            TextView = textView;
            _bufferRange = bufferRange.Clone();
            MoveTo(bufferRange);
        }

        public override PrimitiveTextView TextView { get; }

        public override DisplayTextPoint GetDisplayStartPoint()
        {
            return TextView.GetTextPoint(_bufferRange.GetStartPoint());
        }

        public override DisplayTextPoint GetDisplayEndPoint()
        {
            return TextView.GetTextPoint(_bufferRange.GetEndPoint());
        }

        public override VisibilityState Visibility
        {
            get
            {
                ITextViewLineCollection textViewLines = TextView.AdvancedTextView.TextViewLines;
                SnapshotSpan snapshotSpan = new SnapshotSpan(GetStartPoint().AdvancedTextPoint, GetEndPoint().AdvancedTextPoint);
                SnapshotSpan? nullable = textViewLines.FormattedSpan.Overlap(snapshotSpan);
                if (nullable.HasValue)
                {
                    ITextViewLine containingBufferPosition1 = textViewLines.GetTextViewLineContainingBufferPosition(nullable.Value.Start);
                    ITextViewLine containingBufferPosition2 = textViewLines.GetTextViewLineContainingBufferPosition(nullable.Value.End);
                    VisibilityState visibilityState1 = containingBufferPosition1.VisibilityState;
                    if (containingBufferPosition1 == containingBufferPosition2)
                        return visibilityState1;
                    VisibilityState visibilityState2 = containingBufferPosition2.VisibilityState;
                    if (visibilityState1 == VisibilityState.FullyVisible && visibilityState2 == VisibilityState.FullyVisible)
                        return VisibilityState.FullyVisible;
                    if (visibilityState1 != VisibilityState.Hidden || visibilityState2 != VisibilityState.Hidden || containingBufferPosition1.EndIncludingLineBreak != containingBufferPosition2.Start)
                        return VisibilityState.PartiallyVisible;
                }
                return VisibilityState.Hidden;
            }
        }

        public override bool IsEmpty => _bufferRange.IsEmpty;

        protected override DisplayTextRange CloneDisplayTextRangeInternal()
        {
            return new DefaultDisplayTextRangePrimitive(TextView, _bufferRange);
        }

        protected override IEnumerator<DisplayTextPoint> GetDisplayPointEnumeratorInternal()
        {
            DisplayTextPoint displayTextPoint = GetDisplayStartPoint();
            DisplayTextPoint endPoint = GetDisplayEndPoint();
            while (displayTextPoint.CurrentPosition <= endPoint.CurrentPosition)
            {
                yield return displayTextPoint;
                if (displayTextPoint.CurrentPosition == displayTextPoint.AdvancedTextPoint.Snapshot.Length)
                    break;
                displayTextPoint = displayTextPoint.Clone();
                displayTextPoint.MoveToNextCharacter();
            }
        }

        public override TextPoint GetStartPoint()
        {
            return _bufferRange.GetStartPoint();
        }

        public override TextPoint GetEndPoint()
        {
            return _bufferRange.GetEndPoint();
        }

        public override PrimitiveTextBuffer TextBuffer => TextView.TextBuffer;

        public override SnapshotSpan AdvancedTextRange => _bufferRange.AdvancedTextRange;

        public override bool MakeUppercase()
        {
            return _bufferRange.MakeUppercase();
        }

        public override bool MakeLowercase()
        {
            return _bufferRange.MakeLowercase();
        }

        public override bool Capitalize()
        {
            return _bufferRange.Capitalize();
        }

        public override bool ToggleCase()
        {
            return _bufferRange.ToggleCase();
        }

        public override bool Delete()
        {
            return _bufferRange.Delete();
        }

        public override bool Indent()
        {
            return _bufferRange.Indent();
        }

        public override bool Unindent()
        {
            return _bufferRange.Unindent();
        }

        public override TextRange Find(string pattern)
        {
            return _bufferRange.Find(pattern);
        }

        public override TextRange Find(string pattern, FindOptions findOptions)
        {
            return _bufferRange.Find(pattern, findOptions);
        }

        public override Collection<TextRange> FindAll(string pattern)
        {
            return _bufferRange.FindAll(pattern);
        }

        public override Collection<TextRange> FindAll(string pattern, FindOptions findOptions)
        {
            return _bufferRange.FindAll(pattern, findOptions);
        }

        public override bool ReplaceText(string newText)
        {
            return _bufferRange.ReplaceText(newText);
        }

        public override string GetText()
        {
            return _bufferRange.GetText();
        }

        public override void SetStart(TextPoint startPoint)
        {
            _bufferRange.SetStart(TextView.GetTextPoint(startPoint));
        }

        public override void SetEnd(TextPoint endPoint)
        {
            _bufferRange.SetEnd(TextView.GetTextPoint(endPoint));
        }

        public override void MoveTo(TextRange newRange)
        {
            SetStart(newRange.GetStartPoint());
            SetEnd(newRange.GetEndPoint());
        }

        protected override IEnumerator<TextPoint> GetEnumeratorInternal()
        {
            return _bufferRange.GetEnumerator();
        }
    }
}