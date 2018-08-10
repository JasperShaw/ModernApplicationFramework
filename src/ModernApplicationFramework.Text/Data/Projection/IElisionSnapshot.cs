namespace ModernApplicationFramework.Text.Data.Projection
{
    public interface IElisionSnapshot : IProjectionSnapshot
    {
        ITextSnapshot SourceSnapshot { get; }
        new IElisionBuffer TextBuffer { get; }

        SnapshotPoint MapFromSourceSnapshotToNearest(SnapshotPoint point);
    }
}