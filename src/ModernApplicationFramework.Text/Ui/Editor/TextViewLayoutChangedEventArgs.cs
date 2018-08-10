using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Formatting;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public class TextViewLayoutChangedEventArgs : EventArgs
    {
        private NormalizedSnapshotSpanCollection _newOrReformattedSpans;
        private NormalizedSnapshotSpanCollection _translatedSpans;

        private static NormalizedSnapshotSpanCollection GetSpans(IList<ITextViewLine> lines)
        {
            if (lines.Count <= 0)
                return NormalizedSnapshotSpanCollection.Empty;
            var spanList = lines.Select(line => line.ExtentIncludingLineBreak.Span).ToList();
            return new NormalizedSnapshotSpanCollection(lines[0].Snapshot, spanList);
        }

        public TextViewLayoutChangedEventArgs(ViewState oldState, ViewState newState, IList<ITextViewLine> newOrReformattedLines, IList<ITextViewLine> translatedLines)
        {
            if (translatedLines == null)
                throw new ArgumentNullException(nameof(translatedLines));
            if (newOrReformattedLines == null)
                throw new ArgumentNullException(nameof(newOrReformattedLines));
            OldViewState = oldState ?? throw new ArgumentNullException(nameof(oldState));
            NewViewState = newState ?? throw new ArgumentNullException(nameof(newState));
            NewOrReformattedLines = new ReadOnlyCollection<ITextViewLine>(newOrReformattedLines);
            TranslatedLines = new ReadOnlyCollection<ITextViewLine>(translatedLines);
        }

        public ViewState OldViewState { get; }

        public ViewState NewViewState { get; }

        public bool HorizontalTranslation => OldViewState.ViewportLeft != NewViewState.ViewportLeft;

        public bool VerticalTranslation => OldViewState.ViewportTop != NewViewState.ViewportTop;

        public ITextSnapshot OldSnapshot => OldViewState.EditSnapshot;

        public ITextSnapshot NewSnapshot => NewViewState.EditSnapshot;

        public ReadOnlyCollection<ITextViewLine> NewOrReformattedLines { get; }

        public ReadOnlyCollection<ITextViewLine> TranslatedLines { get; }

        public NormalizedSnapshotSpanCollection NewOrReformattedSpans
        {
            get
            {
                if (_newOrReformattedSpans == null)
                    _newOrReformattedSpans = GetSpans(NewOrReformattedLines);
                return _newOrReformattedSpans;
            }
        }

        public NormalizedSnapshotSpanCollection TranslatedSpans
        {
            get
            {
                if (_translatedSpans == null)
                    _translatedSpans = GetSpans(TranslatedLines);
                return _translatedSpans;
            }
        }
    }
}