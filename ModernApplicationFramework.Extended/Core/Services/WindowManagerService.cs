using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Extended.Core.Services
{
    [Export(typeof(WindowManagerService))]
    public class WindowManagerService
    {
        private readonly BindableCollection<ITool> _tools;

        private IDockingHostViewModel _dockingHost;

        public IObservableCollection<ITool> Tools => _tools;

        //IObservableCollection<ILayoutItem> Documents => _

        public WindowManagerService()
        {
            _tools = new BindableCollection<ITool>();
        }

        public void ShowTool<TTool>()
            where TTool : ITool
        {
            ShowTool(IoC.Get<TTool>());
        }

        public void ShowTool(ITool model)
        {
            if (Tools.Contains(model))
                model.IsVisible = true;
            else
                Tools.Add(model);
            model.IsSelected = true;
            _dockingHost.ActiveLayoutItemBase = model;
        }

        public void CloseDocument(ILayoutItem document)
        {
            _dockingHost.DeactivateItem(document, true);
        }

        public void OpenDocument(ILayoutItem model)
        {
            _dockingHost.ActivateItem(model);
        }
    }
}
