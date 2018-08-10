using System.Collections.Generic;
using ModernApplicationFramework.Text.Data.Differencing;

namespace ModernApplicationFramework.TextEditor.Text.Differencing
{
    public interface IDifferenceService
    {
        IDifferenceCollection<T> DifferenceSequences<T>(IList<T> left, IList<T> right);

        IDifferenceCollection<T> DifferenceSequences<T>(IList<T> left, IList<T> right, ContinueProcessingPredicate<T> continueProcessingPredicate);
    }
}