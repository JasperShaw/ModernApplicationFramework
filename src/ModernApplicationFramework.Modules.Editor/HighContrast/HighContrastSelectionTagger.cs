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
        private readonly IEditorFormatMap _formatMap;
        private readonly ITextView _view;
        private bool _inHighContrastMode;
        private SnapshotSpan _oldSelectedSpan;

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

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

        public HighContrastSelectionTagger(ITextView view, IClassificationTypeRegistryService classificationRegistry,
            IEditorFormatMapService editorFormatMapService)
        {
            _view = view;
            _formatMap = editorFormatMapService.GetEditorFormatMap(view);
            HighContrastSelectionTag.Initialize(classificationRegistry);
            InHighContrastMode = SystemParameters.HighContrast;
            _formatMap.FormatMappingChanged += OnFormatMappingChanged;
        }

        public IEnumerable<ITagSpan<HighContrastSelectionTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (InHighContrastMode)
                return InnerGetTags(spans);
            return Enumerable.Empty<ITagSpan<HighContrastSelectionTag>>();
        }

        void IDisposable.Dispose()
        {
            _formatMap.FormatMappingChanged -= OnFormatMappingChanged;
            _view.Selection.SelectionChanged -= OnSelectionChanged;
        }

        private IEnumerable<ITagSpan<HighContrastSelectionTag>> InnerGetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (!_view.Selection.IsEmpty)
            {
                var selectedSpans = _view.Selection.SelectedSpans;
                if (selectedSpans.Count > 0)
                {
                    var right = spans.CloneAndTrackTo(selectedSpans[0].Snapshot, SpanTrackingMode.EdgeExclusive);
                    foreach (var span in NormalizedSnapshotSpanCollection.Overlap(selectedSpans, right))
                        yield return new TagSpan<HighContrastSelectionTag>(span, HighContrastSelectionTag.Instance);
                }
            }
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
            if (!_oldSelectedSpan.IsEmpty) TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(_oldSelectedSpan));
            if (selection.IsEmpty)
                return;
            TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(selection.StreamSelectionSpan.SnapshotSpan));
        }
    }
}