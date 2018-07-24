using System;
using System.IO;

namespace ModernApplicationFramework.TextEditor.Text
{
    internal class StringRebuilderForChars : UnaryStringRebuilder
    {
        internal readonly char[] Content;

        internal static StringRebuilder Create(char[] source, int length, ILineBreaks lineBreaks)
        {
            return Create(source, lineBreaks, 0, length, 0, lineBreaks.Length);
        }

        internal static StringRebuilder Create(char[] source, ILineBreaks lineBreaks, int start, int length, int lineBreaksStart, int lineBreaksLength)
        {
            if (lineBreaksLength == 0)
                return new StringRebuilderForChars(source, LineBreakManager.Empty, start, length, 0, 0, source[start], source[start + length - 1]);
            return new StringRebuilderForChars(source, lineBreaks, start, length, lineBreaksStart, lineBreaksLength, source[start], source[start + length - 1]);
        }

        private StringRebuilderForChars(char[] source, ILineBreaks lineBreaks, int start, int length, int lineBreaksStart, int lineBreaksLength, char first, char last)
            : base(lineBreaks, start, length, lineBreaksStart, lineBreaksLength, first, last)
        {
            Content = source;
        }

        public override string ToString()
        {
            return new string(Content, TextSpanStart, Length);
        }

        public override char this[int index] => GetChar(Content, index);

        public override string GetText(Span span)
        {
            return GetText(Content, span);
        }

        public override void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
        {
            CopyTo(Content, sourceIndex, destination, destinationIndex, count);
        }

        public override void Write(TextWriter writer, Span span)
        {
            Write(Content, writer, span);
        }

        public override StringRebuilder GetSubText(Span span)
        {
            if (span.End > Length)
                throw new ArgumentOutOfRangeException(nameof(span));
            if (span.Length == 0)
                return Empty;
            if (span.Length == Length)
                return this;
            FindFirstAndLastLines(span, out var firstLineNumber, out var lastLineNumber);
            return Create(Content, LineBreaks, span.Start + TextSpanStart, span.Length, firstLineNumber, lastLineNumber - firstLineNumber);
        }
    }
}