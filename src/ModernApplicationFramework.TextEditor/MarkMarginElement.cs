using System;
using System.Collections.Generic;
using System.Linq;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Tagging;
using ModernApplicationFramework.TextEditor.Implementation;

namespace ModernApplicationFramework.TextEditor
{
    internal class MarkMarginElement : BaseMarginElement<IOverviewMarkTag>
    {
        public MarkMarginElement(ITextView textView, IVerticalScrollBar scrollbar, MarginProvider provider, List<string> orderedErrorTypes)
            : base(textView, scrollbar, DefaultTextViewHostOptions.ShowMarksOptionId, DefaultTextViewHostOptions.MarkMarginWidthOptionId, "OverviewMarkMargin", "Background", provider, orderedErrorTypes)
        {
        }

        protected override IEnumerable<Tuple<string, IMappingSpan>> GetMarksFromTagger(SnapshotSpan span)
        {
            return Aggregator.GetTags(span).Select(tag => new Tuple<string, IMappingSpan>(tag.Tag.MarkKindName, tag.Span));
        }
    }
}