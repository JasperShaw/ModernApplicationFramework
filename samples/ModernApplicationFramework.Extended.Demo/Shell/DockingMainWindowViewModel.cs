using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Interfaces.Services;
using DockingMainWindowView = ModernApplicationFramework.Extended.Controls.DockingMainWindow.Views.DockingMainWindowView;

namespace ModernApplicationFramework.Extended.Demo.Shell
{
    [Export(typeof (IDockingMainWindowViewModel))]
    public class DockingMainWindowViewModel : Controls.DockingMainWindow.ViewModels.DockingMainWindowViewModel
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
            IoC.Get<IStatusBarDataModelService>().SetReadyText();
        }        
    }
}