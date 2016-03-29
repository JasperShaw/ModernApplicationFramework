namespace ModernApplicationFramework.MVVM.Interfaces
{
    public interface ILayoutItemStatePersister
    {
        void SaveState(IDockingHostViewModel shell, IDockingHost shellView, string fileName);
        void LoadState(IDockingHostViewModel shell, IDockingHost shellView, string fileName);
    }
}
