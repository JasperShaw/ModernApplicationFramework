namespace ModernApplicationFramework.Modules.Editor.Text
{
    public interface ILineBreaks
    {
        int Length { get; }

        int StartOfLineBreak(int index);

        int EndOfLineBreak(int index);
    }
}