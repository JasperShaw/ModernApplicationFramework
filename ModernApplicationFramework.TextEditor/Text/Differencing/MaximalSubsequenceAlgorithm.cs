using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Data.Differencing;

namespace ModernApplicationFramework.TextEditor.Text.Differencing
{
    [Export(typeof(IDifferenceService))]
    internal sealed class MaximalSubsequenceAlgorithm : IDifferenceService
    {
        private static readonly IDiffChange[] Empty = new IDiffChange[0];

        public IDifferenceCollection<T> DifferenceSequences<T>(IList<T> left, IList<T> right)
        {
            return DifferenceSequences(left, right, null);
        }

        public IDifferenceCollection<T> DifferenceSequences<T>(IList<T> left, IList<T> right, ContinueProcessingPredicate<T> continueProcessingPredicate)
        {
            return DifferenceSequences(left, right, left, right, continueProcessingPredicate);
        }

        internal static DifferenceCollection<T> DifferenceSequences<T>(IList<T> left, IList<T> right, IList<T> originalLeft, IList<T> originalRight, ContinueProcessingPredicate<T> continueProcessingPredicate)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));
            if (right == null)
                throw new ArgumentNullException(nameof(right));
            IDiffChange[] changes;
            if (left.Count == 0 || right.Count == 0)
            {
                if (left.Count == 0 && right.Count == 0)
                    changes = Empty;
                else
                    changes = new IDiffChange[]
                    {
            new DiffChange(0, left.Count, 0, right.Count)
                    };
            }
            else
                changes = ComputeMaximalSubsequence(left, right, continueProcessingPredicate);
            return DiffChangeCollectionHelper<T>.Create(changes, originalLeft, originalRight);
        }

        private static IDiffChange[] ComputeMaximalSubsequence<T>(IList<T> left, IList<T> right, ContinueProcessingPredicate<T> continueProcessingPredicate)
        {
            return new LcsDiff<T>().Diff(left, right, EqualityComparer<T>.Default, continueProcessingPredicate == null ? null : (ContinueDifferencePredicate<T>)((originalIndex, originalSequence, longestMatchSoFar) => continueProcessingPredicate(originalIndex, originalSequence, longestMatchSoFar)));
        }
    }
}
