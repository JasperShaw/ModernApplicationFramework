using ModernApplicationFramework.MVVM.ViewModels;

namespace ModernApplicationFramework.MVVM.Controls
{
    public class MainWindow : ModernApplicationFramework.Controls.MainWindow
    {
        public MainWindow()
        {
            DataContext = new MainWindowViewModel(this);
        }
    }
}
