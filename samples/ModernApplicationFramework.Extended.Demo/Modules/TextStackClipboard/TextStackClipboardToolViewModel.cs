using System.ComponentModel.Composition;
using ModernApplicationFramework.Extended.Layout;
using ModernApplicationFramework.Extended.Utilities.PaneUtilities;

namespace ModernApplicationFramework.Extended.Demo.Modules.TextStackClipboard
{
    [Export(typeof(TextStackClipboardToolViewModel))]
    public class TextStackClipboardToolViewModel : Tool
    {
        public override PaneLocation PreferredLocation => PaneLocation.Right;

        public TextStackClipboard Stack { get; }

        public TextStackClipboardToolViewModel()
        {
            Stack = new TextStackClipboard(10);
        }
    }
}
