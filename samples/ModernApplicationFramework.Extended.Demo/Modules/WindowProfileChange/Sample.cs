using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.Extended.Demo.Modules.SampleExplorer;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Extended.Demo.Modules.WindowProfileChange
{
    [Export(typeof(ISample))]
    public class Sample : ISample
    {
        public string Name => "Window Profile Change";

        public void Activate(IDockingHostViewModel shell)
        {
            shell?.OpenLayoutItem(IoC.Get<WindowProfileChangeViewModel>());
        }
    }
}
