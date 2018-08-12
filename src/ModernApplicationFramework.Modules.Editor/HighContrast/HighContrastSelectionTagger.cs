using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Classification;
using ModernApplicationFramework.Text.Logic.Tagging;
using ModernApplicationFramework.Text.Ui.Classification;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Modules.Editor.HighContrast
{
    internal class HighContrastSelectionTagger : ITagger<HighContrastSelectionTag>, IDisposable
    {
        private readonly ITextView _view;

        private readonly IEditorFormatMap _formatMap;
        private SnapshotSpan _oldSelectedSpan;
        private bool _inHighContrastMode;

        private bool InHighContrastMode
        {
            set
            {
                if (_inHighContrastMode == value)
                    return;
                _inHighContrastMode = value;
                if (value)
                {
                    _view.Selection.SelectionChanged += OnSelectionChanged;
                }
                else
                {
                    _view.Selection.SelectionChanged -= OnSelectionChanged;
                    TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(_view.TextSnapshot, 0, 0)));
                }
            }
            get => _inHighContrastMode;
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public HighContrastSelectionTagger(ITextView view, IClassificationTypeRegistryService classificationRegistry, IEditorFormatMapService editorFormatMapService)
        {
            _view = view;
            _formatMap = editorFormatMapService.GetEditorFormatMap(view);
            HighContrastSelectionTag.Initialize(classificationRegistry);
            InHighContrastMode = SystemParameters.HighContrast;
            _formatMap.FormatMappingChanged += OnFormatMappingChanged;
        }

        private void OnFormatMappingChanged(object sender, FormatItemsEventArgs e)
        {
            InHighContrastMode = SystemParameters.HighContrast;
        }

        private void OnSelectionChanged(object sender, EventArgs e)
        {
            if (!InHighContrastMode)
                return;
            UpdateTags(_view.Selection);
            _oldSelectedSpan = _view.Selection.StreamSelectionSpan.SnapshotSpan;
        }

        private void UpdateTags(ITextSelection selection)
        {
            if (!_oldSelectedSpan.IsEmpty)
            {
                TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(_oldSelectedSpan));
            }
            if (selection.IsEmpty)
                return;
            TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(selection.StreamSelectionSpan.SnapshotSpan));
        }

        public IEnumerable<ITagSpan<HighContrastSelectionTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (InHighContrastMode)
                return InnerGetTags(spans);
            return Enumerable.Empty<ITagSpan<HighContrastSelectionTag>>();
        }

        private IEnumerable<ITagSpan<HighContrastSelectionTag>> InnerGetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (!_view.Selection.IsEmpty)
            {
                NormalizedSnapshotSpanCollection selectedSpans = _view.Selection.SelectedSpans;
                if (selectedSpans.Count > 0)
                {
                    NormalizedSnapshotSpanCollection right = spans.CloneAndTrackTo(selectedSpans[0].Snapshot, SpanTrackingMode.EdgeExclusive);
                    foreach (SnapshotSpan span in NormalizedSnapshotSpanCollection.Overlap(selectedSpans, right))
                        yield return new TagSpan<HighContrastSelectionTag>(span, HighContrastSelectionTag.Instance);
                }
            }
        }

        void IDisposable.Dispose()
        {
            _formatMap.FormatMappingChanged -= OnFormatMappingChanged;
            _view.Selection.SelectionChanged -= OnSelectionChanged;
        }
    }
}