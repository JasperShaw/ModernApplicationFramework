using System.Collections.Generic;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.OverviewMargin
{
    [Export(typeof(ITextViewMarginProvider))]
    [MarginContainer("VerticalScrollBar")]
    [Name("OverviewMarkMargin")]
    [Order(After = "OverviewChangeTrackingMargin")]
    [Order(Before = "OverviewErrorMargin")]
    [Order(Before = "OverviewSourceImageMargin")]
    [ContentType("text")]
    [TextViewRole("INTERACTIVE")]
    internal sealed class OverviewMarkMarginProvider : MarginProvider
    {
        protected override BaseMarginElement CreateMarginElement(ITextView textView, IVerticalScrollBar scrollbar, MarginProvider provider, List<string> orderedErrorTypes)
        {
            return new MarkMarginElement(textView, scrollbar, provider, orderedErrorTypes);
        }
    }
}
