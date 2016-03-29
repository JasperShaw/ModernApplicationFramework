using System.ComponentModel.Composition;
using ModernApplicationFramework.MVVM.Core;
using ModernApplicationFramework.MVVM.Interfaces;

namespace ModernApplicationFramework.MVVM.Demo.Modules.Startup
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        public override void Initialize()
        {
            DockingHostViewModel.ShowFloatingWindowsInTaskbar = true;
        }
    }
}
