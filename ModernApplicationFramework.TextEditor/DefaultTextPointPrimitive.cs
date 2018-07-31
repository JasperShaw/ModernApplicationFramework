using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace ModernApplicationFramework.TextEditor
{
    internal sealed class DefaultTextPointPrimitive : TextPoint
    {
        private ITrackingPoint _trackingPoint;
        private readonly IEditorOptions _editorOptions;
        private readonly ITextStructureNavigator _textStructureNavigator;
        private readonly ITextSearchService _findLogic;
        private readonly IBufferPrimitivesFactoryService _bufferPrimitivesFactory;

        public DefaultTextPointPrimitive(PrimitiveTextBuffer textBuffer, int position, ITextSearchService findLogic, IEditorOptions editorOptions, ITextStructureNavigator textStructureNavigator, IBufferPrimitivesFactoryService bufferPrimitivesFactory)
        {
            if (position < 0 || position > textBuffer.AdvancedTextBuffer.CurrentSnapshot.Length)
                throw new ArgumentOutOfRangeException(nameof(position));
            TextBuffer = textBuffer;
            _trackingPoint = TextBuffer.AdvancedTextBuffer.CurrentSnapshot.CreateTrackingPoint(0, PointTrackingMode.Positive);
            _editorOptions = editorOptions;
            _textStructureNavigator = textStructureNavigator;
            _findLogic = findLogic;
            _bufferPrimitivesFactory = bufferPrimitivesFactory;
            MoveTo(position);
        }

        public override PrimitiveTextBuffer TextBuffer { get; }

        public override int CurrentPosition => _trackingPoint.GetPosition(TextBuffer.AdvancedTextBuffer.CurrentSnapshot);

        public override int Column
        {
            get
            {
                var point = _trackingPoint.GetPoint(TextBuffer.AdvancedTextBuffer.CurrentSnapshot);
                var containingLine = point.GetContainingLine();
                if (containingLine.Start == point.Position)
                    return 0;
                var stringInfo = new StringInfo(TextBuffer.AdvancedTextBuffer.CurrentSnapshot.GetText(Span.FromBounds(containingLine.Start, point.Position)));
                var num = 0;
                for (var startingTextElement = 0; startingTextElement < stringInfo.LengthInTextElements; ++startingTextElement)
                {
                    if (stringInfo.SubstringByTextElements(startingTextElement, 1) == "\t")
                    {
                        var tabSize = _editorOptions.GetTabSize();
                        num = (num / tabSize + 1) * tabSize;
                    }
                    else
                        ++num;
                }
                return num;
            }
        }

        public override bool DeleteNext()
        {
            return PrimitivesUtilities.Delete(TextBuffer.AdvancedTextBuffer, CurrentPosition, GetNextCharacter().Length);
        }

        public override bool DeletePrevious()
        {
            var currentSnapshot = TextBuffer.AdvancedTextBuffer.CurrentSnapshot;
            var index = CurrentPosition - 1;
            if (index < 0)
                return true;
            var start = index;
            var c = currentSnapshot[index];
            if (char.GetUnicodeCategory(c) == UnicodeCategory.Surrogate)
                --start;
            if (start > 0 && c == '\n' && currentSnapshot[index - 1] == '\r')
                --start;
            return PrimitivesUtilities.Delete(TextBuffer.AdvancedTextBuffer, start, index - start + 1);
        }

        public override TextRange GetCurrentWord()
        {
            var advancedTextPoint = AdvancedTextPoint;
            var containingLine = advancedTextPoint.GetContainingLine();
            if (advancedTextPoint == containingLine.Start && containingLine.Length == 0)
                return _bufferPrimitivesFactory.CreateTextRange(TextBuffer, this, this);
            var textExtent1 = GetTextExtent(CurrentPosition);
            if (!textExtent1.IsSignificant && textExtent1.Span.Start > 0 && textExtent1.Span.Length > 0)
            {
                if (textExtent1.Span.Start == CurrentPosition)
                {
                    if (!char.IsWhiteSpace(TextBuffer.AdvancedTextBuffer.CurrentSnapshot[textExtent1.Span.Start - 1]))
                        textExtent1 = GetTextExtent(textExtent1.Span.Start - 1);
                }
                else if (CurrentPosition == EndOfLine && !char.IsWhiteSpace(TextBuffer.AdvancedTextBuffer.CurrentSnapshot[textExtent1.Span.Start - 1]))
                {
                    textExtent1 =
                        new TextExtent(new SnapshotSpan(new TextExtent(GetTextExtent(textExtent1.Span.Start - 1)).Span.Start,
                                textExtent1.Span.End), true);
                }
            }
            var startPoint = Clone();
            startPoint.MoveTo(textExtent1.Span.Start);
            var endPoint = Clone();
            endPoint.MoveTo(textExtent1.Span.End);
            return _bufferPrimitivesFactory.CreateTextRange(TextBuffer, startPoint, endPoint);
        }

        private TextExtent GetTextExtent(int position)
        {
            return _textStructureNavigator.GetExtentOfWord(new SnapshotPoint(TextBuffer.AdvancedTextBuffer.CurrentSnapshot, position));
        }

        public override TextRange GetNextWord()
        {
            var textExtent1 = GetTextExtent(CurrentPosition);
            if (textExtent1.Span.End >= TextBuffer.AdvancedTextBuffer.CurrentSnapshot.Length)
                return _bufferPrimitivesFactory.CreateTextRange(TextBuffer, TextBuffer.GetEndPoint(), TextBuffer.GetEndPoint());
            if (CurrentPosition == EndOfLine && CurrentPosition != TextBuffer.AdvancedTextBuffer.CurrentSnapshot.Length)
            {
                var textPoint = Clone();
                textPoint.MoveToBeginningOfNextLine();
                textPoint.MoveTo(textPoint.StartOfLine);
                var textExtent2 = GetTextExtent(textPoint.CurrentPosition);
                if (textExtent2.IsSignificant)
                    return textPoint.GetCurrentWord();
                if (textExtent2.Span.End >= textPoint.EndOfLine)
                    return textPoint.GetTextRange(textPoint.EndOfLine);
                return textPoint.GetNextWord();
            }
            var span = textExtent1.Span;
            if (ShouldStopAtEndOfLine(span.End) || IsCurrentWordABlankLine(textExtent1))
            {
                span = textExtent1.Span;
                return GetTextRange(span.End);
            }
            span = textExtent1.Span;
            var textExtent3 = GetTextExtent(span.End);
            if (!textExtent3.IsSignificant)
            {
                span = textExtent3.Span;
                textExtent3 = GetTextExtent(span.End);
            }
            span = textExtent3.Span;
            int start = span.Start;
            span = textExtent3.Span;
            int end1 = span.End;
            var val1 = start;
            span = textExtent1.Span;
            int end2 = span.End;
            var position = Math.Max(val1, end2);
            var startPoint = Clone();
            var endPoint = Clone();
            startPoint.MoveTo(position);
            endPoint.MoveTo(end1);
            return _bufferPrimitivesFactory.CreateTextRange(TextBuffer, startPoint, endPoint);
        }

        public override TextRange GetPreviousWord()
        {
            var currentWord1 = GetCurrentWord();
            if (currentWord1.GetStartPoint().CurrentPosition <= 0)
                return _bufferPrimitivesFactory.CreateTextRange(TextBuffer, TextBuffer.GetStartPoint(), TextBuffer.GetStartPoint());
            if (currentWord1.GetStartPoint().CurrentPosition == StartOfLine && CurrentPosition != StartOfLine)
                return GetTextRange(currentWord1.GetStartPoint());
            if (currentWord1.GetEndPoint().CurrentPosition == CurrentPosition && !currentWord1.IsEmpty)
                return currentWord1;
            var startPoint = currentWord1.GetStartPoint();
            startPoint.MoveTo(startPoint.CurrentPosition - 1);
            var currentWord2 = startPoint.GetCurrentWord();
            if (currentWord2.GetStartPoint().CurrentPosition > 0 && ShouldContinuePastPreviousWord(currentWord2))
            {
                startPoint.MoveTo(currentWord2.GetStartPoint().CurrentPosition - 1);
                currentWord2 = startPoint.GetCurrentWord();
            }
            return currentWord2;
        }

        public override TextRange GetTextRange(TextPoint otherPoint)
        {
            if (otherPoint == null)
                throw new ArgumentNullException(nameof(otherPoint));
            if (otherPoint.TextBuffer != TextBuffer)
                throw new ArgumentException();
            return _bufferPrimitivesFactory.CreateTextRange(TextBuffer, Clone(), otherPoint);
        }

        public override TextRange GetTextRange(int otherPosition)
        {
            if (otherPosition < 0 || otherPosition > TextBuffer.AdvancedTextBuffer.CurrentSnapshot.Length)
                throw new ArgumentOutOfRangeException(nameof(otherPosition));
            var endPoint = Clone();
            endPoint.MoveTo(otherPosition);
            return _bufferPrimitivesFactory.CreateTextRange(TextBuffer, Clone(), endPoint);
        }

        public override bool InsertNewLine()
        {
            string str = null;
            if (_editorOptions.GetReplicateNewLineCharacter())
            {
                var currentSnapshot = TextBuffer.AdvancedTextBuffer.CurrentSnapshot;
                var position = _trackingPoint.GetPosition(currentSnapshot);
                var lineFromPosition = currentSnapshot.GetLineFromPosition(position);
                if (lineFromPosition.LineBreakLength > 0)
                    str = lineFromPosition.GetLineBreakText();
                else if (currentSnapshot.LineCount > 1)
                    str = currentSnapshot.GetLineFromLineNumber(currentSnapshot.LineCount - 2).GetLineBreakText();
            }
            return InsertText(str ?? _editorOptions.GetNewLineCharacter());
        }

        public override bool InsertIndent()
        {
            var tabSize = _editorOptions.GetTabSize();
            return InsertText(_editorOptions.IsConvertTabsToSpacesEnabled() ? new string(' ', tabSize - Column % tabSize) : "\t");
        }

        public override bool InsertText(string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            if (text.Length > 0)
                return PrimitivesUtilities.Insert(TextBuffer.AdvancedTextBuffer, _trackingPoint.GetPosition(TextBuffer.AdvancedTextBuffer.CurrentSnapshot), text);
            return true;
        }

        public override int LineNumber => _trackingPoint.GetPoint(TextBuffer.AdvancedTextBuffer.CurrentSnapshot).GetContainingLine().LineNumber;

        public override int StartOfLine => _trackingPoint.GetPoint(TextBuffer.AdvancedTextBuffer.CurrentSnapshot).GetContainingLine().Start;

        public override int EndOfLine => _trackingPoint.GetPoint(TextBuffer.AdvancedTextBuffer.CurrentSnapshot).GetContainingLine().End;

        public override bool RemovePreviousIndent()
        {
            if (Column <= 0)
                return true;
            var tabSize = _editorOptions.GetTabSize();
            var num = Column - tabSize;
            if (Column % tabSize > 0)
                num = Column / tabSize * tabSize;
            var start = CurrentPosition;
            var textPoint = Clone();
            var position = CurrentPosition - 1;
            while (textPoint.Column >= num)
            {
                textPoint.MoveTo(position);
                var nextCharacter = textPoint.GetNextCharacter();
                if (nextCharacter == " " || nextCharacter == "\t")
                {
                    start = position;
                    if (textPoint.Column != num)
                        --position;
                    else
                        break;
                }
                else
                    break;
            }
            return PrimitivesUtilities.Delete(TextBuffer.AdvancedTextBuffer, Span.FromBounds(start, CurrentPosition));
        }

        public override bool TransposeCharacter()
        {
            var currentPosition = CurrentPosition;
            var lineFromPosition = TextBuffer.AdvancedTextBuffer.CurrentSnapshot.GetLineFromPosition(currentPosition);
            var text = lineFromPosition.GetText();
            if (StringInfo.ParseCombiningCharacters(text).Length < 2)
                return true;
            var index = CurrentPosition - StartOfLine;
            string nextTextElement1;
            Span span1;
            string nextTextElement2;
            if (currentPosition == lineFromPosition.Start)
            {
                nextTextElement1 = StringInfo.GetNextTextElement(text, index);
                span1 = new Span(index + lineFromPosition.Start, nextTextElement1.Length);
                nextTextElement2 = StringInfo.GetNextTextElement(text, index + nextTextElement1.Length);
            }
            else if (currentPosition == lineFromPosition.End)
            {
                nextTextElement2 = StringInfo.GetNextTextElement(text, index - 1);
                var span2 = new Span(index - 1 + lineFromPosition.Start, nextTextElement2.Length);
                nextTextElement1 = StringInfo.GetNextTextElement(text, span2.Start - lineFromPosition.Start - 1);
                span1 = new Span(span2.Start - 1, nextTextElement2.Length);
            }
            else
            {
                nextTextElement2 = StringInfo.GetNextTextElement(text, index);
                var span2 = new Span(index + lineFromPosition.Start, nextTextElement2.Length);
                nextTextElement1 = StringInfo.GetNextTextElement(text, span2.Start - lineFromPosition.Start - 1);
                span1 = new Span(span2.Start - 1, nextTextElement2.Length);
            }
            var replacement = nextTextElement2 + nextTextElement1;
            return PrimitivesUtilities.Replace(TextBuffer.AdvancedTextBuffer, new Span(span1.Start, replacement.Length), replacement);
        }

        public override bool TransposeLine()
        {
            var currentSnapshot = TextBuffer.AdvancedTextBuffer.CurrentSnapshot;
            if (currentSnapshot.LineCount <= 1)
                return true;
            var lineFromPosition = currentSnapshot.GetLineFromPosition(CurrentPosition);
            return TransposeLine((lineFromPosition.LineNumber != currentSnapshot.LineCount - 1 ? currentSnapshot.GetLineFromLineNumber(lineFromPosition.LineNumber + 1) : currentSnapshot.GetLineFromLineNumber(lineFromPosition.LineNumber - 1)).LineNumber);
        }

        public override bool TransposeLine(int lineNumber)
        {
            if (lineNumber < 0 || lineNumber > TextBuffer.AdvancedTextBuffer.CurrentSnapshot.LineCount)
                throw new ArgumentOutOfRangeException(nameof(lineNumber));
            var currentSnapshot = TextBuffer.AdvancedTextBuffer.CurrentSnapshot;
            if (currentSnapshot.LineCount <= 1)
                return true;
            var lineFromPosition = currentSnapshot.GetLineFromPosition(CurrentPosition);
            var lineFromLineNumber = currentSnapshot.GetLineFromLineNumber(lineNumber);
            using (var edit = TextBuffer.AdvancedTextBuffer.CreateEdit())
            {
                if (edit.Replace(lineFromPosition.Extent, lineFromLineNumber.GetText()) && edit.Replace(lineFromLineNumber.Extent, lineFromPosition.GetText()))
                {
                    edit.Apply();
                    if (edit.Canceled)
                        return false;
                }
                else
                {
                    edit.Cancel();
                    return false;
                }
            }
            var lineNumber1 = lineFromLineNumber.LineNumber;
            if (lineFromPosition.LineNumber == currentSnapshot.LineCount - 1)
                lineNumber1 = lineFromPosition.LineNumber;
            MoveTo(TextBuffer.AdvancedTextBuffer.CurrentSnapshot.GetLineFromLineNumber(lineNumber1).Start);
            return true;
        }

        public override SnapshotPoint AdvancedTextPoint => _trackingPoint.GetPoint(TextBuffer.AdvancedTextBuffer.CurrentSnapshot);

        public override string GetNextCharacter()
        {
            var currentSnapshot = TextBuffer.AdvancedTextBuffer.CurrentSnapshot;
            var currentPosition = CurrentPosition;
            if (currentPosition == currentSnapshot.Length)
                return string.Empty;
            var index = currentPosition;
            var str1 = char.ToString(currentSnapshot[currentPosition]);
            while (++index < currentSnapshot.Length)
            {
                var str2 = str1 + currentSnapshot[index].ToString();
                if (StringInfo.GetNextTextElement(str2).Length > str1.Length)
                    str1 = str2;
                else
                    break;
            }
            return str1;
        }

        public override string GetPreviousCharacter()
        {
            var currentSnapshot = TextBuffer.AdvancedTextBuffer.CurrentSnapshot;
            var currentPosition = CurrentPosition;
            if (currentPosition == 0)
                return string.Empty;
            var index = currentPosition - 1;
            var str1 = char.ToString(currentSnapshot[index]);
            while (--index >= 0)
            {
                var str2 = currentSnapshot[index].ToString() + str1;
                if (StringInfo.GetNextTextElement(str2).Length > str1.Length)
                    str1 = str2;
                else
                    break;
            }
            return str1;
        }

        public override TextRange Find(string pattern, TextPoint endPoint)
        {
            ValidateFindParameters(pattern, endPoint);
            return Find(pattern, FindOptions.None, endPoint);
        }

        public override TextRange Find(string pattern)
        {
            ValidateFindParameters(pattern, this);
            return Find(pattern, FindOptions.None);
        }

        public override TextRange Find(string pattern, FindOptions findOptions)
        {
            ValidateFindParameters(pattern, this);
            return FindMatch(pattern, findOptions, null);
        }

        public override TextRange Find(string pattern, FindOptions findOptions, TextPoint endPoint)
        {
            ValidateFindParameters(pattern, endPoint);
            return FindMatch(pattern, findOptions, endPoint);
        }

        private TextRange FindMatch(string pattern, FindOptions findOptions, TextPoint endPoint)
        {
            if (pattern.Length == 0)
                return _bufferPrimitivesFactory.CreateTextRange(TextBuffer, Clone(), Clone());
            var findData =
                new FindData(pattern, TextBuffer.AdvancedTextBuffer.CurrentSnapshot) {FindOptions = findOptions};
            var wraparound = endPoint == null;
            var next = _findLogic.FindNext((findData.FindOptions & FindOptions.SearchReverse) != FindOptions.SearchReverse || wraparound ? CurrentPosition : endPoint.CurrentPosition, wraparound, findData);
            if (next.HasValue)
            {
                SnapshotSpan snapshotSpan;
                if (endPoint != null)
                {
                    snapshotSpan = next.Value;
                    if (snapshotSpan.End > endPoint.CurrentPosition)
                        goto label_6;
                }
                var primitivesFactory1 = _bufferPrimitivesFactory;
                PrimitiveTextBuffer textBuffer1 = TextBuffer;
                var primitivesFactory2 = _bufferPrimitivesFactory;
                PrimitiveTextBuffer textBuffer2 = TextBuffer;
                snapshotSpan = next.Value;
                int start = snapshotSpan.Start;
                var textPoint1 = primitivesFactory2.CreateTextPoint(textBuffer2, start);
                var primitivesFactory3 = _bufferPrimitivesFactory;
                PrimitiveTextBuffer textBuffer3 = TextBuffer;
                snapshotSpan = next.Value;
                int end = snapshotSpan.End;
                var textPoint2 = primitivesFactory3.CreateTextPoint(textBuffer3, end);
                return primitivesFactory1.CreateTextRange(textBuffer1, textPoint1, textPoint2);
            }
            label_6:
            return _bufferPrimitivesFactory.CreateTextRange(TextBuffer, Clone(), Clone());
        }

        public override Collection<TextRange> FindAll(string pattern, TextPoint endPoint)
        {
            return FindAll(pattern, FindOptions.None, endPoint);
        }

        public override Collection<TextRange> FindAll(string pattern)
        {
            return FindAll(pattern, FindOptions.None);
        }

        public override Collection<TextRange> FindAll(string pattern, FindOptions findOptions)
        {
            ValidateFindParameters(pattern, this);
            if (pattern.Length == 0)
                return new Collection<TextRange>();
            var all = _findLogic.FindAll(new FindData(pattern, TextBuffer.AdvancedTextBuffer.CurrentSnapshot)
            {
                FindOptions = findOptions
            });
            var textRangeList = new List<TextRange>();
            var collection = new Collection<TextRange>();
            foreach (var snapshotSpan in all)
            {
                var textRange = _bufferPrimitivesFactory.CreateTextRange(TextBuffer, _bufferPrimitivesFactory.CreateTextPoint(TextBuffer, snapshotSpan.Start), _bufferPrimitivesFactory.CreateTextPoint(TextBuffer, snapshotSpan.End));
                if (textRange.GetStartPoint().CurrentPosition < CurrentPosition)
                    collection.Add(textRange);
                else
                    textRangeList.Add(textRange);
            }
            textRangeList.AddRange(collection);
            return new Collection<TextRange>(textRangeList);
        }

        public override Collection<TextRange> FindAll(string pattern, FindOptions findOptions, TextPoint endPoint)
        {
            ValidateFindParameters(pattern, endPoint);
            var collection = new Collection<TextRange>();
            foreach (var textRange in FindAll(pattern, findOptions))
            {
                if (textRange.GetStartPoint().CurrentPosition >= CurrentPosition && textRange.GetEndPoint().CurrentPosition <= endPoint.CurrentPosition)
                    collection.Add(textRange);
            }
            return collection;
        }

        private void ValidateFindParameters(string pattern, TextPoint endPoint)
        {
            if (pattern == null)
                throw new ArgumentNullException(nameof(pattern));
            if (endPoint == null)
                throw new ArgumentNullException(nameof(endPoint));
            if (endPoint.TextBuffer != TextBuffer)
                throw new ArgumentException();
        }

        public override void MoveTo(int position)
        {
            position = FindStartOfCombiningSequence(TextBuffer.AdvancedTextBuffer.CurrentSnapshot, position);
            _trackingPoint = TextBuffer.AdvancedTextBuffer.CurrentSnapshot.CreateTrackingPoint(position, PointTrackingMode.Positive);
        }

        private static int FindStartOfCombiningSequence(ITextSnapshot snapshot, int position)
        {
            if (position < 0 || position > snapshot.Length)
                throw new ArgumentOutOfRangeException(nameof(position));
            if (position == snapshot.Length || !IsPotentialCombiningCharacter(CharUnicodeInfo.GetUnicodeCategory(snapshot[position])))
                return position;
            var lineFromPosition = snapshot.GetLineFromPosition(position);
            if (position == lineFromPosition.Start)
                return position;
            var end = position + 1;
            int start = lineFromPosition.Start;
            for (--position; position > start; --position)
            {
                if (!IsPotentialCombiningCharacter(CharUnicodeInfo.GetUnicodeCategory(snapshot[position])))
                {
                    var combiningCharacters = StringInfo.ParseCombiningCharacters(snapshot.GetText(Span.FromBounds(position, end)));
                    if (combiningCharacters.Length > 1)
                        return position + combiningCharacters[combiningCharacters.Length - 1];
                }
            }
            if (position == lineFromPosition.Start)
            {
                var combiningCharacters = StringInfo.ParseCombiningCharacters(snapshot.GetText(Span.FromBounds(position, end)));
                if (combiningCharacters.Length > 1)
                    return position + combiningCharacters[combiningCharacters.Length - 1];
            }
            return position;
        }

        private static bool IsPotentialCombiningCharacter(UnicodeCategory category)
        {
            if (category != UnicodeCategory.EnclosingMark && category != UnicodeCategory.NonSpacingMark && category != UnicodeCategory.SpacingCombiningMark)
                return category == UnicodeCategory.Surrogate;
            return true;
        }

        public override void MoveToNextCharacter()
        {
            MoveTo(CurrentPosition + GetNextCharacter().Length);
        }

        public override void MoveToPreviousCharacter()
        {
            MoveTo(CurrentPosition - GetPreviousCharacter().Length);
        }

        protected override TextPoint CloneInternal()
        {
            return new DefaultTextPointPrimitive(TextBuffer, CurrentPosition, _findLogic, _editorOptions, _textStructureNavigator, _bufferPrimitivesFactory);
        }

        public override void MoveToLine(int lineNumber)
        {
            if (lineNumber < 0 || lineNumber > TextBuffer.AdvancedTextBuffer.CurrentSnapshot.LineCount)
                throw new ArgumentOutOfRangeException(nameof(lineNumber));
            MoveTo(TextBuffer.AdvancedTextBuffer.CurrentSnapshot.GetLineFromLineNumber(lineNumber).Start);
        }

        public override void MoveToEndOfLine()
        {
            MoveTo(EndOfLine);
        }

        public override void MoveToStartOfLine()
        {
            var currentPosition1 = GetFirstNonWhiteSpaceCharacterOnLine().CurrentPosition;
            var currentPosition2 = CurrentPosition;
            var startOfLine = StartOfLine;
            if (currentPosition2 == currentPosition1 || currentPosition1 == EndOfLine)
            {
                MoveTo(startOfLine);
            }
            else
            {
                if (currentPosition2 <= currentPosition1 && currentPosition2 != 0)
                    return;
                MoveTo(currentPosition1);
            }
        }

        public override void MoveToEndOfDocument()
        {
            MoveTo(TextBuffer.AdvancedTextBuffer.CurrentSnapshot.Length);
        }

        public override void MoveToStartOfDocument()
        {
            MoveTo(0);
        }

        public override void MoveToBeginningOfNextLine()
        {
            MoveTo(AdvancedTextPoint.GetContainingLine().EndIncludingLineBreak);
        }

        public override void MoveToBeginningOfPreviousLine()
        {
            var containingLine = AdvancedTextPoint.GetContainingLine();
            if (containingLine.LineNumber != 0)
                MoveTo(TextBuffer.AdvancedTextBuffer.CurrentSnapshot.GetLineFromLineNumber(containingLine.LineNumber - 1).Start);
            else
                MoveToStartOfLine();
        }

        public override void MoveToNextWord()
        {
            if (CurrentPosition == TextBuffer.AdvancedTextBuffer.CurrentSnapshot.Length)
                return;
            var nextWord = GetNextWord();
            MoveTo(nextWord.GetStartPoint().CurrentPosition == CurrentPosition
                ? nextWord.GetEndPoint().CurrentPosition
                : nextWord.GetStartPoint().CurrentPosition);
        }

        public override void MoveToPreviousWord()
        {
            if (CurrentPosition == 0)
                return;
            var currentWord = GetCurrentWord();
            MoveTo(CurrentPosition != currentWord.GetStartPoint().CurrentPosition
                ? currentWord.GetStartPoint().CurrentPosition
                : GetPreviousWord().GetStartPoint().CurrentPosition);
        }

        public override TextPoint GetFirstNonWhiteSpaceCharacterOnLine()
        {
            var startOfLine = StartOfLine;
            while (startOfLine < EndOfLine && char.IsWhiteSpace(AdvancedTextPoint.Snapshot[startOfLine]))
                ++startOfLine;
            return _bufferPrimitivesFactory.CreateTextPoint(TextBuffer, startOfLine);
        }

        private bool IsCurrentWordABlankLine(TextExtent currentWord)
        {
            var textPoint = Clone();
            textPoint.MoveTo(currentWord.Span.End);
            if (EndOfLine == CurrentPosition)
                return currentWord.Span.End == textPoint.EndOfLine;
            return false;
        }

        private bool ShouldStopAtEndOfLine(int endOfWord)
        {
            if (endOfWord == EndOfLine)
                return EndOfLine > CurrentPosition;
            return false;
        }

        private static bool ShouldContinuePastPreviousWord(TextRange previousWord)
        {
            if (char.IsWhiteSpace(previousWord.GetStartPoint().GetNextCharacter()[0]))
                return previousWord.GetStartPoint().CurrentPosition != previousWord.GetStartPoint().StartOfLine;
            return false;
        }
    }
}