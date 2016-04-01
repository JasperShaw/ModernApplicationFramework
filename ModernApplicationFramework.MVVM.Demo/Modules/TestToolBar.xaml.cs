using System.Windows.Input;

namespace ModernApplicationFramework.MVVM.Demo.Modules
{
    public partial class TestToolBar
    {
        public TestToolBar()
        {
            InitializeComponent();
        }

        public ICommand TestCommand => new TestCommandDefinition().Command;
    }
}
