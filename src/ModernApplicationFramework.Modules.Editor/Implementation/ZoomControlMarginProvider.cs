using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.Implementation
{
    [Name("ZoomControl")]
    [Export(typeof(ITextViewMarginProvider))]
    [Order(Before = "HorizontalScrollBarContainer")]
    [MarginContainer("BottomControl")]
    [ContentType("text")]
    [TextViewRole("ZOOMABLE")]
    internal sealed class ZoomControlMarginProvider : ITextViewMarginProvider
    {
        public ITextViewMargin CreateMargin(ITextViewHost wpfTextViewHost, ITextViewMargin marginContainer)
        {
            return new ZoomControlMargin(wpfTextViewHost.TextView);
        }
    }
}
