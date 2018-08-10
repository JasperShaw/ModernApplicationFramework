namespace ModernApplicationFramework.Text.Data
{
    public interface ITextChange
    {
        int Delta { get; }

        int LineCountDelta { get; }

        int NewEnd { get; }

        int NewLength { get; }

        int NewPosition { get; }

        Span NewSpan { get; }

        string NewText { get; }

        int OldEnd { get; }

        int OldLength { get; }

        int OldPosition { get; }
        Span OldSpan { get; }

        string OldText { get; }
    }
}