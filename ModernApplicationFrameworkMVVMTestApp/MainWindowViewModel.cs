using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ModernApplicationFramework.Themes.LightIDE;
using ToolBar = ModernApplicationFramework.Controls.ToolBar;

namespace ModernApplicationFrameworkMVVMTestApp
{
    public sealed class MainWindowViewModel : ModernApplicationFramework.Basics.ViewModels.MainWindowViewModel
    {
        protected override void OnTest()
        {
            Theme = new LightTheme();
        }

        public MainWindowViewModel(ModernApplicationFramework.Controls.MainWindow mainWindow) : base(mainWindow)
        {
            ActiveIcon = new BitmapImage(new Uri("pack://application:,,,/ModernApplicationFrameworkMVVMTestApp;component/Build.png"));
            PassiveIcon = new BitmapImage(new Uri("pack://application:,,,/ModernApplicationFrameworkMVVMTestApp;component/test.jpg"));
        }


        protected override void InitializeMainWindow()
        {
            base.InitializeMainWindow();
            ToolBarHostViewModel.AddToolBar(new ToolBar { IdentifierName = "Test" }, true, Dock.Top);
        }
    }
}
