using System.ComponentModel.Composition;
using System.Windows.Controls;
using ModernApplicationFramework.Caliburn.Platform.Xaml;
using ModernApplicationFramework.MVVM.Interfaces;
using ModernApplicationFramework.MVVM.Views;
using ModernApplicationFramework.Themes.LightIDE;
using MenuItem = ModernApplicationFramework.Controls.MenuItem;
using ToolBar = ModernApplicationFramework.Controls.ToolBar;

namespace ModernApplicationFramework.MVVM.Demo.Modules
{
    [Export(typeof(IDockingMainWindowViewModel))]
    public class DockingMainWindowViewModel : ViewModels.DockingMainWindowViewModel
    {

        static DockingMainWindowViewModel()
        {
            ViewLocator.AddNamespaceMapping(typeof(DockingMainWindowViewModel).Namespace, typeof(DockingMainWindowView).Namespace);
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            MenuHostViewModel.Items.Add(new MenuItem {Header = "Test"});
            ToolBarHostViewModel.AddToolBar(new ToolBar {IdentifierName = "1"}, true, Dock.Top);

            Theme = new LightTheme();
        }
    }
}
