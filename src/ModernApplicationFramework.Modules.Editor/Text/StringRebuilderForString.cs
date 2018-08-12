using System;
using System.IO;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Modules.Editor.Text
{
    internal class StringRebuilderForString : UnaryStringRebuilder
    {
        private readonly string _content;

        internal static StringRebuilder Create(string source, int length, ILineBreaks lineBreaks)
        {
            return Create(source, lineBreaks, 0, length, 0, lineBreaks.Length);
        }

        internal static StringRebuilder Create(string source, ILineBreaks lineBreaks, int start, int length,
            int lineBreaksStart, int lineBreaksLength)
        {
            if (lineBreaksLength == 0)
                return new StringRebuilderForString(source, LineBreakManager.Empty, start, length, 0,0, source[start], source[start + length - 1]);
            return new StringRebuilderForString(source, lineBreaks, start, length, lineBreaksStart,
                lineBreaksLength, source[start], source[start + length - 1]);
        }

        internal StringRebuilderForString() : base(LineBreakManager.Empty, 0,0,0,0, char.MinValue, char.MinValue)
        {
            _content = string.Empty;
        }

        private StringRebuilderForString(string source, ILineBreaks lineBreaks, int start, int lenght, int lineBreaksStart, 
            int lineBreaksLength, char first, char last) : 
            base(lineBreaks, start, lenght, lineBreaksStart, lineBreaksLength, first, last)
        {
            _content = source;
        }

        public override string ToString()
        {
            return _content.Substring(TextSpanStart, Length);
        }

        public override char this[int index]
        {
            get
            {
                if (index < 0 || index >= Length)
                    throw new ArgumentOutOfRangeException(nameof(index));
                return _content[index + TextSpanStart];
            }
        }

        public override string GetText(Span span)
        {
            return _content.Substring(span.Start + TextSpanStart, span.Length);
        }

        public override void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
        {
            _content.CopyTo(sourceIndex + TextSpanStart, destination, destinationIndex, count);
        }

        public override void Write(TextWriter writer, Span span)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));
            writer.Write(GetText(span));
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
            return Create(_content, LineBreaks, span.Start + TextSpanStart, span.Length, firstLineNumber, lastLineNumber - firstLineNumber);
        }
    }
}