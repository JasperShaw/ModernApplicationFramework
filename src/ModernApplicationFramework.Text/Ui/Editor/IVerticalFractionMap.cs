using System;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public interface IVerticalFractionMap
    {
        ITextView TextView { get; }

        double GetFractionAtBufferPosition(SnapshotPoint bufferPosition);

        SnapshotPoint GetBufferPositionAtFraction(double fraction);

        event EventHandler MappingChanged;
    }
}