using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.MVVM.Interfaces;
using ModernApplicationFramework.MVVM.Views;

namespace ModernApplicationFramework.MVVM.Demo.Modules
{
    [Export(typeof (IDockingMainWindowViewModel))]
    public class DockingMainWindowViewModel : ViewModels.DockingMainWindowViewModel
    {
        static DockingMainWindowViewModel()
        {
            ViewLocator.AddNamespaceMapping(typeof (DockingMainWindowViewModel).Namespace,
                typeof (DockingMainWindowView).Namespace);
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            Window.Title = "Demo-Tool";
            StatusBar.ModeText = "Ready";
        }
    }
}