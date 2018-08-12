using System;
using System.Globalization;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Operations;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Modules.Editor.Operations
{
    internal class NaturalLanguageNavigator : ITextStructureNavigator
    {
        private readonly ITextBuffer _textBuffer;

        internal NaturalLanguageNavigator(ITextBuffer textBuffer)
        {
            _textBuffer = textBuffer;
        }

        public TextExtent GetExtentOfWord(SnapshotPoint currentPosition)
        {
            if (currentPosition.Snapshot.TextBuffer != _textBuffer)
                throw new ArgumentException("currentPosition  TextBuffer does not match with current TextBuffer");
            if (currentPosition == currentPosition.GetContainingLine().End)
                return new TextExtent(new SnapshotSpan(currentPosition, 0), false);
            var containingLine = currentPosition.GetContainingLine();
            var bufferLineText = new PseudoString(containingLine);
            var characterType = GetCharacterType(currentPosition.GetChar());
            return new TextExtent(
                new SnapshotSpan(FindStartOfWord(currentPosition, containingLine, bufferLineText, characterType),
                    FindEndOfWord(currentPosition, containingLine, bufferLineText, characterType)),
                characterType != CharacterType.WhiteSpace);
        }

        public SnapshotSpan GetSpanOfEnclosing(SnapshotSpan activeSpan)
        {
            var spanType = GetSpanType(activeSpan);
            return GetSpanOfEnclosing(activeSpan, spanType);
        }

        public SnapshotSpan GetSpanOfFirstChild(SnapshotSpan activeSpan)
        {
            var spanType = GetSpanType(activeSpan);
            return GetSpanOfFirstChild(activeSpan, spanType);
        }

        public SnapshotSpan GetSpanOfNextSibling(SnapshotSpan activeSpan)
        {
            var spanType = GetSpanType(activeSpan);
            return GetSpanOfNextSibling(activeSpan, spanType);
        }

        public SnapshotSpan GetSpanOfPreviousSibling(SnapshotSpan activeSpan)
        {
            var spanType = GetSpanType(activeSpan);
            return GetSpanOfPreviousSibling(activeSpan, spanType);
        }

        public IContentType ContentType => _textBuffer.ContentType;

        private static CharacterType GetCharacterType(char c)
        {
            if (char.IsLetterOrDigit(c) || "_$".IndexOf(c) != -1)
                return CharacterType.AlphaNumeric;
            if (char.IsWhiteSpace(c))
                return CharacterType.WhiteSpace;
            return char.IsSurrogate(c) || UnicodeCategory.NonSpacingMark == char.GetUnicodeCategory(c) ? CharacterType.AlphaNumeric : CharacterType.Symbols;
        }

        private static SnapshotPoint FindEndOfWord(SnapshotPoint currentPosition, ITextSnapshotLine snapshotLine, PseudoString bufferLineText, CharacterType baseType)
        {
            if (currentPosition >= currentPosition.Snapshot.Length - 1)
                return new SnapshotPoint(currentPosition.Snapshot, currentPosition.Snapshot.Length);
            var start = (int)snapshotLine.Start;
            var position = (int)currentPosition;
            while (++position - start < bufferLineText.Length)
            {
                if (GetCharacterType(bufferLineText[position - start]) != baseType)
                    return new SnapshotPoint(currentPosition.Snapshot, position);
            }
            return snapshotLine.End;
        }

        private static SnapshotPoint FindStartOfWord(SnapshotPoint currentPosition, ITextSnapshotLine snapshotLine, PseudoString bufferLineText, CharacterType baseType)
        {
            if (currentPosition == 0)
                return new SnapshotPoint(currentPosition.Snapshot, 0);
            var start = (int)snapshotLine.Start;
            var num = (int)currentPosition;
            while (--num >= start)
            {
                if (GetCharacterType(bufferLineText[num - start]) != baseType)
                    return new SnapshotPoint(currentPosition.Snapshot, num + 1);
            }
            return new SnapshotPoint(currentPosition.Snapshot, start);
        }

        private SpanType GetSpanType(SnapshotSpan activeSpan)
        {
            if (activeSpan.IsEmpty)
                return SpanType.Empty;
            var start = activeSpan.Start;
            if (activeSpan == GetSpanOfDocument(start.Snapshot))
                return SpanType.Document;
            var spanOfWord = GetSpanOfWord(start);
            if (spanOfWord == activeSpan)
                return SpanType.Word;
            if (spanOfWord.Contains(activeSpan))
                return SpanType.MultipleCharacters;
            var spanOfSentence = GetSpanOfSentence(start);
            if (spanOfSentence == activeSpan)
                return SpanType.Sentence;
            if (spanOfSentence.Contains(activeSpan))
                return SpanType.MultipleWords;
            var spanOfParagraph = GetSpanOfParagraph(start);
            if (spanOfParagraph == activeSpan)
                return SpanType.Paragraph;
            return spanOfParagraph.Contains(activeSpan) ? SpanType.MultipleSentences : SpanType.MultipleParagraphs;
        }

        private SnapshotSpan GetSpanOfEnclosing(SnapshotSpan activeSpan, SpanType spanType)
        {
            var start = activeSpan.Start;
            switch (spanType)
            {
                case SpanType.Empty:
                case SpanType.MultipleCharacters:
                    var spanOfWord = GetSpanOfWord(start);
                    if (activeSpan.Contains(spanOfWord))
                        return GetSpanOfEnclosing(activeSpan, SpanType.Word);
                    return spanOfWord;
                case SpanType.Word:
                case SpanType.MultipleWords:
                    var spanOfSentence = GetSpanOfSentence(start);
                    if (activeSpan.Contains(spanOfSentence))
                        return GetSpanOfEnclosing(activeSpan, SpanType.Sentence);
                    return spanOfSentence;
                case SpanType.Sentence:
                case SpanType.MultipleSentences:
                    var spanOfParagraph = GetSpanOfParagraph(start);
                    if (activeSpan.Contains(spanOfParagraph))
                        return GetSpanOfEnclosing(activeSpan, SpanType.Paragraph);
                    return spanOfParagraph;
                case SpanType.Paragraph:
                case SpanType.MultipleParagraphs:
                    return GetSpanOfDocument(start.Snapshot);
                default:
                    return GetSpanOfDocument(start.Snapshot);
            }
        }

        private SnapshotSpan GetSpanOfFirstChild(SnapshotSpan activeSpan, SpanType spanType)
        {
            var start = activeSpan.Start;
            switch (spanType)
            {
                case SpanType.Empty:
                case SpanType.MultipleCharacters:
                case SpanType.MultipleWords:
                case SpanType.MultipleSentences:
                case SpanType.MultipleParagraphs:
                    return GetSpanOfEnclosing(activeSpan, spanType);
                case SpanType.Word:
                    return activeSpan;
                case SpanType.Sentence:
                    var extentOfWord = GetExtentOfWord(start);
                    if (!extentOfWord.IsSignificant)
                        return GetSpanOfNextSibling(extentOfWord.Span);
                    return extentOfWord.Span;
                case SpanType.Paragraph:
                    return GetSpanOfSentence(start);
                case SpanType.Document:
                    return GetSpanOfParagraph(start);
                default:
                    return activeSpan;
            }
        }

        private SnapshotSpan GetSpanOfNextSibling(SnapshotSpan activeSpan, SpanType spanType)
        {
            var currentPosition = new SnapshotPoint(activeSpan.Snapshot, activeSpan.Start);
            switch (spanType)
            {
                case SpanType.Empty:
                case SpanType.MultipleCharacters:
                    return GetSpanOfEnclosing(activeSpan, spanType);
                case SpanType.Word:
                case SpanType.MultipleWords:
                    var spanOfSentence = (Span)GetSpanOfSentence(currentPosition);
                    if (activeSpan.End == spanOfSentence.End)
                        return GetSpanOfEnclosing(activeSpan, spanType);
                    var extentOfWord = GetExtentOfWord(activeSpan.End);
                    if (!extentOfWord.IsSignificant)
                        return GetSpanOfNextSibling(extentOfWord.Span);
                    return extentOfWord.Span;
                case SpanType.Sentence:
                case SpanType.MultipleSentences:
                    var spanOfParagraph = (Span)GetSpanOfParagraph(currentPosition);
                    if (activeSpan == spanOfParagraph)
                        return GetSpanOfNextSibling(activeSpan, SpanType.Paragraph);
                    if (activeSpan.End == spanOfParagraph.End)
                        return GetSpanOfEnclosing(activeSpan, spanType);
                    return GetSpanOfSentence(activeSpan.End + 1);
                case SpanType.Paragraph:
                case SpanType.MultipleParagraphs:
                    var spanOfDocument = (Span)GetSpanOfDocument(currentPosition.Snapshot);
                    if (activeSpan.End == spanOfDocument.End)
                        return GetSpanOfEnclosing(activeSpan, spanType);
                    return GetSpanOfParagraph(activeSpan.End + 2);
                case SpanType.Document:
                    return activeSpan;
                default:
                    return activeSpan;
            }
        }

        private SnapshotSpan GetSpanOfPreviousSibling(SnapshotSpan activeSpan, SpanType spanType)
        {
            var currentPosition = new SnapshotPoint(activeSpan.Snapshot, activeSpan.Start);
            switch (spanType)
            {
                case SpanType.Empty:
                case SpanType.MultipleCharacters:
                    return GetSpanOfEnclosing(activeSpan, spanType);
                case SpanType.Word:
                case SpanType.MultipleWords:
                    var spanOfSentence = GetSpanOfSentence(currentPosition);
                    if (activeSpan == spanOfSentence)
                        return GetSpanOfPreviousSibling(activeSpan, SpanType.Sentence);
                    if (activeSpan.Start <= spanOfSentence.Start)
                        return GetSpanOfEnclosing(activeSpan, spanType);
                    var extentOfWord = GetExtentOfWord(activeSpan.Start - 1);
                    if (!extentOfWord.IsSignificant)
                        return GetSpanOfPreviousSibling(extentOfWord.Span);
                    return extentOfWord.Span;
                case SpanType.Sentence:
                case SpanType.MultipleSentences:
                    var spanOfParagraph = GetSpanOfParagraph(currentPosition);
                    if (activeSpan == spanOfParagraph)
                        return GetSpanOfPreviousSibling(activeSpan, SpanType.Paragraph);
                    if (activeSpan.Start == spanOfParagraph.Start)
                        return GetSpanOfEnclosing(activeSpan, spanType);
                    return GetSpanOfSentence(activeSpan.Start - 1);
                case SpanType.Paragraph:
                case SpanType.MultipleParagraphs:
                    var spanOfDocument = GetSpanOfDocument(currentPosition.Snapshot);
                    if (activeSpan.Start == spanOfDocument.Start)
                        return GetSpanOfEnclosing(activeSpan, spanType);
                    return GetSpanOfParagraph(activeSpan.Start - 3);
                case SpanType.Document:
                    return activeSpan;
                default:
                    return activeSpan;
            }
        }

        private SnapshotSpan GetSpanOfWord(SnapshotPoint currentPosition)
        {
            return GetExtentOfWord(currentPosition).Span;
        }

        private static SnapshotSpan GetSpanOfSentence(SnapshotPoint currentPosition)
        {
            return new SnapshotSpan(FindStartOfSentence(currentPosition), FindEndOfSentence(currentPosition));
        }

        private static SnapshotPoint FindStartOfSentence(SnapshotPoint currentPosition)
        {
            var lineNumber = currentPosition.GetContainingLine().LineNumber;
            while (lineNumber >= 0)
            {
                var lineFromLineNumber = currentPosition.Snapshot.GetLineFromLineNumber(lineNumber--);
                if (lineFromLineNumber.Length == 0)
                    return lineFromLineNumber.EndIncludingLineBreak;
                var pseudoString = new PseudoString(lineFromLineNumber);
                for (var index = (lineFromLineNumber.EndIncludingLineBreak - 1) < currentPosition.Position ? lineFromLineNumber.EndIncludingLineBreak - 1 : currentPosition.Position - 1; index >= lineFromLineNumber.Start; --index)
                {
                    if (".!?".IndexOf(pseudoString[index - lineFromLineNumber.Start]) != -1)
                        return new SnapshotPoint(currentPosition.Snapshot, index + 1);
                }
                if (lineFromLineNumber.Start == 0)
                    break;
            }
            return new SnapshotPoint(currentPosition.Snapshot, 0);
        }

        private static SnapshotPoint FindEndOfSentence(SnapshotPoint currentPosition)
        {
            var lineNumber = currentPosition.GetContainingLine().LineNumber;
            while (lineNumber != currentPosition.Snapshot.LineCount)
            {
                var lineFromLineNumber = currentPosition.Snapshot.GetLineFromLineNumber(lineNumber++);
                if (lineFromLineNumber.Length == 0 && currentPosition.GetContainingLine().LineNumber != lineNumber - 1)
                    return lineFromLineNumber.Start;
                var pseudoString = new PseudoString(lineFromLineNumber);
                for (var index = (int)lineFromLineNumber.Start <= currentPosition.Position ? currentPosition.Position : lineFromLineNumber.Start; index < lineFromLineNumber.EndIncludingLineBreak; ++index)
                {
                    if (".!?".IndexOf(pseudoString[index - lineFromLineNumber.Start]) != -1)
                        return new SnapshotPoint(currentPosition.Snapshot, index + 1);
                }
                if (lineFromLineNumber.EndIncludingLineBreak == currentPosition.Snapshot.Length)
                    break;
            }
            return new SnapshotPoint(currentPosition.Snapshot, currentPosition.Snapshot.Length);
        }

        private SnapshotSpan GetSpanOfParagraph(SnapshotPoint currentPosition)
        {
            return new SnapshotSpan(FindStartOfParagraph(currentPosition), FindEndOfParagraph(currentPosition));
        }

        private static SnapshotPoint FindStartOfParagraph(SnapshotPoint currentPosition)
        {
            var lineNumber = currentPosition.GetContainingLine().LineNumber;
            while (lineNumber >= 0)
            {
                var lineFromLineNumber = currentPosition.Snapshot.GetLineFromLineNumber(lineNumber--);
                if (lineFromLineNumber.Length == 0 && currentPosition.GetContainingLine().LineNumber != lineNumber + 1)
                    return lineFromLineNumber.EndIncludingLineBreak;
                if (lineFromLineNumber.Start == 0)
                    break;
            }
            return new SnapshotPoint(currentPosition.Snapshot, 0);
        }

        private static SnapshotPoint FindEndOfParagraph(SnapshotPoint currentPosition)
        {
            var lineNumber = currentPosition.GetContainingLine().LineNumber;
            while (lineNumber != currentPosition.Snapshot.LineCount)
            {
                var lineFromLineNumber = currentPosition.Snapshot.GetLineFromLineNumber(lineNumber++);
                if (lineFromLineNumber.Length == 0 && currentPosition.GetContainingLine().LineNumber != lineNumber - 1)
                    return lineFromLineNumber.Start;
                if (lineFromLineNumber.EndIncludingLineBreak == currentPosition.Snapshot.Length)
                    break;
            }
            return new SnapshotPoint(currentPosition.Snapshot, currentPosition.Snapshot.Length);
        }

        private static SnapshotSpan GetSpanOfDocument(ITextSnapshot snapshot)
        {
            return new SnapshotSpan(snapshot, 0, snapshot.Length);
        }

        private enum CharacterType
        {
            AlphaNumeric,
            WhiteSpace,
            Symbols,
        }

        private enum SpanType
        {
            Empty,
            MultipleCharacters,
            Word,
            MultipleWords,
            Sentence,
            MultipleSentences,
            Paragraph,
            MultipleParagraphs,
            Document,
        }

        internal class PseudoString
        {
            internal readonly SnapshotSpan Span;
            internal readonly string Text;

            public PseudoString(ITextSnapshotLine line)
                : this(line.ExtentIncludingLineBreak)
            {
            }

            public PseudoString(SnapshotSpan span)
            {
                Span = span;
                if (span.Length >= 200)
                    return;
                Text = Span.GetText();
            }

            public char this[int index]
            {
                get
                {
                    if (Text == null)
                        return Span.Snapshot[index + Span.Start];
                    return Text[index];
                }
            }

            public int Length => Span.Length;
        }
    }
}