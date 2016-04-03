using System.Windows.Input;
using ModernApplicationFramework.Caliburn;
using ModernApplicationFramework.Commands.Service;
using ModernApplicationFramework.MVVM.Commands;

namespace ModernApplicationFramework.MVVM.Demo.Modules
{
    public partial class TestToolBar
    {
        public TestToolBar()
        {
            InitializeComponent();
        }

        public ICommand TestCommand => IoC.Get<ICommandService>().GetCommandDefinition(typeof(UndoCommandDefinition)).Command;
    }
}
