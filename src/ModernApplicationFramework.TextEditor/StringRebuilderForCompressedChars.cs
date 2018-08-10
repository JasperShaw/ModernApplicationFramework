using System;
using System.IO;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.TextEditor.Text;

namespace ModernApplicationFramework.TextEditor
{
    internal class StringRebuilderForCompressedChars : UnaryStringRebuilder
    {
        private readonly Page _content;

        internal static StringRebuilder Create(Page content, ILineBreaks lineBreaks)
        {
            return Create(content, lineBreaks, 0, content.Length, 0, lineBreaks.Length);
        }

        private static StringRebuilder Create(Page content, ILineBreaks lineBreaks, int start, int length, int lineBreaksStart, int linebreaksLength)
        {
            char[] chArray = content.Expand();
            return new StringRebuilderForCompressedChars(content, lineBreaks, start, length, lineBreaksStart, linebreaksLength, chArray[start], chArray[start + length - 1]);
        }

        private StringRebuilderForCompressedChars(Page content, ILineBreaks lineBreaks, int start, int length, int lineBreaksStart, int linebreaksLength, char first, char last)
            : base(lineBreaks, start, length, lineBreaksStart, linebreaksLength, first, last)
        {
            _content = content;
        }

        public override char this[int index] => GetChar(_content.Expand(), index);

        public override string GetText(Span span)
        {
            return GetText(_content.Expand(), span);
        }

        public override void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
        {
            CopyTo(_content.Expand(), sourceIndex, destination, destinationIndex, count);
        }

        public override void Write(TextWriter writer, Span span)
        {
            Write(_content.Expand(), writer, span);
        }

        public override StringRebuilder GetSubText(Span span)
        {
            if (span.End > Length)
                throw new ArgumentOutOfRangeException(nameof(span));
            if (span.Length == Length)
                return this;
            if (span.Length == 0)
                return Empty;
            FindFirstAndLastLines(span, out var firstLineNumber, out var lastLineNumber);
            return Create(_content, LineBreaks, span.Start + TextSpanStart, span.Length, firstLineNumber, lastLineNumber - firstLineNumber);
        }
    }
}