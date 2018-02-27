using System;
using Caliburn.Micro;

namespace ModernApplicationFramework.Extended.Interfaces
{
    public interface IDockingHostViewModel : IGuardClose, IDeactivate, IConductor
    {
        event EventHandler ActiveDocumentChanged;

        event EventHandler ActiveDocumentChanging;

        ILayoutItem ActiveItem { get; }

        IDockingHost DockingHostView { get; }

        IObservableCollection<ILayoutItem> Documents { get; }
        IObservableCollection<ITool> Tools { get; }

        ILayoutItemBase ActiveLayoutItemBase { get; set; }

        bool ShowFloatingWindowsInTaskbar { get; set; }

        void Close();

        void CloseLayoutItem(ILayoutItem document);

        void OpenLayoutItem(ILayoutItem model);

        void ShowTool<TTool>() where TTool : ITool;

        void ShowTool(ITool model);

        void HideTool<TTool>(bool remove) where TTool : ITool;

        void HideTool(ITool model, bool remove);
    }
}