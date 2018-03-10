using System.ComponentModel.Composition;
using System.Windows.Media;
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

        public override ImageSource IconSource => null;
    }

    public interface ITestPrinter : ITool
    {
        
    }
}
