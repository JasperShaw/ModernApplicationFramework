namespace ModernApplicationFramework.MVVM.Interfaces
{
    public interface ISettingsPage
    {
        int SortOrder { get; }
        string Name { get; }
        string Path { get; }
        void Apply();
        bool CanApply();
    }
}
