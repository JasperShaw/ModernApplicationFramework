using System.Collections.Generic;

namespace ModernApplicationFramework.TextEditor.Text.Differencing
{
    public delegate bool ContinueProcessingPredicate<T>(int leftIndex, IList<T> leftSequence, int longestMatchSoFar);
}