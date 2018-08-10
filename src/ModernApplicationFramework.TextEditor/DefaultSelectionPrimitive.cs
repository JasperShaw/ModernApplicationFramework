using System.Collections.Generic;
using System.Collections.ObjectModel;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Logic.Operations;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Formatting;

namespace ModernApplicationFramework.TextEditor
{
    internal sealed class DefaultSelectionPrimitive : Selection
    {
        private readonly IEditorOptions _editorOptions;

        public DefaultSelectionPrimitive(PrimitiveTextView textView, IEditorOptions editorOptions)
        {
            TextView = textView;
            _editorOptions = editorOptions;
        }

        private ITextSelection TextSelection => TextView.AdvancedTextView.Selection;

        private ITextCaret Caret => TextView.AdvancedTextView.Caret;

        private DisplayTextRange TextRange
        {
            get
            {
                var textView = TextView;
                var virtualSnapshotPoint = TextSelection.Start;
                int position1 = virtualSnapshotPoint.Position;
                virtualSnapshotPoint = TextSelection.End;
                int position2 = virtualSnapshotPoint.Position;
                return textView.GetTextRange(position1, position2);
            }
        }

        public override void SelectRange(TextRange textRange)
        {
            SelectRange(textRange.GetStartPoint().CurrentPosition, textRange.GetEndPoint().CurrentPosition);
        }

        public override void SelectRange(TextPoint selectionStart, TextPoint selectionEnd)
        {
            SelectRange(selectionStart.CurrentPosition, selectionEnd.CurrentPosition);
        }

        public override void SelectAll()
        {
            AdvancedSelection.Mode = TextSelectionMode.Stream;
            SelectRange(0, TextView.AdvancedTextView.TextSnapshot.Length);
        }

        public override void ExtendSelection(TextPoint newEnd)
        {
            SelectRange(!IsEmpty ? (!IsReversed ? GetStartPoint().CurrentPosition : GetEndPoint().CurrentPosition) : TextSelection.Start.Position, newEnd.CurrentPosition);
        }

        private void SelectRange(int selectionStart, int selectionEnd)
        {
            var position = new SnapshotPoint(TextView.AdvancedTextView.TextSnapshot, selectionStart);
            var snapshotPoint = new SnapshotPoint(TextView.AdvancedTextView.TextSnapshot, selectionEnd);
            TextSelection.Select(new VirtualSnapshotPoint(position), new VirtualSnapshotPoint(snapshotPoint));
            var containingBufferPosition = TextView.AdvancedTextView.GetTextViewLineContainingBufferPosition(snapshotPoint);
            var caretAffinity = containingBufferPosition.IsLastTextViewLineForSnapshotLine || snapshotPoint != containingBufferPosition.End ? PositionAffinity.Successor : PositionAffinity.Predecessor;
            Caret.MoveTo(snapshotPoint, caretAffinity);
            TextView.AdvancedTextView.ViewScroller.EnsureSpanVisible(TextSelection.StreamSelectionSpan.SnapshotSpan, selectionStart <= selectionEnd ? EnsureSpanVisibleOptions.MinimumScroll : EnsureSpanVisibleOptions.ShowStart | EnsureSpanVisibleOptions.MinimumScroll);
        }

        public override void Clear()
        {
            TextSelection.Clear();
        }

        public override ITextSelection AdvancedSelection => TextSelection;

        public override bool IsEmpty => TextSelection.IsEmpty;

        public override bool IsReversed
        {
            get => TextSelection.IsReversed;
            set
            {
                if (value)
                    SelectRange(TextRange.GetEndPoint().CurrentPosition, TextRange.GetStartPoint().CurrentPosition);
                else
                    SelectRange(TextRange.GetStartPoint().CurrentPosition, TextRange.GetEndPoint().CurrentPosition);
            }
        }

        public override PrimitiveTextView TextView { get; }

        public override DisplayTextPoint GetDisplayStartPoint()
        {
            return TextRange.GetDisplayStartPoint();
        }

        public override DisplayTextPoint GetDisplayEndPoint()
        {
            return TextRange.GetDisplayEndPoint();
        }

        public override VisibilityState Visibility => TextRange.Visibility;

        protected override DisplayTextRange CloneDisplayTextRangeInternal()
        {
            return TextRange.Clone();
        }

        protected override IEnumerator<DisplayTextPoint> GetDisplayPointEnumeratorInternal()
        {
            return TextRange.GetEnumerator();
        }

        public override TextPoint GetStartPoint()
        {
            return TextRange.GetStartPoint();
        }

        public override TextPoint GetEndPoint()
        {
            return TextRange.GetEndPoint();
        }

        public override PrimitiveTextBuffer TextBuffer => TextView.TextBuffer;

        public override SnapshotSpan AdvancedTextRange => TextRange.AdvancedTextRange;

        public override bool MakeUppercase()
        {
            Span advancedTextRange = TextRange.AdvancedTextRange;
            var isReversed = IsReversed;
            if (!TextRange.MakeUppercase())
                return false;
            TextSelection.Select(new SnapshotSpan(TextView.AdvancedTextView.TextSnapshot, advancedTextRange), isReversed);
            return true;
        }

        public override bool MakeLowercase()
        {
            Span advancedTextRange = TextRange.AdvancedTextRange;
            var isReversed = IsReversed;
            if (!TextRange.MakeLowercase())
                return false;
            TextSelection.Select(new SnapshotSpan(TextView.AdvancedTextView.TextSnapshot, advancedTextRange), isReversed);
            return true;
        }

        public override bool Capitalize()
        {
            Span advancedTextRange = TextRange.AdvancedTextRange;
            var isReversed = IsReversed;
            var isEmpty = IsEmpty;
            if (!TextRange.Capitalize())
                return false;
            if (!isEmpty)
                TextSelection.Select(new SnapshotSpan(TextView.AdvancedTextView.TextSnapshot, advancedTextRange), isReversed);
            return true;
        }

        public override bool ToggleCase()
        {
            Span advancedTextRange = TextRange.AdvancedTextRange;
            var isReversed = IsReversed;
            var isEmpty = IsEmpty;
            if (!TextRange.ToggleCase())
                return false;
            if (!isEmpty)
                TextSelection.Select(new SnapshotSpan(TextView.AdvancedTextView.TextSnapshot, advancedTextRange), isReversed);
            return true;
        }

        public override bool Delete()
        {
            foreach (var selectedSpan in TextSelection.SelectedSpans)
            {
                if (!TextView.GetTextRange(selectedSpan.Start, selectedSpan.End).Delete())
                    return false;
            }
            return true;
        }

        public override bool Indent()
        {
            if (GetStartPoint().LineNumber == GetEndPoint().LineNumber && (GetStartPoint().CurrentPosition == GetEndPoint().CurrentPosition || GetStartPoint().CurrentPosition != TextBuffer.GetEndPoint().StartOfLine || GetEndPoint().CurrentPosition != TextBuffer.GetEndPoint().EndOfLine))
            {
                var endPoint = GetEndPoint();
                if (!Delete() || !endPoint.InsertIndent())
                    return false;
                TextView.AdvancedTextView.Caret.MoveTo(endPoint.AdvancedTextPoint);
            }
            else
            {
                var start1 = TextSelection.Start;
                var end1 = TextSelection.End;
                var isReversed = TextSelection.IsReversed;
                var lineFromPosition1 = AdvancedTextRange.Snapshot.GetLineFromPosition(start1.Position);
                var lineFromPosition2 = AdvancedTextRange.Snapshot.GetLineFromPosition(end1.Position);
                var flag1 = start1.Position <= TextView.GetTextPoint(lineFromPosition1.Start).GetFirstNonWhiteSpaceCharacterOnLine().CurrentPosition;
                var flag2 = end1.Position < TextView.GetTextPoint(lineFromPosition2.Start).GetFirstNonWhiteSpaceCharacterOnLine().CurrentPosition;
                var flag3 = AdvancedSelection.Mode == TextSelectionMode.Box;
                if (flag3)
                {
                    if (!BoxIndent())
                        return false;
                }
                else if (!TextRange.Indent())
                    return false;
                var start2 = TextSelection.Start;
                var end2 = TextSelection.End;
                if (!flag3 && flag1 | flag2)
                {
                    SnapshotPoint position1;
                    if (flag1)
                    {
                        ref var local = ref start2;
                        var snapshot = AdvancedTextRange.Snapshot;
                        position1 = start1.Position;
                        var position2 = position1.Position;
                        local = new VirtualSnapshotPoint(snapshot, position2);
                    }
                    if (flag2)
                    {
                        position1 = end1.Position;
                        if (position1.Position != lineFromPosition2.Start && lineFromPosition2.Length != 0)
                        {
                            var num = _editorOptions.IsConvertTabsToSpacesEnabled() ? _editorOptions.GetTabSize() : 1;
                            ref var local = ref end2;
                            var snapshot = AdvancedTextRange.Snapshot;
                            position1 = end2.Position;
                            var position2 = position1.Position - num;
                            local = new VirtualSnapshotPoint(snapshot, position2);
                        }
                    }
                    if (!isReversed)
                        TextSelection.Select(start2, end2);
                    else
                        TextSelection.Select(end2, start2);
                    TextView.AdvancedTextView.Caret.MoveTo(TextSelection.ActivePoint, PositionAffinity.Successor);
                }
            }
            TextView.AdvancedTextView.Caret.EnsureVisible();
            return true;
        }

        private bool BoxIndent()
        {
            var text = _editorOptions.IsConvertTabsToSpacesEnabled() ? new string(' ', _editorOptions.GetTabSize()) : "\t";
            using (ITextEdit edit = TextBuffer.AdvancedTextBuffer.CreateEdit())
            {
                ITextSnapshot currentSnapshot = TextBuffer.AdvancedTextBuffer.CurrentSnapshot;
                var lineNumber1 = GetStartPoint().LineNumber;
                var lineNumber2 = GetEndPoint().LineNumber;
                for (var lineNumber3 = lineNumber1; lineNumber3 <= lineNumber2; ++lineNumber3)
                {
                    var lineFromLineNumber = currentSnapshot.GetLineFromLineNumber(lineNumber3);
                    if (lineFromLineNumber.Length > 0 && !edit.Insert(lineFromLineNumber.Start, text))
                        return false;
                }
                edit.Apply();
                if (edit.Canceled)
                    return false;
            }
            return true;
        }

        public override bool Unindent()
        {
            if (GetStartPoint().LineNumber != GetEndPoint().LineNumber && AdvancedSelection.Mode == TextSelectionMode.Box)
            {
                if (!BoxUnindent())
                    return false;
            }
            else if (!TextRange.Unindent())
                return false;
            TextView.AdvancedTextView.Caret.EnsureVisible();
            return true;
        }

        private bool BoxUnindent()
        {
            using (ITextEdit edit = TextBuffer.AdvancedTextBuffer.CreateEdit())
            {
                ITextSnapshot currentSnapshot = TextBuffer.AdvancedTextBuffer.CurrentSnapshot;
                var lineNumber1 = GetStartPoint().LineNumber;
                var lineNumber2 = GetEndPoint().LineNumber;
                for (var lineNumber3 = lineNumber1; lineNumber3 <= lineNumber2; ++lineNumber3)
                {
                    var lineFromLineNumber = currentSnapshot.GetLineFromLineNumber(lineNumber3);
                    if (lineFromLineNumber.Length > 0)
                    {
                        if (currentSnapshot[lineFromLineNumber.Start] == '\t')
                        {
                            if (!edit.Delete(new Span(lineFromLineNumber.Start, 1)))
                                return false;
                        }
                        else
                        {
                            var length = 0;
                            while (lineFromLineNumber.Start + length < currentSnapshot.Length && length < _editorOptions.GetTabSize() && currentSnapshot[lineFromLineNumber.Start + length] == ' ')
                                ++length;
                            if (length > 0 && !edit.Delete(new Span(lineFromLineNumber.Start, length)))
                                return false;
                        }
                    }
                }
                edit.Apply();
                if (edit.Canceled)
                    return false;
            }
            return true;
        }

        public override TextRange Find(string pattern)
        {
            return TextRange.Find(pattern);
        }

        public override TextRange Find(string pattern, FindOptions findOptions)
        {
            return TextRange.Find(pattern, findOptions);
        }

        public override Collection<TextRange> FindAll(string pattern)
        {
            return TextRange.FindAll(pattern);
        }

        public override Collection<TextRange> FindAll(string pattern, FindOptions findOptions)
        {
            return TextRange.FindAll(pattern, findOptions);
        }

        public override bool ReplaceText(string newText)
        {
            var span = new Span(TextRange.GetDisplayStartPoint().CurrentPosition, newText.Length);
            var isReversed = IsReversed;
            if (!TextRange.ReplaceText(newText))
                return false;
            TextSelection.Select(new SnapshotSpan(TextView.AdvancedTextView.TextSnapshot, span), isReversed);
            return true;
        }

        public override string GetText()
        {
            return TextRange.GetText();
        }

        public override void SetStart(TextPoint startPoint)
        {
            SelectRange(startPoint.CurrentPosition, GetDisplayEndPoint().CurrentPosition);
        }

        public override void SetEnd(TextPoint endPoint)
        {
            ExtendSelection(endPoint);
        }

        public override void MoveTo(TextRange newRange)
        {
            SelectRange(newRange);
        }

        protected override IEnumerator<TextPoint> GetEnumeratorInternal()
        {
            return ((TextRange)TextRange).GetEnumerator();
        }
    }
}