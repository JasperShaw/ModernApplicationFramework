using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.Interfaces
{
    public interface IHasMainWindowViewModel
    {
        IMainWindowViewModel MainWindowViewModel { get; set; }
    }
}
