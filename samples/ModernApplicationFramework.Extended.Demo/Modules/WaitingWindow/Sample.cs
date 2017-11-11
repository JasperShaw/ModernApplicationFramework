using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.Extended.Demo.Modules.SampleExplorer;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Extended.Demo.Modules.WaitingWindow
{
    [Export(typeof(ISample))]
    public class Sample : ISample
    {
        public string Name => "Waiting Dialog Demo";

        public void Activate(IDockingHostViewModel shell)
        {
            shell?.OpenDocument(IoC.Get<WaitingDialogDemoViewModel>());
        }
    }
}
