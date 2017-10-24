using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.Extended.DockingHost.Views;
using ModernApplicationFramework.Extended.Interfaces;
using MordernApplicationFramework.WindowManagement.LayoutState;

namespace ModernApplicationFramework.MVVM.Demo.Shell
{
    [Export(typeof(IDockingHostViewModel))]
    public class DockingHostViewModel : Extended.DockingHost.ViewModels.DockingHostViewModel
    {
        [Import] private ILayoutItemStatePersister _layoutItemStatePersister;


        static DockingHostViewModel()
        {
            ViewLocator.AddNamespaceMapping(typeof(DockingHostViewModel).Namespace,
                typeof(DockingHostView).Namespace);
        }


        protected override void OnViewLoaded(object view)
        {
            _layoutItemStatePersister.Initialize(this, DockingHostView);

            if (_layoutItemStatePersister.HasStateFile)
                _layoutItemStatePersister.LoadFromFile(ProcessStateOption.Complete);
            base.OnViewLoaded(view);
        }


        protected override void PreviewDeactivating(bool close)
        {
            _layoutItemStatePersister.SaveToFile(ProcessStateOption.Complete | ProcessStateOption.UseShouldReopenOnStart);
        }
    }
}
