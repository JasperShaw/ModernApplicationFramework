using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.MVVM.Interfaces;
using IMainWindowViewModel = ModernApplicationFramework.MVVM.Interfaces.IMainWindowViewModel;

namespace ModernApplicationFramework.MVVM.ViewModels
{
    [Export(typeof(IMainWindowViewModel))]
    public class MainWindowViewModel : ModernApplicationFramework.ViewModels.MainWindowViewModel, IPartImportsSatisfiedNotification
    {
        public MainWindowViewModel(MainWindow mainWindow) : base(mainWindow)
        {
        }

        [Import]
        private IDockingHostViewModel _dockingHost;

        public IDockingHostViewModel DockingHost => _dockingHost;
        public void OnImportsSatisfied()
        {
            ActivateItem(_dockingHost);
        }
    }
}
