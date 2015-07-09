using System.Windows.Forms;
using System.Windows.Input;
using ModernApplicationFramework.Commands;

namespace ModernApplicationFramework.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ICommand TestCommand => new Command(OnTest);

        protected virtual void OnTest()
        {
            MessageBox.Show("Test");
        }
    }
}
