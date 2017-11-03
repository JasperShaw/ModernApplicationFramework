using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.Extended.Demo.Modules.SampleExplorer;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Extended.Demo.Modules.InfoBarDemo
{
    [Export(typeof(ISample))]
    public class Sample : ISample
    {
        public string Name => "Info Bar Demo";

        public void Activate(IDockingHostViewModel shell)
        {
            shell?.OpenDocument(IoC.Get<InfoBarDemoViewModel>());
        }
    }
}
