using System;
using System.Collections.Generic;
using System.Linq;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Outlining;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    internal class HiddenTextSessionCoordinator
    {
        internal bool HandlingAdaptedOutliningStopped;

        internal SimpleTextViewWindow SimpleTextViewWindow { get; private set; }

        internal ITextView TextView { get; private set; }

        internal IOutliningManager OutliningManager { get; private set; }

        internal IEditorOptions EditorOptions { get; }

        internal HiddenTextSessionAdapter HiddenTextSessionAdapter { get; private set; }

        internal ITextBuffer Buffer => HiddenTextSessionAdapter?.Buffer;

        internal bool HandlingHiddenRegionsChanged { get; set; }

        internal bool HandlingRegionsExpandedOrCollapsed { get; set; }

        internal HiddenTextSessionCoordinator(SimpleTextViewWindow simpleTextViewWindow, ITextView textView, IOutliningManager outliningManager, IEditorOptions editorOptions, TextDocData vsTextDocData)
        {
            if (vsTextDocData == null)
                throw new ArgumentNullException(nameof(vsTextDocData));
            SimpleTextViewWindow = simpleTextViewWindow ?? throw new ArgumentNullException(nameof(simpleTextViewWindow));
            TextView = textView ?? throw new ArgumentNullException(nameof(textView));
            TextView.Closed += OnTextViewClosed;
            OutliningManager = outliningManager ?? throw new ArgumentNullException(nameof(outliningManager));
            OutliningManager.RegionsExpanded += OutliningManager_RegionsExpanded;
            OutliningManager.RegionsCollapsed += OutliningManager_RegionsCollapsed;
            EditorOptions = editorOptions ?? throw new ArgumentNullException(nameof(editorOptions));
            HiddenTextSessionAdapter = HiddenTextManagerAdapter.HiddenTextSessionForTextDocData(vsTextDocData);
            HiddenTextSessionAdapter.AddCoordinator(this);
            HiddenTextSessionAdapter.HiddenRegionExpandedOrCollapsed += VsHiddenTextSessionAdapter_HiddenRegionsExpandedOrCollapsed;
            HiddenTextSessionAdapter.AdaptedOutliningStopped += HiddenTextSessionAdapter_AdaptedOutliningStopped;
            HiddenTextSessionAdapter.AdaptedOutliningStarted += HiddenTextSessionAdapter_AdaptedOutliningStarted;
        }

        internal void OnTextViewClosed(object sender, EventArgs e)
        {
            TextView.Closed -= OnTextViewClosed;
            TextView = null;
            SimpleTextViewWindow = null;
            OutliningManager.RegionsExpanded -= OutliningManager_RegionsExpanded;
            OutliningManager.RegionsCollapsed -= OutliningManager_RegionsCollapsed;
            OutliningManager = null;
            HiddenTextSessionAdapter.HiddenRegionExpandedOrCollapsed -= VsHiddenTextSessionAdapter_HiddenRegionsExpandedOrCollapsed;
            HiddenTextSessionAdapter.AdaptedOutliningStopped -= HiddenTextSessionAdapter_AdaptedOutliningStopped;
            HiddenTextSessionAdapter.AdaptedOutliningStarted -= HiddenTextSessionAdapter_AdaptedOutliningStarted;
            HiddenTextSessionAdapter.RemoveCoordinator(this);
            HiddenTextSessionAdapter = null;
        }

        internal void OutliningManager_RegionsExpanded(object sender, RegionsExpandedEventArgs e)
        {
            HandleRegionsExpandedOrCollapsed(e.ExpandedRegions, false);
        }

        internal void OutliningManager_RegionsCollapsed(object sender, RegionsCollapsedEventArgs e)
        {
            HandleRegionsExpandedOrCollapsed(e.CollapsedRegions, true);
        }

        internal void HandleRegionsExpandedOrCollapsed(IEnumerable<ICollapsible> collapsibles, bool collapsed)
        {
            if (!HiddenTextSessionAdapter.Initialized || HandlingHiddenRegionsChanged || HandlingAdaptedOutliningStopped)
                return;
            if (collapsibles == null)
                return;
            try
            {
                HandlingRegionsExpandedOrCollapsed = true;
                HiddenTextSessionAdapter.HandleExpandedOrCollapsed(collapsibles.Select(collapible => collapible.Extent), collapsed, false, true);
            }
            finally
            {
                HandlingRegionsExpandedOrCollapsed = false;
            }
        }

        internal void VsHiddenTextSessionAdapter_HiddenRegionsExpandedOrCollapsed(object sender, HiddenRegionsExpandedOrCollapsedEventArgs hiddenRegionExpandedOrCollapsedEventArgs)
        {
            if (HandlingRegionsExpandedOrCollapsed || hiddenRegionExpandedOrCollapsedEventArgs == null)
                return;
            var trackingSpans = hiddenRegionExpandedOrCollapsedEventArgs.TrackingSpans;
            if (trackingSpans == null)
                return;
            if (hiddenRegionExpandedOrCollapsedEventArgs.SubjectBufferSpans)
            {
                var textBuffer = TextView.TextBuffer;
                var currentSnapshot = textBuffer.CurrentSnapshot;
                var sortedList = new SortedList<ITrackingSpan, object>(new TrackingSpanComparer(textBuffer));
                foreach (var trackingSpan in trackingSpans)
                {
                    var buffer = TextView.BufferGraph.MapUpToBuffer(trackingSpan.GetSpan(trackingSpan.TextBuffer.CurrentSnapshot), SpanTrackingMode.EdgeExclusive, textBuffer);
                    if (buffer.Count > 0)
                        sortedList.Add(currentSnapshot.CreateTrackingSpan(buffer[0].Span, SpanTrackingMode.EdgeExclusive), SpanTrackingMode.EdgeInclusive);
                }
                trackingSpans = sortedList.Keys;
            }
            //TODO: Undo stuff
            //var flag = !hiddenRegionExpandedOrCollapsedEventArgs.Undoable && EditorOptions.GetOptionValue<bool>(DefaultTextViewOptions.OutliningUndoOptionId);
            //if (flag)
            //    EditorOptions.SetOptionValue<bool>(DefaultTextViewOptions.OutliningUndoOptionId, false);
            //try
            //{
            //    ExpandOrCollapse(trackingSpans, hiddenRegionExpandedOrCollapsedEventArgs.Collapsed);
            //}
            //finally
            //{
            //    if (flag)
            //        EditorOptions.SetOptionValue<bool>(DefaultTextViewOptions.OutliningUndoOptionId, true);
            //}
        }

        internal void HiddenTextSessionAdapter_AdaptedOutliningStopped(object sender, EventArgs eventArgs)
        {
            try
            {
                HandlingAdaptedOutliningStopped = true;
                SimpleTextViewWindow.StopOutlining();
            }
            finally
            {
                HandlingAdaptedOutliningStopped = false;
            }
        }

        internal void HiddenTextSessionAdapter_AdaptedOutliningStarted(object sender, AdaptedOutliningStartedEventArgs adaptedOutliningStartedEventArgs)
        {
            SimpleTextViewWindow.StartOutlining(adaptedOutliningStartedEventArgs.RemoveAdhoc);
        }

        internal void CloneCoordinator(HiddenTextSessionCoordinator vsHiddenTextSessionCoordinator)
        {
            if (vsHiddenTextSessionCoordinator != null && vsHiddenTextSessionCoordinator.TextView.TextBuffer == TextView.TextBuffer)
            {
                var currentSnapshot = vsHiddenTextSessionCoordinator.TextView.TextBuffer.CurrentSnapshot;
                var span = new SnapshotSpan(currentSnapshot, 0, currentSnapshot.Length);
                var collapsedRegions = vsHiddenTextSessionCoordinator.OutliningManager.GetCollapsedRegions(span);
                if (collapsedRegions != null)
                    ExpandOrCollapse(collapsedRegions.Select(collapsed => collapsed.Extent), true);
            }
            //SimpleTextViewWindow.LoadSolutionOpsComplete = true;
        }

        internal void ExpandOrCollapse(IEnumerable<ITrackingSpan> trackingSpans, bool collapse)
        {
            if (trackingSpans == null || !trackingSpans.Any())
                return;
            var trackingSpan1 = trackingSpans.First();
            var trackingSpan2 = trackingSpans.Last();
            if (trackingSpan1 == null || trackingSpan2 == null)
                return;
            var currentSnapshot = TextView.TextBuffer.CurrentSnapshot;
            var collapsibleMatcher = new SortedCollapsibleMatcher(trackingSpans, currentSnapshot);
            var span1 = trackingSpan1.GetSpan(currentSnapshot);
            var snapshotSpan = trackingSpan2 == trackingSpan1 ? span1 : trackingSpan2.GetSpan(currentSnapshot);
            var span2 = new SnapshotSpan(span1.Start, snapshotSpan.End);
            try
            {
                HandlingHiddenRegionsChanged = true;
                if (collapse)
                    OutliningManager.CollapseAll(span2, collapsibleMatcher.Match);
                else
                    OutliningManager.ExpandAll(span2, collapsibleMatcher.Match);
            }
            finally
            {
                HandlingHiddenRegionsChanged = false;
            }
        }

        internal bool IsCollapsed(HiddenRegionAdapter vsHiddenRegionAdapter)
        {
            if (vsHiddenRegionAdapter != null)
            {
                var trackingSpan = vsHiddenRegionAdapter.TrackingSpan;
                var buffer = TextView.BufferGraph.MapUpToBuffer(trackingSpan.GetSpan(trackingSpan.TextBuffer.CurrentSnapshot), SpanTrackingMode.EdgeExclusive, TextView.TextBuffer);
                if (buffer.Count > 0)
                {
                    var collapsedRegions = OutliningManager.GetCollapsedRegions(new SnapshotSpan(buffer[0].Start, buffer[buffer.Count - 1].End));
                    if (collapsedRegions != null)
                        return collapsedRegions.Any(coll =>
                        {
                            if (coll != null)
                                return coll.Tag == vsHiddenRegionAdapter;
                            return false;
                        });
                }
            }
            return false;
        }
    }
}