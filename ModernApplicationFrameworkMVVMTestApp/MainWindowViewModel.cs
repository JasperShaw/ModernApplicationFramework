using System.Windows;

namespace ModernApplicationFrameworkMVVMTestApp
{
    public sealed class MainWindowViewModel : ModernApplicationFramework.ViewModels.MainWindowViewModel
    {
        public override void ShowMessage()
        {
            MessageBox.Show("Testing");
        }
    }
}
