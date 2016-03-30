using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ModernApplicationFramework.Themes.LightIDE;
using ModernApplicationFramework.ViewModels;
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
            SourceInitialized += MainWindow_SourceInitialized;
        }

        private void MainWindow_SourceInitialized(object sender, EventArgs e)
        {
            ((MainWindowViewModel)DataContext).ToolBarHostViewModel.AddToolBar(new ToolBar { IdentifierName = "Test" }, true, Dock.Top);
            ((MainWindowViewModel)DataContext).ToolBarHostViewModel.AddToolBar(new ToolBar { IdentifierName = "Test1" }, true, Dock.Top);
            ((MainWindowViewModel)DataContext).ToolBarHostViewModel.AddToolBar(new ToolBar { IdentifierName = "Testing" }, true, Dock.Left);
            ((MainWindowViewModel)DataContext).ToolBarHostViewModel.AddToolBar(new ToolBar { IdentifierName = "Testing2" }, true, Dock.Left);

            ((MainWindowViewModel)DataContext).ActiveIcon = new BitmapImage(new Uri("pack://application:,,,/ModernApplicationFrameworkTestAppWindow;component/Build.png"));
            ((MainWindowViewModel)DataContext).PassiveIcon = new BitmapImage(new Uri("pack://application:,,,/ModernApplicationFrameworkTestAppWindow;component/test.jpg"));
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindowViewModel)DataContext).Theme = new LightTheme();
        }

        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var a = sender as TextBox;
            Title = a.Text;
        }
    }
}
