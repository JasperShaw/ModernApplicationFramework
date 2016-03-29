using System;
using Caliburn.Micro;

namespace ModernApplicationFramework.MVVM.Interfaces
{
    public interface IDockingHostViewModel : IGuardClose, IDeactivate
    {
        event EventHandler ActiveDocumentChanged;
        event EventHandler ActiveDocumentChanging;

        bool ShowFloatingWindowsInTaskbar { get; set; }

        IDocument ActiveItem { get; }

        IObservableCollection<IDocument> Documents { get; }
        IObservableCollection<ITool> Tools { get; }

        ILayoutItem ActiveLayoutItem { get; set; }

        void Close();
        void CloseDocument(IDocument document);

        void OpenDocument(IDocument model);

        void ShowTool<TTool>() where TTool : ITool;

        void ShowTool(ITool model);
    }
}