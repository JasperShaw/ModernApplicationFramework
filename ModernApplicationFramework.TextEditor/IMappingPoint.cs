using System;

namespace ModernApplicationFramework.TextEditor
{
    public interface IMappingPoint
    {
        SnapshotPoint? GetPoint(ITextBuffer targetBuffer, PositionAffinity affinity);

        SnapshotPoint? GetPoint(ITextSnapshot targetSnapshot, PositionAffinity affinity);

        SnapshotPoint? GetPoint(Predicate<ITextBuffer> match, PositionAffinity affinity);

        SnapshotPoint? GetInsertionPoint(Predicate<ITextBuffer> match);

        ITextBuffer AnchorBuffer { get; }

        IBufferGraph BufferGraph { get; }
    }
}