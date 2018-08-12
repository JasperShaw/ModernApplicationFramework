namespace ModernApplicationFramework.Modules.Editor.Differencing
{
    public interface IDiffChange
    {
        DiffChangeType ChangeType { get; }

        int ModifiedEnd { get; }

        int ModifiedLength { get; }

        int ModifiedStart { get; }

        int OriginalEnd { get; }

        int OriginalLength { get; }

        int OriginalStart { get; }

        IDiffChange Add(IDiffChange diffChange);
    }
}