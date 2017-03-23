using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.Extended.DockingMainWindow.Views;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.MVVM.Demo.Modules
{
    [Export(typeof (IDockingMainWindowViewModel))]
    public class DockingMainWindowViewModel : Extended.DockingMainWindow.ViewModels.DockingMainWindowViewModel
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