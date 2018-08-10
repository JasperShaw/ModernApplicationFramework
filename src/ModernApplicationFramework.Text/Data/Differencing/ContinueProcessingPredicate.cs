using System.Collections.Generic;

namespace ModernApplicationFramework.Text.Data.Differencing
{
    public delegate bool ContinueProcessingPredicate<T>(int leftIndex, IList<T> leftSequence, int longestMatchSoFar);
}