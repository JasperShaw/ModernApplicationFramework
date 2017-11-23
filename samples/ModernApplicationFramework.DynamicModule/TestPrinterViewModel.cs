using System.ComponentModel.Composition;
using ModernApplicationFramework.Extended.Core.LayoutItems;
using ModernApplicationFramework.Extended.Core.Pane;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.DynamicModule
{
    [Export(typeof(ITestPrinter))]
    public sealed class TestPrinterViewModel : Tool, ITestPrinter
    {
        public override PaneLocation PreferredLocation => PaneLocation.Left;

        public TestPrinterViewModel()
        {
            DisplayName = "TestPrinter";
        }
    }

    public interface ITestPrinter : ITool
    {
        
    }
}
