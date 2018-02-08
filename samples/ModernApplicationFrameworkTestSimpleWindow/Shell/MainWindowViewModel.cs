using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFrameworkTestSimpleWindow.Shell
{
    [Export(typeof(IWindowViewModel))]
    public class MainWindowViewModel : ModernApplicationFramework.Basics.WindowModels.WindowViewModelConductor
    {
        protected override void OnViewReady(object view)
        {
            base.OnViewReady(view);
            IsSimpleWindow = true;
            UseSimpleMovement = true;
        }

        public override void OnImportsSatisfied()
        {
            base.OnImportsSatisfied();
            ActivateItem(IoC.Get<TestScreen.TestScreenViewModel>());
        }
    }
}
