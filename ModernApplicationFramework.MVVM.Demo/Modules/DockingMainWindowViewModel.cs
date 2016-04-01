using System.ComponentModel.Composition;
using System.Windows;
using ModernApplicationFramework.Caliburn.Platform.Xaml;
using ModernApplicationFramework.Interfaces.Utilities;
using ModernApplicationFramework.MVVM.Interfaces;
using ModernApplicationFramework.MVVM.Views;
using ModernApplicationFramework.Themes.LightIDE;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.MVVM.Demo.Modules
{
    [Export(typeof(IDockingMainWindowViewModel))]
    public class DockingMainWindowViewModel : ViewModels.DockingMainWindowViewModel
    {

        [Import]
        private IKeyGestureHandler _commandKeyGestureService;

        static DockingMainWindowViewModel()
        {
            ViewLocator.AddNamespaceMapping(typeof(DockingMainWindowViewModel).Namespace, typeof(DockingMainWindowView).Namespace);
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            new MenuCreator().CreateMenu(MenuHostViewModel, new MenuItemDefinitionsPopulator());

            _commandKeyGestureService.BindKeyGesture((UIElement) view);

            new ToolbarTrayCreator().CreateToolbarTray(ToolBarHostViewModel, new ToolbarDefinitionsPopulator());           

            Theme = new LightTheme();
        }
    }
}
