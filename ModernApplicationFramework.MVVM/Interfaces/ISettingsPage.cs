namespace ModernApplicationFramework.MVVM.Interfaces
{
    public interface ISettingsPage
    {
        string Name { get; }
        string Path { get; }
        void Apply();
    }
}
