using System.ComponentModel.Composition;
using System.Media;
using System.Windows;
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
            //MainWindow.WindowState = WindowState.Maximized;
        }
    }
}
