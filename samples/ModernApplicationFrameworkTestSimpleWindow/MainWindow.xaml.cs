using ModernApplicationFramework.Controls.Windows;

namespace ModernApplicationFrameworkTestSimpleWindow
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            //IoC.Get<IStatusBarService>().SetVisibility(0);
            ((MainWindowViewModel) DataContext).UseTitleBar = false;
            ((MainWindowViewModel) DataContext).IsSimpleWindow = true;
            ((MainWindowViewModel) DataContext).UseSimpleMovement = true;
            ((MainWindowViewModel) DataContext).UseMenu = false;
        }
    }
}
