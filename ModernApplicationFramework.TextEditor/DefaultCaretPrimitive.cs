using System.Collections.ObjectModel;

namespace ModernApplicationFramework.TextEditor
{
    internal sealed class DefaultCaretPrimitive : Caret
    {
        private readonly IEditorOptions _editorOptions;

        internal DefaultCaretPrimitive(PrimitiveTextView textView, IEditorOptions editorOptions)
        {
            TextView = textView;
            _editorOptions = editorOptions;
        }

        public override void MoveToNextCharacter(bool extendSelection)
        {
            var virtualBufferPosition = AdvancedCaret.Position.VirtualBufferPosition;
            var endPoint = TextView.Selection.GetEndPoint();
            var isEmpty = TextView.Selection.IsEmpty;
            if (!extendSelection)
                TextView.AdvancedTextView.Selection.Clear();
            if (!extendSelection && !isEmpty)
                MoveTo(endPoint.CurrentPosition);
            else
                AdvancedCaret.MoveToNextCaretPosition();
            AdvancedCaret.EnsureVisible();
            UpdateSelection(extendSelection, virtualBufferPosition);
        }

        public override void MoveToPreviousCharacter(bool extendSelection)
        {
            var virtualBufferPosition = AdvancedCaret.Position.VirtualBufferPosition;
            var startPoint = TextView.Selection.GetStartPoint();
            var isEmpty = TextView.Selection.IsEmpty;
            if (!extendSelection)
                TextView.AdvancedTextView.Selection.Clear();
            if (!extendSelection && !isEmpty)
                MoveTo(startPoint.CurrentPosition);
            else
                AdvancedCaret.MoveToPreviousCaretPosition();
            AdvancedCaret.EnsureVisible();
            UpdateSelection(extendSelection, virtualBufferPosition);
        }

        public override void MoveToBeginningOfPreviousLine(bool extendSelection)
        {
            var virtualBufferPosition = AdvancedCaret.Position.VirtualBufferPosition;
            if (!extendSelection)
                TextView.AdvancedTextView.Selection.Clear();
            MoveToBeginningOfPreviousLine();
            UpdateSelection(extendSelection, virtualBufferPosition);
        }

        public override void MoveToBeginningOfNextLine(bool extendSelection)
        {
            var virtualBufferPosition = AdvancedCaret.Position.VirtualBufferPosition;
            if (!extendSelection)
                TextView.AdvancedTextView.Selection.Clear();
            MoveToBeginningOfNextLine();
            UpdateSelection(extendSelection, virtualBufferPosition);
        }

        public override void MoveToBeginningOfPreviousViewLine(bool extendSelection)
        {
            var virtualBufferPosition = AdvancedCaret.Position.VirtualBufferPosition;
            if (!extendSelection)
                TextView.AdvancedTextView.Selection.Clear();
            MoveToBeginningOfPreviousViewLine();
            UpdateSelection(extendSelection, virtualBufferPosition);
        }

        public override void MoveToBeginningOfNextViewLine(bool extendSelection)
        {
            var virtualBufferPosition = AdvancedCaret.Position.VirtualBufferPosition;
            if (!extendSelection)
                TextView.AdvancedTextView.Selection.Clear();
            MoveToBeginningOfNextViewLine();
            UpdateSelection(extendSelection, virtualBufferPosition);
        }

        public override void MoveToPreviousLine(bool extendSelection)
        {
            var virtualBufferPosition = AdvancedCaret.Position.VirtualBufferPosition;
            var textLine = !(TextView.Selection.IsEmpty | extendSelection) ? TextView.AdvancedTextView.GetTextViewLineContainingBufferPosition(TextView.AdvancedTextView.Selection.Start.Position) : TextView.AdvancedTextView.Caret.ContainingTextViewLine;
            if (textLine.Start != 0)
                textLine = TextView.AdvancedTextView.GetTextViewLineContainingBufferPosition(textLine.Start - 1);
            if (!extendSelection)
                TextView.AdvancedTextView.Selection.Clear();
            AdvancedCaret.MoveTo(textLine);
            AdvancedCaret.EnsureVisible();
            UpdateSelection(extendSelection, virtualBufferPosition);
        }

        public override void MoveToNextLine(bool extendSelection)
        {
            var virtualBufferPosition = AdvancedCaret.Position.VirtualBufferPosition;
            ITextViewLine textLine;
            if (TextView.Selection.IsEmpty | extendSelection)
            {
                textLine = TextView.AdvancedTextView.Caret.ContainingTextViewLine;
            }
            else
            {
                var position = TextView.AdvancedTextView.Selection.End.Position;
                textLine = TextView.AdvancedTextView.GetTextViewLineContainingBufferPosition(position);
                if (!textLine.IsFirstTextViewLineForSnapshotLine && position.Position == textLine.Start.Position)
                    textLine = TextView.AdvancedTextView.GetTextViewLineContainingBufferPosition(position - 1);
            }
            if (!textLine.IsLastTextViewLineForSnapshotLine || textLine.LineBreakLength != 0)
                textLine = TextView.AdvancedTextView.GetTextViewLineContainingBufferPosition(textLine.EndIncludingLineBreak);
            if (!extendSelection)
                TextView.AdvancedTextView.Selection.Clear();
            AdvancedCaret.MoveTo(textLine);
            AdvancedCaret.EnsureVisible();
            UpdateSelection(extendSelection, virtualBufferPosition);
        }

        public override void MoveTo(int position, bool extendSelection)
        {
            var virtualBufferPosition = AdvancedCaret.Position.VirtualBufferPosition;
            var bufferPosition = new SnapshotPoint(TextView.AdvancedTextView.TextSnapshot, position);
            if (!extendSelection)
                TextView.AdvancedTextView.Selection.Clear();
            AdvancedCaret.MoveTo(bufferPosition);
            AdvancedCaret.EnsureVisible();
            UpdateSelection(extendSelection, virtualBufferPosition);
        }

        public override void MovePageUp()
        {
            PageUpDown(ScrollDirection.Up);
        }

        public override void MovePageDown()
        {
            PageUpDown(ScrollDirection.Down);
        }

        public override void MovePageUp(bool extendSelection)
        {
            var virtualBufferPosition = AdvancedCaret.Position.VirtualBufferPosition;
            if (!extendSelection)
                TextView.AdvancedTextView.Selection.Clear();
            MovePageUp();
            UpdateSelection(extendSelection, virtualBufferPosition);
        }

        public override void MovePageDown(bool extendSelection)
        {
            var virtualBufferPosition = AdvancedCaret.Position.VirtualBufferPosition;
            if (!extendSelection)
                TextView.AdvancedTextView.Selection.Clear();
            MovePageDown();
            UpdateSelection(extendSelection, virtualBufferPosition);
        }

        public override void MoveToEndOfLine(bool extendSelection)
        {
            var virtualBufferPosition = AdvancedCaret.Position.VirtualBufferPosition;
            var displayTextPoint = CaretPoint;
            if (!TextView.Selection.IsEmpty && !extendSelection)
                displayTextPoint = TextView.GetTextPoint(TextView.Selection.AdvancedSelection.End.Position);
            displayTextPoint.MoveToEndOfLine();
            if (!extendSelection)
                TextView.AdvancedTextView.Selection.Clear();
            AdvancedCaret.MoveTo(displayTextPoint.AdvancedTextPoint, PositionAffinity.Successor);
            AdvancedCaret.EnsureVisible();
            UpdateSelection(extendSelection, virtualBufferPosition);
        }

        public override void MoveToStartOfLine(bool extendSelection)
        {
            var virtualBufferPosition = AdvancedCaret.Position.VirtualBufferPosition;
            var displayTextPoint = CaretPoint;
            if (!TextView.Selection.IsEmpty && !extendSelection)
                displayTextPoint = TextView.GetTextPoint(TextView.Selection.AdvancedSelection.Start.Position);
            displayTextPoint.MoveToStartOfLine();
            if (!extendSelection)
                TextView.AdvancedTextView.Selection.Clear();
            AdvancedCaret.MoveTo(displayTextPoint.AdvancedTextPoint, PositionAffinity.Successor);
            AdvancedCaret.EnsureVisible();
            UpdateSelection(extendSelection, virtualBufferPosition);
        }

        public override void MoveToEndOfViewLine(bool extendSelection)
        {
            var virtualBufferPosition = AdvancedCaret.Position.VirtualBufferPosition;
            var position = TextView.Selection.AdvancedSelection.End.Position;
            var num = TextView.Selection.IsEmpty ? 1 : 0;
            if (!extendSelection)
                TextView.AdvancedTextView.Selection.Clear();
            if (num == 0 && !extendSelection)
            {
                AdvancedCaret.MoveTo(TextView.AdvancedTextView.GetTextViewLineContainingBufferPosition(position).End, PositionAffinity.Predecessor);
            }
            else
            {
                var containingTextViewLine = AdvancedCaret.ContainingTextViewLine;
                AdvancedCaret.MoveTo(containingTextViewLine.End, containingTextViewLine.IsLastTextViewLineForSnapshotLine ? PositionAffinity.Successor : PositionAffinity.Predecessor);
            }
            AdvancedCaret.EnsureVisible();
            UpdateSelection(extendSelection, virtualBufferPosition);
        }

        public override void MoveToStartOfViewLine(bool extendSelection)
        {
            var virtualBufferPosition = AdvancedCaret.Position.VirtualBufferPosition;
            var position = TextView.Selection.AdvancedSelection.Start.Position;
            var num = TextView.Selection.IsEmpty ? 1 : 0;
            var isReversed = TextView.Selection.AdvancedSelection.IsReversed;
            if (!extendSelection)
                TextView.AdvancedTextView.Selection.Clear();
            AdvancedCaret.MoveTo((num != 0 || isReversed || extendSelection ? AdvancedCaret.ContainingTextViewLine : TextView.AdvancedTextView.GetTextViewLineContainingBufferPosition(position)).Start, PositionAffinity.Successor);
            AdvancedCaret.EnsureVisible();
            UpdateSelection(extendSelection, virtualBufferPosition);
        }

        public override void MoveToLine(int lineNumber, bool extendSelection)
        {
            var virtualBufferPosition = AdvancedCaret.Position.VirtualBufferPosition;
            if (!extendSelection)
                TextView.AdvancedTextView.Selection.Clear();
            MoveToLine(lineNumber);
            UpdateSelection(extendSelection, virtualBufferPosition);
        }

        public override void MoveToLine(int lineNumber, int offset, bool extendSelection)
        {
            var virtualBufferPosition = AdvancedCaret.Position.VirtualBufferPosition;
            if (!extendSelection)
                TextView.AdvancedTextView.Selection.Clear();
            AdvancedCaret.MoveTo(new VirtualSnapshotPoint(TextView.AdvancedTextView.TextSnapshot.GetLineFromLineNumber(lineNumber), offset));
            AdvancedCaret.EnsureVisible();
            UpdateSelection(extendSelection, virtualBufferPosition);
        }

        public override void MoveToStartOfDocument(bool extendSelection)
        {
            var virtualBufferPosition = AdvancedCaret.Position.VirtualBufferPosition;
            if (!extendSelection)
                TextView.AdvancedTextView.Selection.Clear();
            AdvancedCaret.MoveTo(new SnapshotPoint(TextView.AdvancedTextView.TextSnapshot, 0));
            AdvancedCaret.EnsureVisible();
            UpdateSelection(extendSelection, virtualBufferPosition);
        }

        public override void MoveToEndOfDocument(bool extendSelection)
        {
            var virtualBufferPosition = AdvancedCaret.Position.VirtualBufferPosition;
            if (!extendSelection)
                TextView.AdvancedTextView.Selection.Clear();
            AdvancedCaret.MoveTo(new SnapshotPoint(TextView.AdvancedTextView.TextSnapshot, TextView.AdvancedTextView.TextSnapshot.Length));
            AdvancedCaret.EnsureVisible();
            UpdateSelection(extendSelection, virtualBufferPosition);
        }

        public override void MoveToNextWord(bool extendSelection)
        {
            var virtualBufferPosition = AdvancedCaret.Position.VirtualBufferPosition;
            var caretPoint = CaretPoint;
            if (!extendSelection)
                TextView.AdvancedTextView.Selection.Clear();
            caretPoint.MoveToNextWord();
            AdvancedCaret.MoveTo(caretPoint.AdvancedTextPoint);
            AdvancedCaret.EnsureVisible();
            UpdateSelection(extendSelection, virtualBufferPosition);
        }

        public override void MoveToPreviousWord(bool extendSelection)
        {
            var virtualBufferPosition = AdvancedCaret.Position.VirtualBufferPosition;
            var caretPoint = CaretPoint;
            if (!extendSelection)
                TextView.AdvancedTextView.Selection.Clear();
            caretPoint.MoveToPreviousWord();
            AdvancedCaret.MoveTo(caretPoint.AdvancedTextPoint);
            AdvancedCaret.EnsureVisible();
            UpdateSelection(extendSelection, virtualBufferPosition);
        }

        public override void EnsureVisible()
        {
            AdvancedCaret.EnsureVisible();
        }

        public override ITextCaret AdvancedCaret => TextView.AdvancedTextView.Caret;

        public override PrimitiveTextView TextView { get; }

        public override ITextViewLine AdvancedTextViewLine => AdvancedCaret.ContainingTextViewLine;

        public override int DisplayColumn
        {
            get
            {
                var textViewLine = AdvancedTextViewLine;
                if (textViewLine != null && textViewLine.IsValid)
                    return PrimitivesUtilities.GetColumnOfPoint(TextView.AdvancedTextView.TextSnapshot, AdvancedTextPoint, textViewLine.Start, _editorOptions.GetTabSize(), p => textViewLine.GetTextElementSpan(p).End);
                return CaretPoint.DisplayColumn;
            }
        }

        public override bool IsVisible => CaretPoint.IsVisible;

        public override DisplayTextRange GetDisplayTextRange(DisplayTextPoint otherPoint)
        {
            return CaretPoint.GetDisplayTextRange(otherPoint);
        }

        public override DisplayTextRange GetDisplayTextRange(int otherPosition)
        {
            return CaretPoint.GetDisplayTextRange(otherPosition);
        }

        protected override DisplayTextPoint CloneDisplayTextPointInternal()
        {
            return CaretPoint.Clone();
        }

        public override PrimitiveTextBuffer TextBuffer => TextView.TextBuffer;

        public override int CurrentPosition => CaretPoint.CurrentPosition;

        public override int Column => CaretPoint.Column;

        public override bool DeleteNext()
        {
            if (TextView.Selection.IsEmpty)
            {
                if (!CaretPoint.DeleteNext())
                    return false;
            }
            else if (!TextView.Selection.Delete())
                return false;
            AdvancedCaret.EnsureVisible();
            return true;
        }

        public override bool DeletePrevious()
        {
            if (TextView.Selection.IsEmpty)
            {
                if (AdvancedCaret.InVirtualSpace)
                {
                    AdvancedCaret.MoveToPreviousCaretPosition();
                }
                else
                {
                    if (!CaretPoint.DeletePrevious())
                        return false;
                    if (AdvancedCaret.InVirtualSpace)
                        AdvancedCaret.MoveTo(AdvancedCaret.Position.BufferPosition);
                }
            }
            else if (!TextView.Selection.Delete())
                return false;
            AdvancedCaret.EnsureVisible();
            return true;
        }

        public override TextRange GetCurrentWord()
        {
            return CaretPoint.GetCurrentWord();
        }

        public override TextRange GetNextWord()
        {
            return CaretPoint.GetNextWord();
        }

        public override TextRange GetPreviousWord()
        {
            return CaretPoint.GetPreviousWord();
        }

        public override TextRange GetTextRange(TextPoint otherPoint)
        {
            return CaretPoint.GetTextRange(otherPoint);
        }

        public override TextRange GetTextRange(int otherPosition)
        {
            return CaretPoint.GetTextRange(otherPosition);
        }

        public override bool InsertNewLine()
        {
            if (!TextView.Selection.IsEmpty && !TextView.Selection.Delete() || !CaretPoint.InsertNewLine())
                return false;
            AdvancedCaret.EnsureVisible();
            return true;
        }

        public override bool InsertIndent()
        {
            if (!CaretPoint.InsertIndent())
                return false;
            AdvancedCaret.EnsureVisible();
            return true;
        }

        public override bool InsertText(string text)
        {
            if (!TextView.Selection.IsEmpty && !TextView.Selection.Delete() || !CaretPoint.InsertText(text))
                return false;
            AdvancedCaret.EnsureVisible();
            return true;
        }

        public override int LineNumber => CaretPoint.LineNumber;

        public override int StartOfLine => CaretPoint.StartOfLine;

        public override int EndOfLine => CaretPoint.EndOfLine;

        public override int StartOfViewLine => AdvancedTextViewLine.Start;

        public override int EndOfViewLine => AdvancedTextViewLine.End;

        public override bool RemovePreviousIndent()
        {
            if (!CaretPoint.RemovePreviousIndent())
                return false;
            AdvancedCaret.EnsureVisible();
            return true;
        }

        public override bool TransposeCharacter()
        {
            if (!CaretPoint.TransposeCharacter())
                return false;
            TextView.Selection.Clear();
            AdvancedCaret.EnsureVisible();
            return true;
        }

        public override bool TransposeLine()
        {
            if (TextView.Selection.IsEmpty)
            {
                TextPoint caretPoint = CaretPoint;
                var left = AdvancedCaret.Left;
                if (!caretPoint.TransposeLine())
                    return false;
                AdvancedCaret.MoveTo(caretPoint.AdvancedTextPoint, PositionAffinity.Successor, false);
                AdvancedCaret.EnsureVisible();
                AdvancedCaret.MoveTo(AdvancedTextViewLine, left);
                AdvancedCaret.EnsureVisible();
            }
            return true;
        }

        public override bool TransposeLine(int lineNumber)
        {
            if (!TextView.Selection.IsEmpty)
                return true;
            if (!CaretPoint.TransposeLine(lineNumber))
                return false;
            AdvancedCaret.EnsureVisible();
            return true;
        }

        public override SnapshotPoint AdvancedTextPoint => CaretPoint.AdvancedTextPoint;

        public override string GetNextCharacter()
        {
            return CaretPoint.GetNextCharacter();
        }

        public override string GetPreviousCharacter()
        {
            return CaretPoint.GetPreviousCharacter();
        }

        public override TextRange Find(string pattern, FindOptions findOptions, TextPoint endPoint)
        {
            return CaretPoint.Find(pattern, findOptions, endPoint);
        }

        public override TextRange Find(string pattern, TextPoint endPoint)
        {
            return CaretPoint.Find(pattern, endPoint);
        }

        public override TextRange Find(string pattern, FindOptions findOptions)
        {
            return CaretPoint.Find(pattern, findOptions);
        }

        public override TextRange Find(string pattern)
        {
            return CaretPoint.Find(pattern);
        }

        public override Collection<TextRange> FindAll(string pattern, TextPoint endPoint)
        {
            return CaretPoint.FindAll(pattern, endPoint);
        }

        public override Collection<TextRange> FindAll(string pattern, FindOptions findOptions, TextPoint endPoint)
        {
            return CaretPoint.FindAll(pattern, findOptions, endPoint);
        }

        public override Collection<TextRange> FindAll(string pattern)
        {
            return CaretPoint.FindAll(pattern);
        }

        public override Collection<TextRange> FindAll(string pattern, FindOptions findOptions)
        {
            return CaretPoint.FindAll(pattern, findOptions);
        }

        public override void MoveTo(int position)
        {
            MoveTo(position, false);
        }

        public override void MoveToNextCharacter()
        {
            MoveToNextCharacter(false);
        }

        public override void MoveToPreviousCharacter()
        {
            MoveToPreviousCharacter(false);
        }

        public override void MoveToLine(int lineNumber)
        {
            var caretPoint = CaretPoint;
            TextView.Selection.Clear();
            caretPoint.MoveToLine(lineNumber);
            AdvancedCaret.MoveTo(caretPoint.AdvancedTextPoint);
            AdvancedCaret.EnsureVisible();
        }

        public override void MoveToEndOfLine()
        {
            MoveToEndOfLine(false);
        }

        public override void MoveToStartOfLine()
        {
            MoveToStartOfLine(false);
        }

        public override void MoveToEndOfViewLine()
        {
            MoveToEndOfViewLine(false);
        }

        public override void MoveToStartOfViewLine()
        {
            MoveToStartOfViewLine(false);
        }

        public override void MoveToEndOfDocument()
        {
            MoveToEndOfDocument(false);
        }

        public override void MoveToStartOfDocument()
        {
            MoveToStartOfDocument(false);
        }

        public override void MoveToBeginningOfNextLine()
        {
            var caretPoint = CaretPoint;
            caretPoint.MoveToBeginningOfNextLine();
            AdvancedCaret.MoveTo(caretPoint.AdvancedTextPoint);
            AdvancedCaret.EnsureVisible();
        }

        public override void MoveToBeginningOfPreviousLine()
        {
            var caretPoint = CaretPoint;
            caretPoint.MoveToBeginningOfPreviousLine();
            AdvancedCaret.MoveTo(caretPoint.AdvancedTextPoint);
            AdvancedCaret.EnsureVisible();
        }

        public override void MoveToBeginningOfNextViewLine()
        {
            AdvancedCaret.MoveTo(AdvancedTextViewLine.EndIncludingLineBreak);
            AdvancedCaret.EnsureVisible();
        }

        public override void MoveToBeginningOfPreviousViewLine()
        {
            var textViewLine = AdvancedTextViewLine;
            if (textViewLine.Start > 0)
                textViewLine = TextView.AdvancedTextView.GetTextViewLineContainingBufferPosition(textViewLine.Start - 1);
            AdvancedCaret.MoveTo(textViewLine.Start);
            AdvancedCaret.EnsureVisible();
        }

        public override void MoveToNextWord()
        {
            MoveToNextWord(false);
        }

        public override void MoveToPreviousWord()
        {
            MoveToPreviousWord(false);
        }

        public override DisplayTextPoint GetFirstNonWhiteSpaceCharacterOnViewLine()
        {
            var advancedTextViewLine = AdvancedTextViewLine;
            var snapshot = advancedTextViewLine.Extent.Snapshot;
            int start = advancedTextViewLine.Start;
            while (start < advancedTextViewLine.End && char.IsWhiteSpace(snapshot[start]))
                ++start;
            return TextView.GetTextPoint(start);
        }

        public override TextPoint GetFirstNonWhiteSpaceCharacterOnLine()
        {
            return CaretPoint.GetFirstNonWhiteSpaceCharacterOnLine();
        }

        private DisplayTextPoint CaretPoint => TextView.GetTextPoint(TextView.AdvancedTextView.Caret.Position.BufferPosition);

        private void UpdateSelection(bool extendSelection, VirtualSnapshotPoint oldPosition)
        {
            if (extendSelection)
            {
                TextView.AdvancedTextView.Selection.Select(
                    TextView.Selection.IsEmpty
                        ? oldPosition.TranslateTo(TextView.AdvancedTextView.TextSnapshot)
                        : TextView.AdvancedTextView.Selection.AnchorPoint,
                    AdvancedCaret.Position.VirtualBufferPosition);
            }
            else
                TextView.AdvancedTextView.Selection.Clear();
        }

        private void PageUpDown(ScrollDirection direction)
        {
            if (direction == ScrollDirection.Up)
            {
                var firstVisibleLine = TextView.AdvancedTextView.TextViewLines.FirstVisibleLine;
                var bufferPosition = firstVisibleLine.VisibilityState == VisibilityState.FullyVisible ? firstVisibleLine.Start : firstVisibleLine.EndIncludingLineBreak;
                if (TextView.AdvancedTextView.ViewScroller.ScrollViewportVerticallyByPage(direction))
                {
                    if (TextView.AdvancedTextView.ViewportBottom > TextView.AdvancedTextView.TextViewLines.GetTextViewLineContainingBufferPosition(bufferPosition).Bottom)
                        AdvancedCaret.MoveTo(TextView.AdvancedTextView.TextViewLines.FirstVisibleLine);
                    else
                        AdvancedCaret.MoveToPreferredCoordinates();
                }
            }
            else
            {
                var lastVisibleLine = TextView.AdvancedTextView.TextViewLines.LastVisibleLine;
                if (lastVisibleLine.VisibilityState == VisibilityState.FullyVisible && lastVisibleLine.End == lastVisibleLine.Snapshot.Length)
                {
                    AdvancedCaret.MoveTo(lastVisibleLine);
                    return;
                }
                var bufferPosition = lastVisibleLine.VisibilityState == VisibilityState.FullyVisible || (int)lastVisibleLine.Start == 0 ? lastVisibleLine.Start : lastVisibleLine.Start - 1;
                if (TextView.AdvancedTextView.ViewScroller.ScrollViewportVerticallyByPage(direction))
                {
                    if (TextView.AdvancedTextView.TextViewLines.GetTextViewLineContainingBufferPosition(bufferPosition).Bottom > TextView.AdvancedTextView.ViewportTop)
                        AdvancedCaret.MoveTo(TextView.AdvancedTextView.TextViewLines.LastVisibleLine);
                    else
                        AdvancedCaret.MoveToPreferredCoordinates();
                }
            }
            EnsureVisible();
        }
    }
}