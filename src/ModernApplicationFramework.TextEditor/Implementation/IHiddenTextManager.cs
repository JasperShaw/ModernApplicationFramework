namespace ModernApplicationFramework.Editor.Implementation
{
    public interface IHiddenTextManager
    {
        int GetHiddenTextSession(object pOwningObject, out IHiddenTextSession ppSession);

        int CreateHiddenTextSession(uint dwFlags, object pOwningObject, IHiddenTextClient pClient, out IHiddenTextSession ppState);
    }
}