using ModernApplicationFramework.Text.Data.Differencing;

namespace ModernApplicationFramework.Modules.Editor.Differencing
{
    internal interface ITokenizedStringListInternal : ITokenizedStringList
    {
        string OriginalSubstring(int startIndex, int length);
    }
}