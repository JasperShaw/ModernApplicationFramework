using System.Collections.Generic;
using System.Collections.ObjectModel;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Operations;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Formatting;

namespace ModernApplicationFramework.Modules.Editor.EditorPrimitives
{
    internal sealed class DefaultDisplayTextRangePrimitive : DisplayTextRange
    {
        private readonly TextRange _bufferRange;

        public override SnapshotSpan AdvancedTextRange => _bufferRange.AdvancedTextRange;

        public override bool IsEmpty => _bufferRange.IsEmpty;

        public override PrimitiveTextBuffer TextBuffer => TextView.TextBuffer;

        public override PrimitiveTextView TextView { get; }

        public override VisibilityState Visibility
        {
            get
            {
                var textViewLines = TextView.AdvancedTextView.TextViewLines;
                var snapshotSpan = new SnapshotSpan(GetStartPoint().AdvancedTextPoint, GetEndPoint().AdvancedTextPoint);
                var nullable = textViewLines.FormattedSpan.Overlap(snapshotSpan);
                if (nullable.HasValue)
                {
                    var containingBufferPosition1 =
                        textViewLines.GetTextViewLineContainingBufferPosition(nullable.Value.Start);
                    var containingBufferPosition2 =
                        textViewLines.GetTextViewLineContainingBufferPosition(nullable.Value.End);
                    var visibilityState1 = containingBufferPosition1.VisibilityState;
                    if (containingBufferPosition1 == containingBufferPosition2)
                        return visibilityState1;
                    var visibilityState2 = containingBufferPosition2.VisibilityState;
                    if (visibilityState1 == VisibilityState.FullyVisible &&
                        visibilityState2 == VisibilityState.FullyVisible)
                        return VisibilityState.FullyVisible;
                    if (visibilityState1 != VisibilityState.Hidden || visibilityState2 != VisibilityState.Hidden ||
                        containingBufferPosition1.EndIncludingLineBreak != containingBufferPosition2.Start)
                        return VisibilityState.PartiallyVisible;
                }

                return VisibilityState.Hidden;
            }
        }

        internal DefaultDisplayTextRangePrimitive(PrimitiveTextView textView, TextRange bufferRange)
        {
            TextView = textView;
            _bufferRange = bufferRange.Clone();
            MoveTo(bufferRange);
        }

        public override bool Capitalize()
        {
            return _bufferRange.Capitalize();
        }

        public override bool Delete()
        {
            return _bufferRange.Delete();
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

        public override DisplayTextPoint GetDisplayEndPoint()
        {
            return TextView.GetTextPoint(_bufferRange.GetEndPoint());
        }

        public override DisplayTextPoint GetDisplayStartPoint()
        {
            return TextView.GetTextPoint(_bufferRange.GetStartPoint());
        }

        public override TextPoint GetEndPoint()
        {
            return _bufferRange.GetEndPoint();
        }

        public override TextPoint GetStartPoint()
        {
            return _bufferRange.GetStartPoint();
        }

        public override string GetText()
        {
            return _bufferRange.GetText();
        }

        public override bool Indent()
        {
            return _bufferRange.Indent();
        }

        public override bool MakeLowercase()
        {
            return _bufferRange.MakeLowercase();
        }

        public override bool MakeUppercase()
        {
            return _bufferRange.MakeUppercase();
        }

        public override void MoveTo(TextRange newRange)
        {
            SetStart(newRange.GetStartPoint());
            SetEnd(newRange.GetEndPoint());
        }

        public override bool ReplaceText(string newText)
        {
            return _bufferRange.ReplaceText(newText);
        }

        public override void SetEnd(TextPoint endPoint)
        {
            _bufferRange.SetEnd(TextView.GetTextPoint(endPoint));
        }

        public override void SetStart(TextPoint startPoint)
        {
            _bufferRange.SetStart(TextView.GetTextPoint(startPoint));
        }

        public override bool ToggleCase()
        {
            return _bufferRange.ToggleCase();
        }

        public override bool Unindent()
        {
            return _bufferRange.Unindent();
        }

        protected override DisplayTextRange CloneDisplayTextRangeInternal()
        {
            return new DefaultDisplayTextRangePrimitive(TextView, _bufferRange);
        }

        protected override IEnumerator<DisplayTextPoint> GetDisplayPointEnumeratorInternal()
        {
            var displayTextPoint = GetDisplayStartPoint();
            var endPoint = GetDisplayEndPoint();
            while (displayTextPoint.CurrentPosition <= endPoint.CurrentPosition)
            {
                yield return displayTextPoint;
                if (displayTextPoint.CurrentPosition == displayTextPoint.AdvancedTextPoint.Snapshot.Length)
                    break;
                displayTextPoint = displayTextPoint.Clone();
                displayTextPoint.MoveToNextCharacter();
            }
        }

        protected override IEnumerator<TextPoint> GetEnumeratorInternal()
        {
            return _bufferRange.GetEnumerator();
        }
    }
}