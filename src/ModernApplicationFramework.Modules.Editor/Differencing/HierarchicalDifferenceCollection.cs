using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Differencing;

namespace ModernApplicationFramework.Modules.Editor.Differencing
{
    internal class HierarchicalDifferenceCollection : IHierarchicalDifferenceCollection
    {
        private readonly ITokenizedStringListInternal _left;
        private readonly ITokenizedStringListInternal _right;
        private readonly IDifferenceCollection<string> _differenceCollection;
        private readonly ITextDifferencingService _differenceService;
        private readonly StringDifferenceOptions _options;
        private readonly ConcurrentDictionary<int, IHierarchicalDifferenceCollection> _containedDifferences;

        public HierarchicalDifferenceCollection(IDifferenceCollection<string> differenceCollection, ITokenizedStringListInternal left, ITokenizedStringListInternal right, ITextDifferencingService differenceService, StringDifferenceOptions options)
        {
            if (differenceCollection == null)
                throw new ArgumentNullException(nameof(differenceCollection));
            if (left == null)
                throw new ArgumentNullException(nameof(left));
            if (right == null)
                throw new ArgumentNullException(nameof(right));
            if (!Equals(left, differenceCollection.LeftSequence))
                throw new ArgumentException("left must equal differenceCollection.LeftSequence");
            if (!Equals(right, differenceCollection.RightSequence))
                throw new ArgumentException("right must equal differenceCollection.RightSequence");
            _left = left;
            _right = right;
            _differenceCollection = differenceCollection;
            _differenceService = differenceService;
            _options = options;
            _containedDifferences = new ConcurrentDictionary<int, IHierarchicalDifferenceCollection>();
        }

        public ITokenizedStringList LeftDecomposition => _left;

        public ITokenizedStringList RightDecomposition => _right;

        public IHierarchicalDifferenceCollection GetContainedDifferences(int index)
        {
            return _options.DifferenceType == 0 ? null : _containedDifferences.GetOrAdd(index, CalculateContainedDiff);
        }

        private IHierarchicalDifferenceCollection CalculateContainedDiff(int index)
        {
            Difference difference = Differences[index];
            if (difference.DifferenceType != DifferenceType.Change)
                return null;
            Span spanInOriginal1 = _left.GetSpanInOriginal(difference.Left);
            Span spanInOriginal2 = _right.GetSpanInOriginal(difference.Right);
            return _differenceService.DiffStrings(_left.OriginalSubstring(spanInOriginal1.Start, spanInOriginal1.Length), _right.OriginalSubstring(spanInOriginal2.Start, spanInOriginal2.Length), _options);
        }

        public bool HasContainedDifferences(int index)
        {
            return GetContainedDifferences(index) != null;
        }

        public IList<Difference> Differences => _differenceCollection.Differences;

        public IList<string> LeftSequence => _left;

        public IEnumerable<Tuple<int, int>> MatchSequence => _differenceCollection.MatchSequence;

        public IList<string> RightSequence => _right;

        public IEnumerator<Difference> GetEnumerator()
        {
            return _differenceCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}