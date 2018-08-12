using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Data.Projection;
using ModernApplicationFramework.Text.Logic.Classification;
using ModernApplicationFramework.Text.Ui.Classification;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Formatting;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.OverviewMargin
{
    [Export(typeof(ITextViewMarginProvider))]
    [MarginContainer("VerticalScrollBar")]
    [Name("OverviewSourceImageMargin")]
    [Order(After = "OverviewChangeTrackingMargin")]
    [Order(After = "OverviewMarkMargin")]
    [Order(After = "OverviewErrorMargin")]
    [ContentType("text")]
    [TextViewRole("INTERACTIVE")]
    internal sealed class SourceImageMarginFactory : ITextViewMarginProvider
    {
        [Import] internal IBufferGraphFactoryService BufferGraphFactoryService { get; private set; }

        [Import] internal IClassificationFormatMapService ClassificationFormatMappingService { get; private set; }

        [Import] internal IClassifierAggregatorService ClassifierAggregatorService { get; private set; }

        [Import] internal IFormattedTextSourceFactoryService FormattedTextSourceFactoryService { get; private set; }

        public ITextViewMargin CreateMargin(ITextViewHost textViewHost, ITextViewMargin containerMargin)
        {
            if (containerMargin is IVerticalScrollBar scrollBar)
                return new SourceImageMargin(textViewHost, scrollBar, this);
            return null;
        }
    }
}