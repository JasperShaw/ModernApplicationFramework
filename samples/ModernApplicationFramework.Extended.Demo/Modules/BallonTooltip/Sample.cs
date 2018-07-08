using System.ComponentModel.Composition;
using ModernApplicationFramework.Extended.Demo.Modules.SampleExplorer;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Extended.Demo.Modules.BallonTooltip
{
    [Export(typeof(ISample))]
    public class Sample : ISample
    {
        public string Name => "Ballon Tooltip";

        public void Activate(IDockingHostViewModel shell)
        {
            shell?.ShowTool<BallonTooltipViewModel>();
        }
    }
}
