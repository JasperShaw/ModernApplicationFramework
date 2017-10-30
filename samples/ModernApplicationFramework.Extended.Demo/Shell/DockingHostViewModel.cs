using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.Extended.DockingHost.Views;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Extended.Demo.Shell
{
    [Export(typeof(IDockingHostViewModel))]
    public class DockingHostViewModel : DockingHost.ViewModels.DockingHostViewModel
    {

        static DockingHostViewModel()
        {
            ViewLocator.AddNamespaceMapping(typeof(DockingHostViewModel).Namespace,
                typeof(DockingHostView).Namespace);
        }
    }
}
