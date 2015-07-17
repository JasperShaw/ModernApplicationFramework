using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Menu = ModernApplicationFramework.Controls.Menu;
using MenuItem = ModernApplicationFramework.Controls.MenuItem;
using ToolBar = ModernApplicationFramework.Controls.ToolBar;

namespace ModernApplicationFrameworkMVVMTestApp
{
    public sealed class MainWindowViewModel : ModernApplicationFramework.ViewModels.MainWindowViewModel
    {
        protected override void OnTest()
        {
            base.OnTest();
            //MessageBox.Show("Testing");
            ActiveIcon = new BitmapImage(new Uri("pack://application:,,,/ModernApplicationFrameworkMVVMTestApp;component/test.jpg"));
        }

        public MainWindowViewModel(ModernApplicationFramework.Controls.MainWindow mainWindow) : base(mainWindow)
        {
            ActiveIcon = new BitmapImage(new Uri("pack://application:,,,/ModernApplicationFrameworkMVVMTestApp;component/Build.png"));
            PassiveIcon = new BitmapImage(new Uri("pack://application:,,,/ModernApplicationFrameworkMVVMTestApp;component/test.jpg"));
        }


        protected override void InitializeMainWindow()
        {
            base.InitializeMainWindow();
            var m = new Menu();
            m.Items.Add(new MenuItem { Header = "Test" });
            MenuHostViewModel.Menu = m;
            ToolBarHostViewModel.AddToolBar(new ToolBar { IdentifierName = "Test" }, true, Dock.Top);
        }
    }
}
