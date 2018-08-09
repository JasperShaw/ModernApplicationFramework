using System;
using System.Collections.Generic;
using ModernApplicationFramework.TextEditor.Text;

namespace ModernApplicationFramework.TextEditor.Implementation.Outlining
{
    internal class AdhocOutliner
    {
        public ITextView TextView { get; private set; }

        public IOutliningManager OutliningManager { get; private set; }

        private AdhocOutliner(ITextView view, IOutliningManager outliningManager)
        {
            TextView = view;
            OutliningManager = outliningManager;
            TextView.Closed += OnTextViewClosed;
        }

        private void OnTextViewClosed(object sender, EventArgs e)
        {
            TextView.Closed -= OnTextViewClosed;
            TextView = null;
            OutliningManager = null;
        }

        public static AdhocOutliner GetOutlinerForView(ITextView view, IOutliningManager outliningManager)
        {
            return view.Properties.GetOrCreateSingletonProperty(typeof(AdhocOutliner), () => new AdhocOutliner(view, outliningManager));
        }

        //TODO: Undo stuff

        //public void AddRegion(ITextUndoHistory history, SnapshotSpan spanToHide)
        //{
        //    spanToHide = TrimTrailingLineBreak(spanToHide);
        //    SimpleTagger<IOutliningRegionTag> outliningTagger = AdhocOutliningTaggerProvider.GetOutliningTagger(TextView.TextBuffer);
        //    if (spanToHide.IsEmpty || OutliningManager == null || outliningTagger == null)
        //        return;
        //    AdhocOutlinerUndoPrimitive outlinerUndoPrimitive = new AdhocOutlinerUndoPrimitive(this, AdhocOutlinerAction.AddRegions, (IList<Span>)new List<Span>(new Span[1]
        //    {
        //        spanToHide
        //    }));
        //    using (ITextUndoTransaction transaction = history.CreateTransaction(Microsoft.VisualStudio.Editor.Implementation.Strings.HideRegionUndoTitle))
        //    {
        //        outlinerUndoPrimitive.Do();
        //        transaction.AddUndo((ITextUndoPrimitive)outlinerUndoPrimitive);
        //        transaction.Complete();
        //    }
        //}

        //public void RemoveRegions(ITextUndoHistory history, SnapshotSpan spanToRemove)
        //{
        //    SimpleTagger<IOutliningRegionTag> outliningTagger = AdhocOutliningTaggerProvider.GetOutliningTagger(TextView.TextBuffer);
        //    if (outliningTagger == null || OutliningManager == null)
        //        return;
        //    IList<Span> spansToRemove = DetermineSpansToRemove(outliningTagger.GetTaggedSpans(spanToRemove), spanToRemove);
        //    if (spansToRemove.Count == 0)
        //        return;
        //    AdhocOutlinerUndoPrimitive outlinerUndoPrimitive = new AdhocOutlinerUndoPrimitive(this, AdhocOutlinerAction.RemoveRegions, spansToRemove);
        //    string description = spansToRemove.Count == 1 ? Microsoft.VisualStudio.Editor.Implementation.Strings.StopHidingRegionUndoTitle : Microsoft.VisualStudio.Editor.Implementation.Strings.StopHidingRegionsUndoTitle;
        //    using (ITextUndoTransaction transaction = history.CreateTransaction(description))
        //    {
        //        outlinerUndoPrimitive.Do();
        //        transaction.AddUndo((ITextUndoPrimitive)outlinerUndoPrimitive);
        //        transaction.Complete();
        //    }
        //}

        private static IList<Span> DetermineSpansToRemove(IEnumerable<TrackingTagSpan<IOutliningRegionTag>> trackingTagSpans, SnapshotSpan spanToRemove)
        {
            List<Span> spanList1 = new List<Span>();
            List<Span> spanList2 = new List<Span>();
            Span? nullable = new Span?();
            ITextSnapshot snapshot = spanToRemove.Snapshot;
            foreach (TrackingTagSpan<IOutliningRegionTag> trackingTagSpan in trackingTagSpans)
            {
                SnapshotSpan span = trackingTagSpan.Span.GetSpan(snapshot);
                if (spanToRemove.Contains(span))
                    spanList1.Add(span);
                else if (span.Contains(spanToRemove))
                {
                    if (!nullable.HasValue || span.Length < nullable.Value.Length)
                        nullable = new Span?(span);
                }
                else
                    spanList2.Add(span);
            }
            if (spanList1.Count > 0)
                return spanList1;
            if (spanList2.Count > 0)
                return spanList2;
            if (!nullable.HasValue)
                return new List<Span>(0);
            return new List<Span>(new Span[1]
            {
                nullable.Value
            });
        }

        private static SnapshotSpan TrimTrailingLineBreak(SnapshotSpan span)
        {
            ITextSnapshotLine lineFromPosition1 = span.Snapshot.GetLineFromPosition(span.End);
            if (span.End == lineFromPosition1.Start && lineFromPosition1.Start > 0)
            {
                ITextSnapshotLine lineFromPosition2 = span.Snapshot.GetLineFromPosition(lineFromPosition1.Start - 1);
                if (span.Start <= lineFromPosition2.End)
                    span = new SnapshotSpan(span.Start, lineFromPosition2.End);
            }
            return span;
        }
    }
}