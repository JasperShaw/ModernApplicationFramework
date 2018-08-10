using System;
using System.Collections.Generic;

namespace ModernApplicationFramework.Text.Data.Differencing
{
    public interface IDifferenceCollection<T> : IEnumerable<Difference>
    {
        IList<Difference> Differences { get; }

        IList<T> LeftSequence { get; }
        IEnumerable<Tuple<int, int>> MatchSequence { get; }

        IList<T> RightSequence { get; }
    }
}