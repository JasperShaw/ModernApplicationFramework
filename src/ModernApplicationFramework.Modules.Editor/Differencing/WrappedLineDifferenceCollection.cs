using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Differencing;

namespace ModernApplicationFramework.Modules.Editor.Differencing
{
    internal class WrappedLineDifferenceCollection : IHierarchicalDifferenceCollection
    {
        private readonly IDifferenceCollection<string> _original;
        private readonly ITextDifferencingService _diffService;
        private readonly StringDifferenceOptions _nextLevelOptions;
        private readonly ITextSnapshot _leftSnapshot;
        private readonly ITextSnapshot _rightSnapshot;
        private readonly IHierarchicalDifferenceCollection[] _wordDifferences;

        public WrappedLineDifferenceCollection(IDifferenceCollection<string> original, IList<int> notIgnoredDifferences, ITextSnapshot leftSnapshot, ITextSnapshot rightSnapshot, ITokenizedStringList leftDecomposition, ITokenizedStringList rightDecomposition, ITextDifferencingService diffService, StringDifferenceOptions nextLevelOptions)
        {
            _original = original.Differences.Count != notIgnoredDifferences.Count ? new DifferenceCollection<string>(CreateDiffs(original, notIgnoredDifferences), original.LeftSequence, original.RightSequence) : original;
            _diffService = diffService;
            _nextLevelOptions = nextLevelOptions;
            _leftSnapshot = leftSnapshot;
            _rightSnapshot = rightSnapshot;
            _wordDifferences = new IHierarchicalDifferenceCollection[_original.Differences.Count];
            LeftDecomposition = leftDecomposition;
            RightDecomposition = rightDecomposition;
        }

        private static IList<Difference> CreateDiffs(IDifferenceCollection<string> original, IList<int> notIgnoredDifferences)
        {
            IList<Difference> differenceList = new List<Difference>(notIgnoredDifferences.Count);
            if (notIgnoredDifferences.Count != 0)
            {
                var difference1 = original.Differences[notIgnoredDifferences[0]];
                var initialMatch = DifferenceCollection<string>.CreateInitialMatch(difference1.Left.Start);
                Span span;
                for (var index = 1; index < notIgnoredDifferences.Count; ++index)
                {
                    var difference2 = original.Differences[notIgnoredDifferences[index]];
                    span = difference1.Left;
                    var start1 = span.Start;
                    span = difference1.Left;
                    var end1 = span.End;
                    span = difference2.Left;
                    var start2 = span.Start;
                    span = difference1.Right;
                    var start3 = span.Start;
                    span = difference1.Right;
                    var end2 = span.End;
                    span = difference2.Right;
                    var start4 = span.Start;
                    var diffs = differenceList;
                    ref var local = ref initialMatch;
                    DifferenceCollection<string>.AddDifference(start1, end1, start2, start3, end2, start4, diffs, ref local);
                    difference1 = difference2;
                }
                span = difference1.Left;
                var start5 = span.Start;
                span = difference1.Left;
                var end3 = span.End;
                var count1 = original.LeftSequence.Count;
                span = difference1.Right;
                var start6 = span.Start;
                span = difference1.Right;
                var end4 = span.End;
                var count2 = original.RightSequence.Count;
                var diffs1 = differenceList;
                ref var local1 = ref initialMatch;
                DifferenceCollection<string>.AddDifference(start5, end3, count1, start6, end4, count2, diffs1, ref local1);
            }
            return differenceList;
        }

        public ITokenizedStringList LeftDecomposition { get; }

        public ITokenizedStringList RightDecomposition { get; }

        public IHierarchicalDifferenceCollection GetContainedDifferences(int index)
        {
            return _wordDifferences[index];
        }

        internal IHierarchicalDifferenceCollection CalculateContainedDiff(int index)
        {
            var difference = Differences[index];
            if (difference.DifferenceType == DifferenceType.Change)
            {
                var spanInOriginal1 = LeftDecomposition.GetSpanInOriginal(difference.Left);
                var spanInOriginal2 = RightDecomposition.GetSpanInOriginal(difference.Right);
                if (spanInOriginal1.Length + spanInOriginal2.Length < 2000)
                {
                    var wordDifferences = _diffService.DiffSnapshotSpans(new SnapshotSpan(_leftSnapshot, spanInOriginal1), new SnapshotSpan(_rightSnapshot, spanInOriginal2), _nextLevelOptions);
                    if (!ShouldIgnoreWordDiffs(wordDifferences))
                    {
                        _wordDifferences[index] = wordDifferences;
                        return wordDifferences;
                    }
                }
            }
            return null;
        }

        private static bool ShouldIgnoreWordDiffs(IHierarchicalDifferenceCollection wordDifferences)
        {
            var num1 = wordDifferences.Differences.Sum(d => d.Left.Length);
            var num2 = wordDifferences.Differences.Sum(d => d.Right.Length);
            var num3 = wordDifferences.LeftDecomposition.Count / 2;
            if (num1 > num3 && num2 > wordDifferences.RightDecomposition.Count / 2)
                return true;
            var count = wordDifferences.Differences.Count;
            var num4 = Math.Min(wordDifferences.LeftSequence.Count, wordDifferences.RightSequence.Count);
            return count > 1 && count > num4 / 3 || count > 2 && count > num4 / 4;
        }

        public void ClearContainedDifferences(int index)
        {
            _wordDifferences[index] = null;
        }

        public bool HasContainedDifferences(int index)
        {
            return GetContainedDifferences(index) != null;
        }

        public IEnumerable<Tuple<int, int>> MatchSequence => _original.MatchSequence;

        public IList<string> LeftSequence => LeftDecomposition;

        public IList<string> RightSequence => RightDecomposition;

        public IList<Difference> Differences => _original.Differences;

        public IEnumerator<Difference> GetEnumerator()
        {
            return Differences.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Differences.GetEnumerator();
        }
    }
}