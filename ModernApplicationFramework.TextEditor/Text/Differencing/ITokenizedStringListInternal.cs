namespace ModernApplicationFramework.TextEditor.Text.Differencing
{
    internal interface ITokenizedStringListInternal : ITokenizedStringList
    {
        string OriginalSubstring(int startIndex, int length);
    }
}