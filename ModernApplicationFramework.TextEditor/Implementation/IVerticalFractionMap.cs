using System;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    public interface IVerticalFractionMap
    {
        ITextView TextView { get; }

        double GetFractionAtBufferPosition(SnapshotPoint bufferPosition);

        SnapshotPoint GetBufferPositionAtFraction(double fraction);

        event EventHandler MappingChanged;
    }
}