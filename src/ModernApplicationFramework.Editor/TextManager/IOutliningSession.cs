namespace ModernApplicationFramework.Editor.TextManager
{
    public interface IOutliningSession
    {
        int AddOutlineRegions(uint dwOutliningFlags, int cRegions,NewOutlineRegion[] rgOutlnReg);
    }
}