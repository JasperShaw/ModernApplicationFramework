namespace ModernApplicationFramework.MVVM.Interfaces
{
    interface ILayoutItemStatePersister
    {
        void SaveState(IDockingHostViewModel shell, IDockingHost shellView, string fileName);
        void LoadState(IDockingHostViewModel shell, IDockingHost shellView, string fileName);
    }
}
