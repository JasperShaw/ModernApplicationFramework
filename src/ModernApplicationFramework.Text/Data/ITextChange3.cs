namespace ModernApplicationFramework.Text.Data
{
    public interface ITextChange3 : ITextChange2
    {
        string GetNewText(Span span);

        char GetNewTextAt(int position);
        string GetOldText(Span span);

        char GetOldTextAt(int position);
    }
}