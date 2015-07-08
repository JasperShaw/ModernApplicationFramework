using System.Windows.Forms;

namespace ModernApplicationFramework.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public virtual void ShowMessage()
        {
            MessageBox.Show("Test");
        }
    }
}
