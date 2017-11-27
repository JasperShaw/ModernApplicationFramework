using System.ComponentModel.Composition;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Extended.Layout;
using ModernApplicationFramework.Extended.Utilities.PaneUtilities;

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
