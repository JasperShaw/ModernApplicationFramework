namespace ModernApplicationFramework.TextEditor.Implementation
{
    public interface ITextViewFilter
    {
        int GetWordExtent(int iLine, int iIndex, uint dwFlags, TextSpan[] pSpan);
  
        int GetDataTipText(TextSpan[] pSpan, out string pbstrText);

        int GetPairExtents(int iLine, int iIndex, TextSpan[] pSpan);
    }
}