namespace ModernApplicationFramework.Modules.Editor.Text
{
    public interface ILineBreaks
    {
        int Length { get; }

        int EndOfLineBreak(int index);

        int StartOfLineBreak(int index);
    }
}