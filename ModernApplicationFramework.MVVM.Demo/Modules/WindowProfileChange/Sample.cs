using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.MVVM.Demo.Modules.SampleExplorer;

namespace ModernApplicationFramework.MVVM.Demo.Modules.WindowProfileChange
{
    [Export(typeof(ISample))]
    public class Sample : ISample
    {
        public string Name => "Window Profile Change";

        public void Activate(IDockingHostViewModel shell)
        {
            shell?.OpenDocument(IoC.Get<WindowProfileChangeViewModel>());
        }
    }
}
