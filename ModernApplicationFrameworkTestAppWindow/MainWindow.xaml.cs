using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ModernApplicationFramework.Basics;

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
            ((MainWindowViewModel)DataContext).ActiveIcon = new BitmapImage(new Uri("pack://application:,,,/ModernApplicationFrameworkTestAppWindow;component/Build.png"));
            ((MainWindowViewModel)DataContext).PassiveIcon = new BitmapImage(new Uri("pack://application:,,,/ModernApplicationFrameworkTestAppWindow;component/test.jpg"));
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }

        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var a = sender as TextBox;
            Title = a.Text;
        }
    }
}
