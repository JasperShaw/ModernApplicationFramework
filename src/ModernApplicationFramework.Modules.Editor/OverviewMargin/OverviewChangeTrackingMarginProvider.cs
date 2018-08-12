using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Ui.Classification;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Tagging;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.OverviewMargin
{
    [Export(typeof(ITextViewMarginProvider))]
    [MarginContainer("VerticalScrollBar")]
    [Name("OverviewChangeTrackingMargin")]
    [ContentType("text")]
    [Order(Before = "OverviewMarkMargin")]
    [Order(Before = "OverviewErrorMargin")]
    [Order(Before = "OverviewSourceImageMargin")]
    [TextViewRole("INTERACTIVE")]
    internal sealed class OverviewChangeTrackingMarginProvider : ITextViewMarginProvider
    {
        [Import]
        internal IViewTagAggregatorFactoryService TagAggregatorFactoryService { get; private set; }

        [Import]
        internal IEditorFormatMapService EditorFormatMapService { get; private set; }

        public ITextViewMargin CreateMargin(ITextViewHost textViewHost, ITextViewMargin parentMargin)
        {
            if (parentMargin is IVerticalScrollBar scrollBar)
                return OverviewChangeTrackingMargin.Create(textViewHost, scrollBar, this);
            return null;
        }
    }
}