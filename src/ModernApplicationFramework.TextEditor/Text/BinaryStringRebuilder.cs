using System;
using System.Globalization;
using System.IO;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.TextEditor.Text
{
    internal sealed class BinaryStringRebuilder : StringRebuilder
    {
        private readonly StringRebuilder _left;
        private readonly StringRebuilder _right;

        private static readonly StringRebuilder Crlf = Create("\r\n");

        public override int Depth { get; }

        internal BinaryStringRebuilder(StringRebuilder left, StringRebuilder right) :
            base(left.Length + right.Length, left.LineBreakCount + right.LineBreakCount, left.FirstCharacter, right.LastCharacter)
        {
            _left = left;
            _right = right;
            Depth = 1 + Math.Max(left.Depth, right.Depth);
        }

        public static StringRebuilder Create(StringRebuilder left, StringRebuilder right)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));
            if (right == null)
                throw new ArgumentNullException(nameof(right));
            if (left.Length == 0)
                return right;
            if (right.Length == 0)
                return left;
            if (left.Length + right.Length < TextModelOptions.StringRebuilderMaxCharactersToConsolidate && left.LineBreakCount + right.LineBreakCount <= TextModelOptions.StringRebuilderMaxLinesToConsolidate)
                return Consolidate(left, right);
            if (right.FirstCharacter == '\n' && left.LastCharacter == '\r')
                return Create(Create(left.GetSubText(new Span(0, left.Length - 1)), Crlf), right.GetSubText(Span.FromBounds(1, right.Length)));
            return BalanceStringRebuilder(left, right);
        }

        public override int GetLineNumberFromPosition(int position)
        {
            if (position < 0 || position > Length)
                throw new ArgumentOutOfRangeException(nameof(position));
            if (position > _left.Length)
                return _left.LineBreakCount + _right.GetLineNumberFromPosition(position - _left.Length);
            return _left.GetLineNumberFromPosition(position);
        }

        public override void GetLineFromLineNumber(int lineNumber, out Span extent, out int lineBreakLength)
        {
            if (lineNumber < 0 || lineNumber > LineBreakCount)
                throw new ArgumentOutOfRangeException(nameof(lineNumber));
            if (lineNumber < _left.LineBreakCount)
                _left.GetLineFromLineNumber(lineNumber, out extent, out lineBreakLength);
            else if (lineNumber > _left.LineBreakCount)
            {
                _right.GetLineFromLineNumber(lineNumber - _left.LineBreakCount, out extent, out lineBreakLength);
                extent = new Span(extent.Start + _left.Length, extent.Length);
            }
            else
            {
                var start = 0;
                if (lineNumber != 0)
                {
                    _left.GetLineFromLineNumber(lineNumber, out extent, out lineBreakLength);
                    start = extent.Start;
                }
                int end;
                if (lineNumber == LineBreakCount)
                {
                    end = Length;
                    lineBreakLength = 0;
                }
                else
                {
                    _right.GetLineFromLineNumber(0, out extent, out lineBreakLength);
                    end = extent.End + _left.Length;
                }
                extent = Span.FromBounds(start, end);
            }
        }

        public override StringRebuilder GetLeaf(int position, out int offset)
        {
            if (position < _left.Length)
                return _left.GetLeaf(position, out offset);
            var leaf = _right.GetLeaf(position - _left.Length, out offset);
            offset += _left.Length;
            return leaf;
        }

        public override char this[int index]
        {
            get
            {
                if (index < 0 || index >= Length)
                    throw new ArgumentOutOfRangeException(nameof(index));
                if (index >= _left.Length)
                    return _right[index - _left.Length];
                return _left[index];
            }
        }

        public override void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
        {
            if (sourceIndex >= _left.Length)
                _right.CopyTo(sourceIndex - _left.Length, destination, destinationIndex, count);
            else if (sourceIndex + count <= _left.Length)
            {
                _left.CopyTo(sourceIndex, destination, destinationIndex, count);
            }
            else
            {
                var count1 = _left.Length - sourceIndex;
                _left.CopyTo(sourceIndex, destination, destinationIndex, count1);
                _right.CopyTo(0, destination, destinationIndex + count1, count - count1);
            }
        }

        public override void Write(TextWriter writer, Span span)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));
            if (span.End > Length)
                throw new ArgumentOutOfRangeException(nameof(span));
            if (span.Start >= _left.Length)
                _right.Write(writer, new Span(span.Start - _left.Length, span.Length));
            else if (span.End <= _left.Length)
            {
                _left.Write(writer, span);
            }
            else
            {
                _left.Write(writer, Span.FromBounds(span.Start, _left.Length));
                _right.Write(writer, Span.FromBounds(0, span.End - _left.Length));
            }
        }

        public override StringRebuilder GetSubText(Span span)
        {
            if (span.End > Length)
                throw new ArgumentOutOfRangeException(nameof(span));
            if (span.Length == Length)
                return this;
            if (span.End <= _left.Length)
                return _left.GetSubText(span);
            if (span.Start >= _left.Length)
                return _right.GetSubText(new Span(span.Start - _left.Length, span.Length));
            return Create(_left.GetSubText(Span.FromBounds(span.Start, _left.Length)), _right.GetSubText(Span.FromBounds(0, span.End - _left.Length)));
        }

        public override string GetText(Span span)
        {
            if (span.End > Length)
                throw new ArgumentOutOfRangeException(nameof(span));
            if (span.End <= _left.Length)
                return _left.GetText(span);
            if (span.Start >= _left.Length)
                return _right.GetText(new Span(span.Start - _left.Length, span.Length));
            var destination = new char[span.Length];
            var num = _left.Length - span.Start;
            _left.CopyTo(span.Start, destination, 0, num);
            _right.CopyTo(0, destination, num, span.Length - num);
            return new string(destination);
        }

        public override StringRebuilder Child(bool rightSide)
        {
            if (!rightSide)
                return _left;
            return _right;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, Depth % 2 == 0 ? "({0})({1})" : "[{0}][{1}]",
                _left.ToString(), _right.ToString());
        }

        private static StringRebuilder BalanceStringRebuilder(StringRebuilder left, StringRebuilder right)
        {
            return BalanceTreeNode(left, right);
        }

        private static StringRebuilder BalanceTreeNode(StringRebuilder left, StringRebuilder right)
        {
            if (left.Depth > right.Depth + 1)
                return Pivot(left, right, false);
            if (right.Depth > left.Depth + 1)
                return Pivot(right, left, true);
            return new BinaryStringRebuilder(left, right);
        }

        private static StringRebuilder Pivot(StringRebuilder child, StringRebuilder other, bool deepOnRightSide)
        {
            var stringRebuilder1 = child.Child(deepOnRightSide);
            var stringRebuilder2 = child.Child(!deepOnRightSide);
            if (stringRebuilder1.Depth >= stringRebuilder2.Depth)
            {
                StringRebuilder stringRebuilder3;
                if (deepOnRightSide)
                {
                    stringRebuilder3 = ConsolidateOrBalanceTreeNode(ConsolidateOrBalanceTreeNode(other, stringRebuilder2), stringRebuilder1);
                }
                else
                {
                    var right = ConsolidateOrBalanceTreeNode(stringRebuilder2, other);
                    stringRebuilder3 = ConsolidateOrBalanceTreeNode(stringRebuilder1, right);
                }
                return stringRebuilder3;
            }
            var stringRebuilder4 = stringRebuilder2.Child(deepOnRightSide);
            var stringRebuilder5 = stringRebuilder2.Child(!deepOnRightSide);
            StringRebuilder stringRebuilder6;
            if (deepOnRightSide)
            {
                stringRebuilder6 = ConsolidateOrBalanceTreeNode(ConsolidateOrBalanceTreeNode(other, stringRebuilder5), ConsolidateOrBalanceTreeNode(stringRebuilder4, stringRebuilder1));
            }
            else
            {
                var right = ConsolidateOrBalanceTreeNode(stringRebuilder5, other);
                stringRebuilder6 = ConsolidateOrBalanceTreeNode(ConsolidateOrBalanceTreeNode(stringRebuilder1, stringRebuilder4), right);
            }
            return stringRebuilder6;
        }

        private static StringRebuilder ConsolidateOrBalanceTreeNode(StringRebuilder left, StringRebuilder right)
        {
            if (left.Length + right.Length < TextModelOptions.StringRebuilderMaxCharactersToConsolidate && left.LineBreakCount + right.LineBreakCount <= TextModelOptions.StringRebuilderMaxLinesToConsolidate)
                return Consolidate(left, right);
            return BalanceTreeNode(left, right);
        }

    }
}