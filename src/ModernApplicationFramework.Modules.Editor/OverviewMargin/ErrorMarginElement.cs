using System;
using System.Collections.Generic;
using System.Linq;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Tagging;

namespace ModernApplicationFramework.Modules.Editor.OverviewMargin
{
    internal class ErrorMarginElement : BaseMarginElement<IErrorTag>
    {
        public ErrorMarginElement(ITextView textView, IVerticalScrollBar scrollbar, MarginProvider provider, List<string> orderedErrorTypes)
            : base(textView, scrollbar, DefaultTextViewHostOptions.ShowErrorsOptionId, DefaultTextViewHostOptions.ErrorMarginWidthOptionId, "OverviewErrorMargin", "Foreground", provider, orderedErrorTypes)
        {
        }

        protected override IEnumerable<Tuple<string, IMappingSpan>> GetMarksFromTagger(SnapshotSpan span)
        {
            return Aggregator.GetTags(span).Select(tag => new Tuple<string, IMappingSpan>(tag.Tag.ErrorType, tag.Span));
        }
    }
}