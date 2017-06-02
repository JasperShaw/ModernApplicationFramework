namespace ModernApplicationFramework.Extended.Interfaces
{
    public interface ILayoutItemStatePersister
    {
        void LoadState(IDockingHostViewModel shell, IDockingHost shellView);
        void SaveState(IDockingHostViewModel shell, IDockingHost shellView);
        bool HasStateFile { get; }
    }
}