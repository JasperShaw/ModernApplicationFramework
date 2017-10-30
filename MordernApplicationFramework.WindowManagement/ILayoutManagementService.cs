namespace MordernApplicationFramework.WindowManagement
{
    public interface ILayoutManagementService
    {
        void LoadLayout(string profileName);
        void SaveActiveFrameLayout();
        void Reload();
    }
}