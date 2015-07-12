using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ModernApplicationFramework.Themes.LightIDE;
using ModernApplicationFramework.ViewModels;
using Menu = ModernApplicationFramework.Controls.Menu;
using MenuItem = ModernApplicationFramework.Controls.MenuItem;
using ToolBar = ModernApplicationFramework.Controls.ToolBar;

namespace ModernApplicationFrameworkTestAppWindow
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Icon = new BitmapImage(new Uri("pack://application:,,,/ModernApplicationFrameworkTestAppWindow;component/Build.png"));
        }

        protected override void PopulateMenuAndToolBars()
        {
            ((MainWindowViewModel)DataContext).ToolBarHostViewModel.AddToolBar(new ToolBar { IdentifierName = "Test" }, true, Dock.Top);
            ((MainWindowViewModel)DataContext).ToolBarHostViewModel.AddToolBar(new ToolBar { IdentifierName = "Test1" }, true, Dock.Top);
            ((MainWindowViewModel)DataContext).ToolBarHostViewModel.AddToolBar(new ToolBar { IdentifierName = "Testing" }, true, Dock.Left);
            ((MainWindowViewModel)DataContext).ToolBarHostViewModel.AddToolBar(new ToolBar { IdentifierName = "Testing2" }, true, Dock.Left);

            var m = new Menu();
            m.Items.Add(new MenuItem {Header = "Test"});
            ((MainWindowViewModel)DataContext).MenuHostViewModel.Menu = m;
        }

        protected override void SetWindowIcons()
        {
            ActivatedIcon = new BitmapImage(new Uri("pack://application:,,,/ModernApplicationFrameworkTestAppWindow;component/Build.png"));
            DeactivatedIcon = new BitmapImage(new Uri("pack://application:,,,/ModernApplicationFrameworkTestAppWindow;component/test.jpg"));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Theme = new LightTheme();
        }

        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var a = sender as TextBox;
            Title = a.Text;
        }
    }
}
