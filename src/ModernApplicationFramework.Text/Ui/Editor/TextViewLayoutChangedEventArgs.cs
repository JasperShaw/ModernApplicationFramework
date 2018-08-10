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

        public bool HorizontalTranslation => OldViewState.ViewportLeft != NewViewState.ViewportLeft;

        public ReadOnlyCollection<ITextViewLine> NewOrReformattedLines { get; }

        public NormalizedSnapshotSpanCollection NewOrReformattedSpans
        {
            get
            {
                if (_newOrReformattedSpans == null)
                    _newOrReformattedSpans = GetSpans(NewOrReformattedLines);
                return _newOrReformattedSpans;
            }
        }

        public ITextSnapshot NewSnapshot => NewViewState.EditSnapshot;

        public ViewState NewViewState { get; }

        public ITextSnapshot OldSnapshot => OldViewState.EditSnapshot;

        public ViewState OldViewState { get; }

        public ReadOnlyCollection<ITextViewLine> TranslatedLines { get; }

        public NormalizedSnapshotSpanCollection TranslatedSpans
        {
            get
            {
                if (_translatedSpans == null)
                    _translatedSpans = GetSpans(TranslatedLines);
                return _translatedSpans;
            }
        }

        public bool VerticalTranslation => OldViewState.ViewportTop != NewViewState.ViewportTop;

        public TextViewLayoutChangedEventArgs(ViewState oldState, ViewState newState,
            IList<ITextViewLine> newOrReformattedLines, IList<ITextViewLine> translatedLines)
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

        private static NormalizedSnapshotSpanCollection GetSpans(IList<ITextViewLine> lines)
        {
            if (lines.Count <= 0)
                return NormalizedSnapshotSpanCollection.Empty;
            var spanList = lines.Select(line => line.ExtentIncludingLineBreak.Span).ToList();
            return new NormalizedSnapshotSpanCollection(lines[0].Snapshot, spanList);
        }
    }
}