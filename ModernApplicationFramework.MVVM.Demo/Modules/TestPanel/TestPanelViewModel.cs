using System.ComponentModel;
using System.ComponentModel.Composition;

namespace ModernApplicationFramework.MVVM.Demo.Modules.TestPanel
{
    [DisplayName("TestPanel")]
    [Export(typeof(TestPanelViewModel))]
    public sealed class TestPanelViewModel : Controls.Document
    {
    }
}
