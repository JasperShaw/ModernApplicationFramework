using System.ComponentModel.Composition;
using ModernApplicationFramework.Extended.Demo.Modules.SampleExplorer;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Extended.Demo.Modules.TextStackClipboard
{
    [Export(typeof(ISample))]
    public class Sample : ISample
    {
        public string Name => "Text Stack Clipboard";

        public void Activate(IDockingHostViewModel shell)
        {
            shell?.ShowTool<TextStackClipboardToolViewModel>();
        }
    }
}
