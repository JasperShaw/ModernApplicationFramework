namespace ModernApplicationFramework.Text.Data
{
    public interface ITextChange
    {
        Span OldSpan { get; }

        Span NewSpan { get; }

        int OldPosition { get; }

        int NewPosition { get; }

        int Delta { get; }

        int OldEnd { get; }

        int NewEnd { get; }

        string OldText { get; }

        string NewText { get; }

        int OldLength { get; }

        int NewLength { get; }

        int LineCountDelta { get; }
    }
}