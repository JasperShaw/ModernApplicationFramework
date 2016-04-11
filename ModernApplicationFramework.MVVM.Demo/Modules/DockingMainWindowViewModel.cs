using System.ComponentModel.Composition;
using System.Windows;
using ModernApplicationFramework.Caliburn.Platform.Xaml;
using ModernApplicationFramework.Interfaces.Utilities;
using ModernApplicationFramework.MVVM.Interfaces;
using ModernApplicationFramework.MVVM.Views;
using ModernApplicationFramework.Themes.LightIDE;

namespace ModernApplicationFramework.MVVM.Demo.Modules
{
    [Export(typeof (IDockingMainWindowViewModel))]
    public class DockingMainWindowViewModel : ViewModels.DockingMainWindowViewModel
    {
#pragma warning disable 649
        [Import] private IKeyGestureHandler _commandKeyGestureService;

        [Import] private IMenuCreator _menuCreator;

        [Import] private IToolbarTrayCreator _toolbarTrayCreator;
#pragma warning restore 649

        static DockingMainWindowViewModel()
        {
            ViewLocator.AddNamespaceMapping(typeof (DockingMainWindowViewModel).Namespace,
                typeof (DockingMainWindowView).Namespace);
        }


        public static DockingMainWindowViewModel Instance { get; private set; }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            _menuCreator.CreateMenu(MenuHostViewModel);

            _commandKeyGestureService.BindKeyGesture((UIElement) view);

            _toolbarTrayCreator.CreateToolbarTray(ToolBarHostViewModel);

            Theme = new LightTheme();

            Instance = this;

            Window.Title = "Demo-Tool";
        }
    }
}