namespace ModernApplicationFramework.Text.Data
{
    public interface ITextSnapshotLine
    {
        SnapshotPoint End { get; }

        SnapshotPoint EndIncludingLineBreak { get; }

        SnapshotSpan Extent { get; }

        SnapshotSpan ExtentIncludingLineBreak { get; }

        int Length { get; }

        int LengthIncludingLineBreak { get; }

        int LineBreakLength { get; }

        int LineNumber { get; }
        ITextSnapshot Snapshot { get; }

        SnapshotPoint Start { get; }

        string GetLineBreakText();

        string GetText();

        string GetTextIncludingLineBreak();
    }
}