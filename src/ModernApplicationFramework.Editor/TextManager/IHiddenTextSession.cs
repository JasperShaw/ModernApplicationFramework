using ModernApplicationFramework.Editor.Implementation;

namespace ModernApplicationFramework.Editor.TextManager
{
    public interface IHiddenTextSession
    {
        int AddHiddenRegions(uint dwUpdateFlags, int cRegions, NewHiddenRegion[] rgHidReg, IEnumHiddenRegions[] ppEnum);

        int EnumHiddenRegions(uint dwFindFlags, uint dwCookie,  TextSpan[] ptsRange, out IEnumHiddenRegions ppEnum);

        int UnadviseClient();

        int Terminate();
    }
}