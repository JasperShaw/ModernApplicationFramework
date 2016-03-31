using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.MVVM.Interfaces
{
    public interface IDockingMainWindowViewModel : IMainWindowViewModel, IUseDockingHost
    {
    }
}