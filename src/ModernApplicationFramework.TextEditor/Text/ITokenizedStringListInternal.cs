using ModernApplicationFramework.Text.Data.Differencing;

namespace ModernApplicationFramework.TextEditor.Text
{
    internal interface ITokenizedStringListInternal : ITokenizedStringList
    {
        string OriginalSubstring(int startIndex, int length);
    }
}