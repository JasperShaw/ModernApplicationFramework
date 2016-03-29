namespace ModernApplicationFramework.MVVM.Interfaces
{
    public interface ILayoutItemStatePersister
    {
        void LoadState(IDockingHostViewModel shell, IDockingHost shellView, string fileName);
        void SaveState(IDockingHostViewModel shell, IDockingHost shellView, string fileName);
    }
}