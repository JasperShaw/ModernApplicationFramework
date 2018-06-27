using System.ComponentModel.Composition;
using ModernApplicationFramework.Extended.Layout;
using ModernApplicationFramework.Extended.Utilities.PaneUtilities;

namespace ModernApplicationFramework.Extended.Demo.Modules.TextStackClipboard
{
    [Export(typeof(TextStackClipboardToolViewModel))]
    public class TextStackClipboardToolViewModel : Tool
    {
        public override PaneLocation PreferredLocation => PaneLocation.Right;

        public TextMruClipboard Stack { get; }

        public TextStackClipboardToolViewModel()
        {
            Stack = new TextMruClipboard(10);
        }
    }
}
