using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.OverviewMargin
{
    [Export(typeof(ITextViewMarginProvider))]
    [Name("OverviewScrollBarMargin")]
    [MarginContainer("VerticalScrollBar")]
    [ContentType("text")]
    [TextViewRole("INTERACTIVE")]
    internal sealed class OverviewScrollBarMarginProvider : ITextViewMarginProvider
    {
        [Import]
        private OverviewElementFactory _overviewElementFactory;

        public ITextViewMargin CreateMargin(ITextViewHost textViewHost, ITextViewMargin marginContainer)
        {
            return new OverviewScrollBarMargin(textViewHost, _overviewElementFactory);
        }
    }
}
