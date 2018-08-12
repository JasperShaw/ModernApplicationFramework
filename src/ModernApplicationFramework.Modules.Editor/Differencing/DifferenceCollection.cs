using System;
using System.Collections;
using System.Collections.Generic;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Differencing;

namespace ModernApplicationFramework.Modules.Editor.Differencing
{
    internal class DifferenceCollection<T> : IDifferenceCollection<T>
    {
        public IList<Difference> Differences { get; }

        public IList<T> LeftSequence { get; }

        public IEnumerable<Tuple<int, int>> MatchSequence { get; }

        public IList<T> RightSequence { get; }

        public DifferenceCollection(IList<Difference> diffs, IList<T> originalLeft, IList<T> originalRight)
        {
            LeftSequence = originalLeft;
            RightSequence = originalRight;
            Differences = diffs;
            MatchSequence = new MatchEnumerator(diffs, originalLeft.Count);
        }

        public static void AddDifference(int originalStart, int originalEnd, int nextOriginalEnd, int modifiedStart,
            int modifiedEnd, int nextModifiedEnd, IList<Difference> diffs, ref Match before)
        {
            var after = originalEnd != nextOriginalEnd
                ? new Match(Span.FromBounds(originalEnd, nextOriginalEnd),
                    Span.FromBounds(modifiedEnd, nextModifiedEnd))
                : null;
            diffs.Add(new Difference(Span.FromBounds(originalStart, originalEnd),
                Span.FromBounds(modifiedStart, modifiedEnd), before, after));
            before = after;
        }

        public static Match CreateInitialMatch(int originalStart)
        {
            if (originalStart == 0)
                return null;
            return new Match(new Span(0, originalStart), new Span(0, originalStart));
        }

        public IEnumerator<Difference> GetEnumerator()
        {
            return Differences.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal static IList<Match> MatchesFromPairs(IList<Tuple<int, int>> matches)
        {
            if (matches.Count == 0)
                return new List<Match>();
            IList<Match> matchList = new List<Match>();
            var match1 = matches[0];
            var start1 = match1.Item1;
            var end1 = start1 + 1;
            var start2 = match1.Item2;
            var end2 = start2 + 1;
            for (var index = 1; index < matches.Count; ++index)
            {
                var match2 = matches[index];
                if (match2.Item1 == end1 && match2.Item2 == end2)
                {
                    ++end1;
                    ++end2;
                }
                else
                {
                    matchList.Add(new Match(Span.FromBounds(start1, end1), Span.FromBounds(start2, end2)));
                    start1 = match2.Item1;
                    end1 = start1 + 1;
                    start2 = match2.Item2;
                    end2 = start2 + 1;
                }
            }

            matchList.Add(new Match(Span.FromBounds(start1, end1), Span.FromBounds(start2, end2)));
            return matchList;
        }
    }

    internal sealed class MatchEnumerator : IEnumerable<Tuple<int, int>>
    {
        private readonly IList<Difference> _differences;
        private readonly int _leftCount;

        public MatchEnumerator(IList<Difference> differences, int leftCount)
        {
            _differences = differences;
            _leftCount = leftCount;
        }

        public IEnumerator<Tuple<int, int>> GetEnumerator()
        {
            var leftStart = 0;
            var rightStart = 0;
            int i;
            if (_differences.Count != 0)
                foreach (var difference1 in _differences)
                {
                    var difference = difference1;
                    var m = difference.Before;
                    Span span;
                    if (m != null)
                        for (i = 0; i < m.Length; ++i)
                        {
                            span = m.Left;
                            var num1 = span.Start + i;
                            span = m.Right;
                            var num2 = span.Start + i;
                            yield return new Tuple<int, int>(num1, num2);
                        }

                    span = difference.Left;
                    leftStart = span.End;
                    span = difference.Right;
                    rightStart = span.End;
                }

            for (i = leftStart; i < _leftCount; ++i)
                yield return new Tuple<int, int>(i, i + rightStart - leftStart);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}