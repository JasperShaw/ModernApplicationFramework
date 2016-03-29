using System;
using System.Windows.Controls;
using ModernApplicationFramework.ViewModels;
using Menu = ModernApplicationFramework.Controls.Menu;
using MenuItem = ModernApplicationFramework.Controls.MenuItem;
using ToolBar = ModernApplicationFramework.Controls.ToolBar;

namespace ModernApplicationFramework.MVVM.Demo
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            //DataContext = new ModernApplicationFramework.ViewModels.UseDockingHost(this);
        }

        private void MainWindow_OnSourceInitialized(object sender, EventArgs e)
        {
            var m = new Menu();

            var mi2 = new MenuItem { Header = "Test2" };
            mi2.Items.Add(new MenuItem { Header = "Test2" });

            var mi = new MenuItem { Header = "Test" };
            mi.Items.Add(mi2);


            m.Items.Add(mi);
            ((IMainWindowViewModel)DataContext).MenuHostViewModel.Menu = m;
            ((IMainWindowViewModel)DataContext).ToolBarHostViewModel.AddToolBar(new ToolBar { IdentifierName = "Test" }, true, Dock.Top);
        }
    }
}
