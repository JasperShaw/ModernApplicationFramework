namespace ModernApplicationFramework.TextEditor
{
    public interface ILineBreaks
    {
        int Length { get; }

        int StartOfLineBreak(int index);

        int EndOfLineBreak(int index);
    }
}