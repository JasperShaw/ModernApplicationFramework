using System;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public interface IVerticalFractionMap
    {
        event EventHandler MappingChanged;
        ITextView TextView { get; }

        SnapshotPoint GetBufferPositionAtFraction(double fraction);

        double GetFractionAtBufferPosition(SnapshotPoint bufferPosition);
    }
}