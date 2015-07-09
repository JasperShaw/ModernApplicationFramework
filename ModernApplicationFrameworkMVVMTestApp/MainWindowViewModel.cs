using System.Windows;

namespace ModernApplicationFrameworkMVVMTestApp
{
    public sealed class MainWindowViewModel : ModernApplicationFramework.ViewModels.MainWindowViewModel
    {
        protected override void OnTest()
        {
            MessageBox.Show("Testing");
        }
    }
}
