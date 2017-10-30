namespace MordernApplicationFramework.WindowManagement.WindowProfile
{
    public interface IDefaultWindowProfileProvider
    {
        WindowProfile GetLayout(string profileName);
    }
}
