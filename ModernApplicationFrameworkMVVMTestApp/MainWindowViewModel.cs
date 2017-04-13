using System;
using System.Windows.Media.Imaging;

namespace ModernApplicationFrameworkMVVMTestApp
{
    public sealed class MainWindowViewModel : ModernApplicationFramework.Basics.MainWindowViewModel
    {
        public MainWindowViewModel(ModernApplicationFramework.Controls.MainWindow mainWindow) : base(mainWindow)
        {
            ActiveIcon = new BitmapImage(new Uri("pack://application:,,,/ModernApplicationFrameworkMVVMTestApp;component/Build.png"));
            PassiveIcon = new BitmapImage(new Uri("pack://application:,,,/ModernApplicationFrameworkMVVMTestApp;component/test.jpg"));
        }
    }
}
