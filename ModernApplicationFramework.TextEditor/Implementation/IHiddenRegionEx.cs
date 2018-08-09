namespace ModernApplicationFramework.TextEditor.Implementation
{
    public interface IHiddenRegionEx
    {
        int GetBannerAttr(uint dwLength, uint[] pColorAttr);

        int SetBannerAttr(uint dwLength, uint[] pColorAttr);
    }
}