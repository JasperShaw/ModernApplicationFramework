using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Differencing;

namespace ModernApplicationFramework.Text.Logic.Differencing
{
    public delegate bool IgnoreDifferencePredicate(Difference lineDifference, ITextSnapshot leftSnapshot, ITextSnapshot rightSnapshot);
}