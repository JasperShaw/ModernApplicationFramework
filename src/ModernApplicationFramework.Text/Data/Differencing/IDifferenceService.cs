using System.Collections.Generic;

namespace ModernApplicationFramework.Text.Data.Differencing
{
    public interface IDifferenceService
    {
        IDifferenceCollection<T> DifferenceSequences<T>(IList<T> left, IList<T> right);

        IDifferenceCollection<T> DifferenceSequences<T>(IList<T> left, IList<T> right, ContinueProcessingPredicate<T> continueProcessingPredicate);
    }
}