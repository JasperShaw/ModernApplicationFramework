using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace ModernApplicationFramework.Test
{
    [Export(typeof(CustomizeDialogViewModel))]
    public sealed class CustomizeDialogViewModel : Conductor<IScreen>.Collection.OneActive
    {



        public CustomizeDialogViewModel()
        {
            var toolbarsViewModel = IoC.Get<TabViewModel>();
            ActivateItem(toolbarsViewModel);
        }


        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
        }
    }
}
