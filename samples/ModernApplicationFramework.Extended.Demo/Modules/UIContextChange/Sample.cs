using System.ComponentModel.Composition;
using ModernApplicationFramework.Extended.Demo.Modules.SampleExplorer;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Extended.Demo.Modules.UIContextChange
{
    [Export(typeof(ISample))]
    public class Sample : ISample
    {
        public string Name => "UI Context Change";

        public void Activate(IDockingHostViewModel shell)
        {
            shell?.ShowTool<TriggerUiContextViewModel>();
        }
    }
}
