using System;

namespace ModernApplicationFramework.Text.Logic.Differencing
{
    public class SnapshotDifferenceChangeEventArgs : EventArgs
    {
        public SnapshotDifferenceChangeEventArgs(ISnapshotDifference before, ISnapshotDifference after)
        {
            Before = before;
            After = after;
        }

        public ISnapshotDifference Before { get; }

        public ISnapshotDifference After { get; }
    }
}