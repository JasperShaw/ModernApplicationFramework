namespace ModernApplicationFramework.WindowManagement.WindowProfile
{
    public interface IDefaultWindowProfileProvider
    {
        WindowProfile GetLayout(string profileName);
    }
}
