using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.Extended.Demo.Modules.SampleExplorer;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Extended.Demo.Modules.KeyGestureScopeTest
{
    [Export(typeof(ISample))]
    public class Sample : ISample
    {
        public string Name => "Key Gesture Scope Example";

        public void Activate(IDockingHostViewModel shell)
        {
            shell?.OpenLayoutItem(IoC.Get<TextViewModel>());
        }
    }
}
