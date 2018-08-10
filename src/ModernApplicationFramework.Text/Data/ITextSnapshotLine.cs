namespace ModernApplicationFramework.Text.Data
{
    public interface ITextSnapshotLine
    {
        ITextSnapshot Snapshot { get; }

        SnapshotSpan Extent { get; }

        SnapshotSpan ExtentIncludingLineBreak { get; }

        int LineNumber { get; }

        SnapshotPoint Start { get; }

        int Length { get; }

        int LengthIncludingLineBreak { get; }

        SnapshotPoint End { get; }

        SnapshotPoint EndIncludingLineBreak { get; }

        int LineBreakLength { get; }

        string GetText();

        string GetTextIncludingLineBreak();

        string GetLineBreakText();
    }
}