using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Tagging;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Tagging;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.OverviewMargin
{
    [Export(typeof(IViewTaggerProvider))]
    [TagType(typeof(ITextMarkerTag))]
    [TextViewRole("ENHANCED_SCROLLBAR_PREVIEW")]
    [ContentType("any")]
    internal class PreviewTextMarkerTaggerProvider : IViewTaggerProvider
    {
        [Import]
        internal IViewTagAggregatorFactoryService TagAggregatorFactoryService { get; set; }

        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            if (textView.TextBuffer == buffer)
            {
                if (textView.TextViewModel is PreviewTextViewModel textViewModel)
                    return new PreviewTextMarkerTagger(textViewModel.SourceView, textView, TagAggregatorFactoryService.CreateTagAggregator<ITextMarkerTag>(textViewModel.SourceView)) as ITagger<T>;
            }
            return null;
        }
    }
}
