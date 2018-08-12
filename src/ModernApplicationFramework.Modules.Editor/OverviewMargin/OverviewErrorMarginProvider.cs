using System.Collections.Generic;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.OverviewMargin
{
    [Export(typeof(ITextViewMarginProvider))]
    [MarginContainer("VerticalScrollBar")]
    [Name("OverviewErrorMargin")]
    [Order(After = "OverviewChangeTrackingMargin")]
    [Order(After = "OverviewMarkMargin")]
    [Order(Before = "OverviewSourceImageMargin")]
    [ContentType("text")]
    [TextViewRole("INTERACTIVE")]
    internal sealed class OverviewErrorMarginProvider : MarginProvider
    {
        protected override BaseMarginElement CreateMarginElement(ITextView textView, IVerticalScrollBar scrollbar,
            MarginProvider provider, List<string> orderedErrorTypes)
        {
            return new ErrorMarginElement(textView, scrollbar, provider, orderedErrorTypes);
        }
    }
}