using System.ComponentModel;
using System.ComponentModel.Composition;

namespace ModernApplicationFramework.Extended.Demo.Modules.TestPanel
{
    [DisplayName("TestPanel")]
    [Export(typeof(TestPanelViewModel))]
    public sealed class TestPanelViewModel : Core.LayoutItems.Document
    {
    }
}
