using System.Windows;

namespace ModernApplicationFrameworkMVVMTestApp
{
    public sealed class MainWindowViewModel : ModernApplicationFramework.ViewModels.MainWindowViewModel
    {
        protected override void OnTest()
        {
            base.OnTest();
            //MessageBox.Show("Testing");
        }

        public MainWindowViewModel(ModernApplicationFramework.Controls.MainWindow mainWindow) : base(mainWindow)
        {
        }
    }
}
