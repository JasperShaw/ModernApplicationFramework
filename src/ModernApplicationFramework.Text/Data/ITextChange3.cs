namespace ModernApplicationFramework.Text.Data
{
    public interface ITextChange3 : ITextChange2
    {
        string GetOldText(Span span);

        string GetNewText(Span span);

        char GetOldTextAt(int position);

        char GetNewTextAt(int position);
    }
}