namespace ModernApplicationFramework.Editor.Implementation
{
    public interface IOutliningSession
    {
        int AddOutlineRegions(uint dwOutliningFlags, int cRegions,NewOutlineRegion[] rgOutlnReg);
    }
}