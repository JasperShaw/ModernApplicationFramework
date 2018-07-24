using System;
using System.Collections;
using System.Collections.Generic;

namespace ModernApplicationFramework.TextEditor.Text.Differencing
{
    public class Match : IEnumerable<Tuple<int, int>>
    {
        public Match(Span left, Span right)
        {
            if (left.Length != right.Length)
                throw new ArgumentException("Spans must be of equal length");
            Left = left;
            Right = right;
        }

        public Span Left { get; }

        public Span Right { get; }

        public int Length => Left.Length;

        public override bool Equals(object obj)
        {
            if (obj is Match match && Left.Equals(match.Left))
                return Right.Equals(match.Right);
            return false;
        }

        public override int GetHashCode()
        {
            return Left.GetHashCode() << 16 ^ Right.GetHashCode();
        }

        public IEnumerator<Tuple<int, int>> GetEnumerator()
        {
            for (int i = 0; i < Length; ++i)
                yield return new Tuple<int, int>(Left.Start + i, Right.Start + i);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
