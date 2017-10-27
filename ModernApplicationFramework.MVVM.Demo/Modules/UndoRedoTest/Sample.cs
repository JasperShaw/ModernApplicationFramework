using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.MVVM.Demo.Modules.Document;

namespace ModernApplicationFramework.MVVM.Demo.Modules.UndoRedoTest
{
    [Export(typeof(ISample))]
    public class Sample : ISample
    {
        public string Name => "Undo-Redo View";

        public void Activate(IDockingHostViewModel shell)
        {
            shell?.OpenDocument(IoC.Get<UndoRedoViewModel>());
        }
    }
}
