namespace ModernApplicationFramework.TextEditor.Implementation
{
    internal interface IHiddenTextSessionExPrivate
    {
        int AddHiddenRegionsEx(uint dwUpdateFlags, int cRegions, NewHiddenRegionEx[] rgHidReg, IEnumHiddenRegions[] ppEnum);
    }
}