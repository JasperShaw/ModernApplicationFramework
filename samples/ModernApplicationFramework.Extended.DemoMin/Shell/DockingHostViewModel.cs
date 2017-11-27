using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.Extended.Interfaces;
using DockingHostView = ModernApplicationFramework.Extended.Controls.DockingHost.Views.DockingHostView;

namespace ModernApplicationFramework.Extended.DemoMin.Shell
{
    [Export(typeof(IDockingHostViewModel))]
    public class DockingHostViewModel : Controls.DockingHost.ViewModels.DockingHostViewModel
    {

        static DockingHostViewModel()
        {
            ViewLocator.AddNamespaceMapping(typeof(DockingHostViewModel).Namespace,
                typeof(DockingHostView).Namespace);
        }
    }
}
