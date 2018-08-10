using System;
using System.IO;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.TextEditor.Text
{
    internal abstract class StringRebuilder
    {
        public static readonly StringRebuilder Empty = new StringRebuilderForString();
        public readonly int Length;
        public int LineBreakCount;
        public readonly char FirstCharacter;
        public readonly char LastCharacter;

        public virtual int Depth => 0;

        protected StringRebuilder(int length, int lineBreakCount, char first, char last)
        {
            Length = length;
            LineBreakCount = lineBreakCount;
            FirstCharacter = first;
            LastCharacter = last;
        }

        public static StringRebuilder Create(string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            if (text.Length != 0)
                return StringRebuilderForString.Create(text, text.Length, LineBreakManager.CreateLineBreaks(text));
            return Empty;
        }

        public static StringRebuilder Create(ITextImage image)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            if (image is CachingTextImage cachingTextImage)
                return cachingTextImage.Builder;
            return Create(image.GetText(0, image.Length));
        }

        public static StringRebuilder Consolidate(StringRebuilder left, StringRebuilder right)
        {
            var length = left.Length + right.Length;
            var chArray = new char[length];
            left.CopyTo(0, chArray, 0, left.Length);
            right.CopyTo(0, chArray, left.Length, right.Length);
            ILineBreaks lineBreaks;
            if (left.LineBreakCount == 0 && right.LineBreakCount == 0)
            {
                lineBreaks = LineBreakManager.Empty;
            }
            else
            {
                var lineBreakEditor = LineBreakManager.CreateLineBreakEditor(length, left.LineBreakCount + right.LineBreakCount);
                var num1 = 0;
                if (chArray[left.Length] == '\n' && chArray[left.Length - 1] == '\r')
                    num1 = 1;
                var num2 = left.LineBreakCount - num1;
                for (var lineNumber = 0; lineNumber < num2; ++lineNumber)
                {
                    left.GetLineFromLineNumber(lineNumber, out var extent, out var lineBreakLength);
                    lineBreakEditor.Add(extent.End, lineBreakLength);
                }
                if (num1 == 1)
                    lineBreakEditor.Add(left.Length - 1, 2);
                for (var lineNumber = num1; lineNumber < right.LineBreakCount; ++lineNumber)
                {
                    right.GetLineFromLineNumber(lineNumber, out var extent, out var lineBreakLength);
                    lineBreakEditor.Add(extent.End + left.Length, lineBreakLength);
                }
                lineBreaks = lineBreakEditor;
            }
            return StringRebuilderForChars.Create(chArray, length, lineBreaks);
        }

        public abstract int GetLineNumberFromPosition(int position);

        public abstract void GetLineFromLineNumber(int lineNumber, out Span extent, out int lineBreakLength);

        public abstract StringRebuilder GetLeaf(int position, out int offset);

        public abstract char this[int index] { get; }

        public abstract void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count);

        public abstract void Write(TextWriter writer, Span span);

        public abstract StringRebuilder GetSubText(Span span);

        public abstract string GetText(Span span);

        public abstract StringRebuilder Child(bool rightSide);

        public char[] ToCharArray(int startIndex, int lenght)
        {
            if (startIndex < 0 )
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (lenght < 0 || startIndex + lenght > Length || startIndex + lenght < 0)
                throw new ArgumentOutOfRangeException(nameof(lenght));
            var destination = new char[lenght];
            CopyTo(startIndex, destination, 0, lenght);
            return destination;
        }

        public StringRebuilder Append(string text)
        {
            return Insert(Length, text);
        }

        public StringRebuilder Append(StringRebuilder text)
        {
            return Insert(Length, text);
        }

        public StringRebuilder Insert(int position, string text)
        {
            return Insert(position, Create(text));
        }

        public StringRebuilder Insert(int position, StringRebuilder text)
        {
            if (position < 0 || position > Length)
                throw new ArgumentOutOfRangeException(nameof(position));
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            return Assemble(Span.FromBounds(0, position), text, Span.FromBounds(position, Length));
        }

        public StringRebuilder Delete(Span span)
        {
            if (span.End > Length)
                throw new ArgumentOutOfRangeException(nameof(span));
            return Assemble(Span.FromBounds(0, span.Start), Span.FromBounds(span.End, Length));
        }

        public StringRebuilder Replace(Span span, string text)
        {
            return Replace(span, Create(text));
        }

        public StringRebuilder Replace(Span span, StringRebuilder text)
        {
            if (span.End > Length)
                throw new ArgumentOutOfRangeException(nameof(span));
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            return Assemble(Span.FromBounds(0, span.Start), text, Span.FromBounds(span.End, Length));
        }

        private StringRebuilder Assemble(Span left, Span right)
        {
            if (left.Length == 0)
                return GetSubText(right);
            if (right.Length == 0)
                return GetSubText(left);
            if (left.Length + right.Length == Length)
                return this;
            return BinaryStringRebuilder.Create(GetSubText(left), GetSubText(right));
        }

        private StringRebuilder Assemble(Span left, StringRebuilder text, Span right)
        {
            if (text.Length == 0)
                return Assemble(left, right);
            if (left.Length == 0)
            {
                return right.Length != 0 ? BinaryStringRebuilder.Create(text, GetSubText(right)) : text;
            }

            if (right.Length == 0)
                return BinaryStringRebuilder.Create(GetSubText(left), text);
            if (left.Length < right.Length)
                return BinaryStringRebuilder.Create(BinaryStringRebuilder.Create(GetSubText(left), text), GetSubText(right));
            return BinaryStringRebuilder.Create(GetSubText(left), BinaryStringRebuilder.Create(text, GetSubText(right)));
        }
    }
}
