using System;
using ModernApplicationFramework.Text.Data.Projection;

namespace ModernApplicationFramework.Text.Data
{
    public interface IMappingPoint
    {
        ITextBuffer AnchorBuffer { get; }

        IBufferGraph BufferGraph { get; }

        SnapshotPoint? GetInsertionPoint(Predicate<ITextBuffer> match);
        SnapshotPoint? GetPoint(ITextBuffer targetBuffer, PositionAffinity affinity);

        SnapshotPoint? GetPoint(ITextSnapshot targetSnapshot, PositionAffinity affinity);

        SnapshotPoint? GetPoint(Predicate<ITextBuffer> match, PositionAffinity affinity);
    }
}