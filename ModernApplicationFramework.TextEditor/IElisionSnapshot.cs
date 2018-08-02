namespace ModernApplicationFramework.TextEditor
{
    public interface IElisionSnapshot : IProjectionSnapshot
    {
        new IElisionBuffer TextBuffer { get; }

        ITextSnapshot SourceSnapshot { get; }

        SnapshotPoint MapFromSourceSnapshotToNearest(SnapshotPoint point);
    }
}