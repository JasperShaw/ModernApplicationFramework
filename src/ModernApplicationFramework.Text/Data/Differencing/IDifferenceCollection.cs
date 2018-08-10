using System;
using System.Collections.Generic;

namespace ModernApplicationFramework.Text.Data.Differencing
{
    public interface IDifferenceCollection<T> : IEnumerable<Difference>
    {
        IEnumerable<Tuple<int, int>> MatchSequence { get; }

        IList<T> LeftSequence { get; }

        IList<T> RightSequence { get; }

        IList<Difference> Differences { get; }
    }
}