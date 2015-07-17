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
            this.SourceInitialized += MainWindow_SourceInitialized;
        }

        private void MainWindow_SourceInitialized(object sender, EventArgs e)
        {
            ((MainWindowViewModel)DataContext).ToolBarHostViewModel.AddToolBar(new ToolBar { IdentifierName = "Test" }, true, Dock.Top);
            ((MainWindowViewModel)DataContext).ToolBarHostViewModel.AddToolBar(new ToolBar { IdentifierName = "Test1" }, true, Dock.Top);
            ((MainWindowViewModel)DataContext).ToolBarHostViewModel.AddToolBar(new ToolBar { IdentifierName = "Testing" }, true, Dock.Left);
            ((MainWindowViewModel)DataContext).ToolBarHostViewModel.AddToolBar(new ToolBar { IdentifierName = "Testing2" }, true, Dock.Left);

            var m = new Menu();
            m.Items.Add(new MenuItem { Header = "Test" });
            ((MainWindowViewModel)DataContext).MenuHostViewModel.Menu = m;

            ((MainWindowViewModel)DataContext).ActiveIcon = new BitmapImage(new Uri("pack://application:,,,/ModernApplicationFrameworkTestAppWindow;component/Build.png"));
            ((MainWindowViewModel)DataContext).PassiveIcon = new BitmapImage(new Uri("pack://application:,,,/ModernApplicationFrameworkTestAppWindow;component/test.jpg"));
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
