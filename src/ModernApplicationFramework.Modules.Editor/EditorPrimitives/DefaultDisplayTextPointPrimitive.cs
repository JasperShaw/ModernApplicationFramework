using System;
using System.Collections.ObjectModel;
using System.Globalization;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Logic.Operations;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Formatting;

namespace ModernApplicationFramework.Modules.Editor.EditorPrimitives
{
    internal sealed class DefaultDisplayTextPointPrimitive : DisplayTextPoint
    {
        private readonly TextPoint _bufferPoint;
        private readonly IEditorOptions _editorOptions;

        public override SnapshotPoint AdvancedTextPoint =>
            _bufferPoint.AdvancedTextPoint.TranslateTo(TextView.AdvancedTextView.TextSnapshot,
                PointTrackingMode.Positive);

        public override ITextViewLine AdvancedTextViewLine =>
            TextView.AdvancedTextView.GetTextViewLineContainingBufferPosition(AdvancedTextPoint);

        public override int Column => _bufferPoint.Column;

        public override int CurrentPosition => _bufferPoint.CurrentPosition;

        public override int DisplayColumn
        {
            get
            {
                var textView = TextView.AdvancedTextView;
                var start = AdvancedTextViewLine.Start;
                return PrimitivesUtilities.GetColumnOfPoint(textView.TextSnapshot, AdvancedTextPoint, start,
                    _editorOptions.GetTabSize(), p => textView.GetTextElementSpan(p).End);
            }
        }

        public override int EndOfLine => _bufferPoint.EndOfLine;

        public override int EndOfViewLine => AdvancedTextViewLine.End;

        public override bool IsVisible => AdvancedTextViewLine.VisibilityState == VisibilityState.FullyVisible;

        public override int LineNumber => _bufferPoint.LineNumber;

        public override int StartOfLine => _bufferPoint.StartOfLine;

        public override int StartOfViewLine => AdvancedTextViewLine.Start;

        public override PrimitiveTextBuffer TextBuffer => TextView.TextBuffer;

        public override PrimitiveTextView TextView { get; }

        internal DefaultDisplayTextPointPrimitive(PrimitiveTextView textView, int position,
            IEditorOptions editorOptions)
        {
            TextView = textView;
            _editorOptions = editorOptions;
            _bufferPoint = TextView.TextBuffer.GetStartPoint();
            MoveTo(TextView.AdvancedTextView.TextSnapshot.CreateTrackingPoint(position, PointTrackingMode.Positive)
                .GetPosition(TextView.TextBuffer.AdvancedTextBuffer.CurrentSnapshot));
        }

        public override bool DeleteNext()
        {
            if (TextView.AdvancedTextView.TextViewModel.IsPointInVisualBuffer(AdvancedTextPoint,
                PositionAffinity.Successor))
                return PrimitivesUtilities.Delete(GetNextTextElementSpan());
            return _bufferPoint.DeleteNext();
        }

        public override bool DeletePrevious()
        {
            var previousTextElementSpan = GetPreviousTextElementSpan();
            if (previousTextElementSpan.Length > 0 &&
                TextView.AdvancedTextView.TextViewModel.IsPointInVisualBuffer(AdvancedTextPoint,
                    PositionAffinity.Successor) &&
                !TextView.AdvancedTextView.TextViewModel.IsPointInVisualBuffer(previousTextElementSpan.End - 1,
                    PositionAffinity.Successor))
                return PrimitivesUtilities.Delete(previousTextElementSpan);
            return _bufferPoint.DeletePrevious();
        }

        public override TextRange Find(string pattern, FindOptions findOptions, TextPoint endPoint)
        {
            return _bufferPoint.Find(pattern, findOptions, endPoint);
        }

        public override TextRange Find(string pattern, TextPoint endPoint)
        {
            return _bufferPoint.Find(pattern, endPoint);
        }

        public override TextRange Find(string pattern, FindOptions findOptions)
        {
            return _bufferPoint.Find(pattern, findOptions);
        }

        public override TextRange Find(string pattern)
        {
            return _bufferPoint.Find(pattern);
        }

        public override Collection<TextRange> FindAll(string pattern, TextPoint endPoint)
        {
            return _bufferPoint.FindAll(pattern, endPoint);
        }

        public override Collection<TextRange> FindAll(string pattern, FindOptions findOptions, TextPoint endPoint)
        {
            return _bufferPoint.FindAll(pattern, findOptions, endPoint);
        }

        public override Collection<TextRange> FindAll(string pattern)
        {
            return _bufferPoint.FindAll(pattern);
        }

        public override Collection<TextRange> FindAll(string pattern, FindOptions findOptions)
        {
            return _bufferPoint.FindAll(pattern, findOptions);
        }

        public override TextRange GetCurrentWord()
        {
            return _bufferPoint.GetCurrentWord();
        }

        public override DisplayTextRange GetDisplayTextRange(DisplayTextPoint otherPoint)
        {
            if (TextBuffer != otherPoint.TextBuffer)
                throw new ArgumentException("The other point must have the same TextBuffer as this one",
                    nameof(otherPoint));
            return TextView.GetTextRange(this, otherPoint);
        }

        public override DisplayTextRange GetDisplayTextRange(int otherPosition)
        {
            return TextView.GetTextRange(CurrentPosition, otherPosition);
        }

        public override TextPoint GetFirstNonWhiteSpaceCharacterOnLine()
        {
            return _bufferPoint.GetFirstNonWhiteSpaceCharacterOnLine();
        }

        public override DisplayTextPoint GetFirstNonWhiteSpaceCharacterOnViewLine()
        {
            var containingBufferPosition =
                TextView.AdvancedTextView.GetTextViewLineContainingBufferPosition(AdvancedTextPoint);
            var snapshot = containingBufferPosition.Extent.Snapshot;
            int start = containingBufferPosition.Start;
            while (start < containingBufferPosition.End && char.IsWhiteSpace(snapshot[start]))
                ++start;
            return new DefaultDisplayTextPointPrimitive(TextView, start, _editorOptions);
        }

        public override string GetNextCharacter()
        {
            if (CurrentPosition == TextView.AdvancedTextView.TextSnapshot.Length)
                return string.Empty;
            return TextView.AdvancedTextView.TextSnapshot.GetText(
                TextView.AdvancedTextView.GetTextElementSpan(AdvancedTextPoint));
        }

        public override TextRange GetNextWord()
        {
            return _bufferPoint.GetNextWord();
        }

        public override string GetPreviousCharacter()
        {
            if (CurrentPosition == 0)
                return string.Empty;
            return TextView.AdvancedTextView.TextSnapshot.GetText(GetPreviousTextElementSpan());
        }

        public override TextRange GetPreviousWord()
        {
            return _bufferPoint.GetPreviousWord();
        }

        public override TextRange GetTextRange(TextPoint otherPoint)
        {
            return _bufferPoint.GetTextRange(otherPoint);
        }

        public override TextRange GetTextRange(int otherPosition)
        {
            return _bufferPoint.GetTextRange(otherPosition);
        }

        public override bool InsertIndent()
        {
            return _bufferPoint.InsertIndent();
        }

        public override bool InsertNewLine()
        {
            return _bufferPoint.InsertNewLine();
        }

        public override bool InsertText(string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            return _bufferPoint.InsertText(text);
        }

        public override void MoveTo(int position)
        {
            _bufferPoint.MoveTo(TextView.AdvancedTextView
                .GetTextElementSpan(new SnapshotPoint(TextView.AdvancedTextView.TextSnapshot, position)).Start);
        }

        public override void MoveToBeginningOfNextLine()
        {
            _bufferPoint.MoveToBeginningOfNextLine();
        }

        public override void MoveToBeginningOfNextViewLine()
        {
            MoveTo(AdvancedTextViewLine.EndIncludingLineBreak);
        }

        public override void MoveToBeginningOfPreviousLine()
        {
            _bufferPoint.MoveToBeginningOfPreviousLine();
        }

        public override void MoveToBeginningOfPreviousViewLine()
        {
            var textViewLine = AdvancedTextViewLine;
            if (textViewLine.Start > 0)
                textViewLine =
                    TextView.AdvancedTextView.GetTextViewLineContainingBufferPosition(textViewLine.Start - 1);
            MoveTo(textViewLine.Start);
        }

        public override void MoveToEndOfDocument()
        {
            _bufferPoint.MoveToEndOfDocument();
        }

        public override void MoveToEndOfLine()
        {
            _bufferPoint.MoveToEndOfLine();
        }

        public override void MoveToEndOfViewLine()
        {
            MoveTo(EndOfViewLine);
        }

        public override void MoveToLine(int lineNumber)
        {
            _bufferPoint.MoveToLine(lineNumber);
        }

        public override void MoveToNextCharacter()
        {
            var currentPosition = CurrentPosition;
            MoveTo(GetNextTextElementSpan().End);
            if (currentPosition != CurrentPosition || currentPosition == AdvancedTextPoint.Snapshot.Length)
                return;
            _bufferPoint.MoveToNextCharacter();
        }

        public override void MoveToNextWord()
        {
            _bufferPoint.MoveToNextWord();
            var advancedTextPoint = _bufferPoint.AdvancedTextPoint;
            var textElementSpan = TextView.AdvancedTextView.GetTextElementSpan(advancedTextPoint);
            if (!(advancedTextPoint > textElementSpan.Start))
                return;
            MoveTo(textElementSpan.End);
        }

        public override void MoveToPreviousCharacter()
        {
            MoveTo(GetPreviousTextElementSpan().Start);
        }

        public override void MoveToPreviousWord()
        {
            _bufferPoint.MoveToPreviousWord();
            MoveTo(_bufferPoint.AdvancedTextPoint);
        }

        public override void MoveToStartOfDocument()
        {
            _bufferPoint.MoveToStartOfDocument();
        }

        public override void MoveToStartOfLine()
        {
            _bufferPoint.MoveToStartOfLine();
        }

        public override void MoveToStartOfViewLine()
        {
            MoveTo(StartOfViewLine);
        }

        public override bool RemovePreviousIndent()
        {
            return _bufferPoint.RemovePreviousIndent();
        }

        public override bool TransposeCharacter()
        {
            var advancedTextPoint = AdvancedTextPoint;
            var lineFromPosition = TextView.AdvancedTextView.TextSnapshot.GetLineFromPosition(advancedTextPoint);
            if (StringInfo.ParseCombiningCharacters(lineFromPosition.GetText()).Length < 2)
                return true;
            SnapshotSpan textElementSpan1;
            SnapshotSpan textElementSpan2;
            if (advancedTextPoint == lineFromPosition.Start)
            {
                textElementSpan1 = TextView.AdvancedTextView.GetTextElementSpan(advancedTextPoint);
                textElementSpan2 = TextView.AdvancedTextView.GetTextElementSpan(textElementSpan1.End);
            }
            else if (advancedTextPoint == lineFromPosition.End)
            {
                textElementSpan2 = TextView.AdvancedTextView.GetTextElementSpan(advancedTextPoint - 1);
                textElementSpan1 = TextView.AdvancedTextView.GetTextElementSpan(textElementSpan2.Start - 1);
            }
            else
            {
                textElementSpan2 = TextView.AdvancedTextView.GetTextElementSpan(advancedTextPoint);
                textElementSpan1 = TextView.AdvancedTextView.GetTextElementSpan(textElementSpan2.Start - 1);
            }

            var replacement = TextView.AdvancedTextView.TextSnapshot.GetText(textElementSpan2) +
                              TextView.AdvancedTextView.TextSnapshot.GetText(textElementSpan1);
            return PrimitivesUtilities.Replace(TextBuffer.AdvancedTextBuffer,
                new Span(textElementSpan1.Start, replacement.Length), replacement);
        }

        public override bool TransposeLine()
        {
            return _bufferPoint.TransposeLine();
        }

        public override bool TransposeLine(int lineNumber)
        {
            return _bufferPoint.TransposeLine(lineNumber);
        }

        protected override DisplayTextPoint CloneDisplayTextPointInternal()
        {
            return new DefaultDisplayTextPointPrimitive(TextView, CurrentPosition, _editorOptions);
        }

        private SnapshotSpan GetNextTextElementSpan()
        {
            return TextView.AdvancedTextView.GetTextElementSpan(AdvancedTextPoint);
        }

        private SnapshotSpan GetPreviousTextElementSpan()
        {
            if (CurrentPosition == 0)
                return new SnapshotSpan(AdvancedTextPoint, 0);
            return TextView.AdvancedTextView.GetTextElementSpan(AdvancedTextPoint - 1);
        }
    }
}