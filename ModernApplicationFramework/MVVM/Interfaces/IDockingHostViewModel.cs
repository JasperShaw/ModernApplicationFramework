using System;
using ModernApplicationFramework.Caliburn;
using ModernApplicationFramework.Caliburn.Collections;
using ModernApplicationFramework.Caliburn.Interfaces;

namespace ModernApplicationFramework.MVVM.Interfaces
{
    public interface IDockingHostViewModel : IGuardClose, IDeactivate
    {
        event EventHandler ActiveDocumentChanged;
        event EventHandler ActiveDocumentChanging;

        IDocument ActiveItem { get; }

        IObservableCollection<IDocument> Documents { get; }
        IObservableCollection<ITool> Tools { get; }

        ILayoutItem ActiveLayoutItem { get; set; }

        bool ShowFloatingWindowsInTaskbar { get; set; }

        void Close();
        void CloseDocument(IDocument document);

        void OpenDocument(IDocument model);

        void ShowTool<TTool>() where TTool : ITool;

        void ShowTool(ITool model);
    }
}