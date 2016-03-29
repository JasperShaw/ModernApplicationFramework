namespace ModernApplicationFramework.MVVM.Interfaces
{
    public interface IMainWindowViewModel : ModernApplicationFramework.ViewModels.IMainWindowViewModel
    {
        IDockingHostViewModel DockingHost { get; }
    }
}
