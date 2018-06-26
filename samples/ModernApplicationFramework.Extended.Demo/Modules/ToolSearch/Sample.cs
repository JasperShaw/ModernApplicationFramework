using System.ComponentModel.Composition;
using ModernApplicationFramework.Extended.Demo.Modules.SampleExplorer;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Extended.Demo.Modules.ToolSearch
{
    [Export(typeof(ISample))]
    public class Sample : ISample
    {
        public string Name => "Search Tool Window";

        public void Activate(IDockingHostViewModel shell)
        {
            shell?.ShowTool<ToolSearchExampleViewModel>();
        }
    }
}
