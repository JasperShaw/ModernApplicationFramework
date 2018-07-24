using System;
using System.Collections.Generic;
using ModernApplicationFramework.TextEditor.Text.Differencing;

namespace ModernApplicationFramework.TextEditor.Text
{
    public interface IDifferenceCollection<T> : IEnumerable<Difference>
    {
        IEnumerable<Tuple<int, int>> MatchSequence { get; }

        IList<T> LeftSequence { get; }

        IList<T> RightSequence { get; }

        IList<Difference> Differences { get; }
    }
}