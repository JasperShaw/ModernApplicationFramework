using System;
using System.IO;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.TextEditor.Text
{
    internal abstract class UnaryStringRebuilder : StringRebuilder
    {
        internal readonly ILineBreaks LineBreaks;
        protected readonly int TextSpanStart;
        protected readonly int LineBreakSpanStart;

        protected int TextSpanEnd => TextSpanStart + Length;

        protected int LineBreakSpanEnd => LineBreakSpanStart + LineBreakCount;

        protected UnaryStringRebuilder(ILineBreaks lineBreaks, int start, int length, int linebreaksStart, int linebreaksLength, char first, char last) :
            base(length, linebreaksLength, first, last)
        {
            LineBreaks = lineBreaks;
            TextSpanStart = start;
            LineBreakSpanStart = linebreaksStart;
        }

        public override int GetLineNumberFromPosition(int position)
        {
            if (position < 0 || position > Length)
                throw new ArgumentOutOfRangeException(nameof(position));
            if (position == Length)
                return LineBreakCount;
            position += TextSpanStart;
            int num1 = LineBreakSpanStart;
            int num2 = LineBreakSpanEnd;
            while (num1 < num2)
            {
                int index = (num1 + num2) / 2;
                if (position < LineBreaks.EndOfLineBreak(index))
                    num2 = index;
                else
                    num1 = index + 1;
            }
            return num1 - LineBreakSpanStart;
        }

        public override void GetLineFromLineNumber(int lineNumber, out Span extent, out int lineBreakLength)
        {
            if (lineNumber < 0 || lineNumber > LineBreakCount)
                throw new ArgumentOutOfRangeException(nameof(lineNumber));
            int index = LineBreakSpanStart + lineNumber;
            int start = lineNumber == 0 ? 0 : Math.Min(TextSpanEnd, LineBreaks.EndOfLineBreak(index - 1)) - TextSpanStart;
            int end;
            if (lineNumber < LineBreakCount)
            {
                int num = Math.Max(TextSpanStart, LineBreaks.StartOfLineBreak(index));
                lineBreakLength = Math.Min(TextSpanEnd, LineBreaks.EndOfLineBreak(index)) - num;
                end = num - TextSpanStart;
            }
            else
            {
                end = Length;
                lineBreakLength = 0;
            }
            extent = Span.FromBounds(start, end);
        }

        public override StringRebuilder GetLeaf(int position, out int offset)
        {
            offset = 0;
            return this;
        }

        public override StringRebuilder Child(bool rightSide)
        {
            throw new InvalidOperationException();
        }

        internal void FindFirstAndLastLines(Span span, out int firstLineNumber, out int lastLineNumber)
        {
            firstLineNumber = GetLineNumberFromPosition(span.Start) + LineBreakSpanStart;
            lastLineNumber = GetLineNumberFromPosition(span.End) + LineBreakSpanStart;
            if (lastLineNumber >= LineBreakSpanEnd || span.End <= LineBreaks.StartOfLineBreak(lastLineNumber) - TextSpanStart)
                return;
            ++lastLineNumber;
        }

        protected char GetChar(char[] content, int index)
        {
            if (index < 0 || index >= Length)
                throw new ArgumentOutOfRangeException(nameof(index));
            return content[index + TextSpanStart];
        }

        protected string GetText(char[] content, Span span)
        {
            if (span.End > Length)
                throw new ArgumentOutOfRangeException(nameof(span));
            return new string(content, span.Start + TextSpanStart, span.Length);
        }

        protected void CopyTo(char[] content, int sourceIndex, char[] destination, int destinationIndex, int count)
        {
            if (sourceIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(sourceIndex));
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));
            if (destinationIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(destinationIndex));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (sourceIndex + count > Length || sourceIndex + count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (destinationIndex + count > destination.Length || destinationIndex + count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            Array.Copy(content, sourceIndex + TextSpanStart, destination, destinationIndex, count);
        }

        protected void Write(char[] content, TextWriter writer, Span span)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));
            if (span.End > Length)
                throw new ArgumentOutOfRangeException(nameof(span));
            writer.Write(content, span.Start + TextSpanStart, span.Length);
        }


    }
}