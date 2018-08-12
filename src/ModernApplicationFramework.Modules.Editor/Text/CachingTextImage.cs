using System;
using System.IO;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Modules.Editor.Text
{
    internal class CachingTextImage : ITextImage
    {
        public readonly StringRebuilder Builder;
        private Tuple<int, StringRebuilder> _cache;

        public int Length => Builder.Length;

        public int LineCount => Builder.LineBreakCount + 1;

        public ITextImageVersion Version { get; }

        private CachingTextImage(StringRebuilder builder, ITextImageVersion version)
        {
            Builder = builder ?? throw new ArgumentNullException(nameof(builder));
            Version = version;
        }

        public char this[int position]
        {
            get
            {
                var tuple = UpdateCache(position);
                return tuple.Item2[position - tuple.Item1];
            }
        }

        public static ITextImage Create(StringRebuilder builder, ITextImageVersion version)
        {
            return new CachingTextImage(builder, version);
        }

        public void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
        {
            Builder.CopyTo(sourceIndex, destination, destinationIndex, count);
        }

        public TextImageLine GetLineFromLineNumber(int lineNumber)
        {
            Builder.GetLineFromLineNumber(lineNumber, out var extent, out var lineBreakLength);
            return new TextImageLine(this, lineNumber, extent, lineBreakLength);
        }

        public TextImageLine GetLineFromPosition(int position)
        {
            return GetLineFromLineNumber(Builder.GetLineNumberFromPosition(position));
        }

        public int GetLineNumberFromPosition(int position)
        {
            return Builder.GetLineNumberFromPosition(position);
        }

        public ITextImage GetSubText(Span span)
        {
            return Create(Builder.GetSubText(span), null);
        }

        public string GetText(Span span)
        {
            var tuple = UpdateCache(span.Start);
            var start = span.Start - tuple.Item1;
            if (start + span.Length < tuple.Item2.Length)
                return tuple.Item2.GetText(new Span(start, span.Length));
            return Builder.GetText(span);
        }

        public char[] ToCharArray(int startIndex, int length)
        {
            return Builder.ToCharArray(startIndex, length);
        }

        public void Write(TextWriter writer, Span span)
        {
            Builder.Write(writer, span);
        }

        private Tuple<int, StringRebuilder> UpdateCache(int position)
        {
            var tuple = _cache;
            if (tuple == null || position < tuple.Item1 || position >= tuple.Item1 + tuple.Item2.Length)
            {
                var leaf = Builder.GetLeaf(position, out var offset);
                tuple = new Tuple<int, StringRebuilder>(offset, leaf);
                _cache = tuple;
            }

            return tuple;
        }
    }
}