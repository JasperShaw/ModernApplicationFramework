using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Tagging;
using ModernApplicationFramework.Text.Ui.Classification;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Outlining;

namespace ModernApplicationFramework.Modules.Editor.Implementation
{
    internal class CollapsedAdornmentProvider : ITagger<IntraTextAdornmentTag>
    {
        private readonly ResourceDictionary _resources = new ResourceDictionary();
        private List<AdornmentRecord> _adornments = new List<AdornmentRecord>();
        private readonly ITextView _view;
        private readonly IOutliningManager _outliningManager;
        private readonly IEditorFormatMap _editorFormatMap;
        private readonly IClassificationFormatMap _classificationFormatMap;
        private readonly IViewPrimitives _viewPrimitives;
        private bool _useLayoutRounding;
        private bool _snapsToDevicePixels;
        private TextFormattingMode _textFormattingMode;
        private TextHintingMode _textHintingMode;
        private TextRenderingMode _textRenderingMode;

        internal CollapsedAdornmentProvider(ITextView textView, IOutliningManager outliningManager, IEditorFormatMap editorFormatMap, IClassificationFormatMap classificationFormatMap, IViewPrimitives viewPrimitives)
        {
            _view = textView ?? throw new ArgumentNullException(nameof(textView));
            _outliningManager = outliningManager ?? throw new ArgumentNullException(nameof(outliningManager));
            _editorFormatMap = editorFormatMap;
            _classificationFormatMap = classificationFormatMap;
            UpdateFromFormat();
            UpdateFromClassifications();
            _outliningManager.RegionsExpanded += HandleRegionsExpanded;
            _outliningManager.RegionsCollapsed += HandleRegionsCollapsed;
            _outliningManager.RegionsChanged += HandleRegionsChanged;
            _view.LayoutChanged += HandleLayoutChanged;
            _view.Closed += HandleViewClosed;
            _editorFormatMap.FormatMappingChanged += HandleFormatMappingChanged;
            _classificationFormatMap.ClassificationFormatMappingChanged += HandleClassificationFormatChanged;
            _viewPrimitives = viewPrimitives;
            _view.VisualElement.Resources.MergedDictionaries.Add(_resources);
            UpdateViewLayoutOptionsCache(_view.VisualElement);
        }

        private void HandleViewClosed(object sender, EventArgs args)
        {
            _classificationFormatMap.ClassificationFormatMappingChanged -= HandleClassificationFormatChanged;
            _editorFormatMap.FormatMappingChanged -= HandleFormatMappingChanged;
            _view.Closed -= HandleViewClosed;
            _view.LayoutChanged -= HandleLayoutChanged;
            _outliningManager.RegionsExpanded -= HandleRegionsExpanded;
            _outliningManager.RegionsCollapsed -= HandleRegionsCollapsed;
            _outliningManager.RegionsChanged -= HandleRegionsChanged;
        }

        private void HandleClassificationFormatChanged(object sender, EventArgs args)
        {
            UpdateFromClassifications();
        }

        private void HandleFormatMappingChanged(object sender, FormatItemsEventArgs args)
        {
            if (!args.ChangedItems.Contains("Collapsible Text (Collapsed)"))
                return;
            UpdateFromFormat();
        }

        private void UpdateFromFormat()
        {
            var properties = _editorFormatMap.GetProperties("Collapsible Text (Collapsed)");
            Brush brush = null;
            if (properties != null)
                brush = (Brush)properties["Foreground"];
            _resources["CollapsedTextForeground"] = brush;
        }

        private void UpdateFromClassifications()
        {
            var defaultTextProperties = _classificationFormatMap.DefaultTextProperties;
            _resources["CollapsedTextFontFamily"] = defaultTextProperties.Typeface.FontFamily;
            _resources["CollapsedTextSize"] = defaultTextProperties.FontRenderingEmSize;
        }

        private void HandleRegionsExpanded(object sender, RegionsExpandedEventArgs args)
        {
            var textSnapshot = _view.TextSnapshot;
            var start = new SnapshotPoint(textSnapshot, textSnapshot.Length);
            var end = new SnapshotPoint(textSnapshot, 0);
            foreach (var expandedRegion in args.ExpandedRegions)
            {
                var span = expandedRegion.Extent.GetSpan(textSnapshot);
                if (span.Start < start)
                    start = span.Start;
                if (span.End > end)
                    end = span.End;
            }
            RaiseTagsChanged(new SnapshotSpan(start, end));
        }

        private void HandleRegionsCollapsed(object sender, RegionsCollapsedEventArgs args)
        {
            var textSnapshot = _view.TextSnapshot;
            var start = new SnapshotPoint(textSnapshot, textSnapshot.Length);
            var end = new SnapshotPoint(textSnapshot, 0);
            foreach (var collapsedRegion in args.CollapsedRegions)
            {
                var span = collapsedRegion.Extent.GetSpan(textSnapshot);
                if (span.Start < start)
                    start = span.Start;
                if (span.End > end)
                    end = span.End;
            }
            RaiseTagsChanged(new SnapshotSpan(start, end));
        }

        private void HandleRegionsChanged(object sender, RegionsChangedEventArgs args)
        {
            RaiseTagsChanged(args.AffectedSpan);
        }

        private void RemoveOffscreenAdornments()
        {
            var formattedSpan = _view.TextViewLines.FormattedSpan;
            var snapshot = formattedSpan.Snapshot;
            var index1 = 0;
            for (var index2 = 0; index2 < _adornments.Count; ++index2)
            {
                var adornment = _adornments[index2];
                if (formattedSpan.IntersectsWith(adornment.SourceSpan.GetSpan(snapshot)))
                {
                    if (index2 != index1)
                        _adornments[index1] = adornment;
                    ++index1;
                }
            }
            if (index1 >= _adornments.Count)
                return;
            _adornments.RemoveRange(index1, _adornments.Count - index1);
            _adornments.TrimExcess();
        }

        private void HandleLayoutChanged(object sender, TextViewLayoutChangedEventArgs args)
        {
            if (_adornments.Count != 0)
                RemoveOffscreenAdornments();
            if (!AreViewLayoutOptionsChanged(_view.VisualElement))
                return;
            UpdateViewLayoutOptionsCache(_view.VisualElement);
            foreach (var adornment in _adornments)
                SetLayoutOptions(adornment.Ui);
        }

        private void UpdateViewLayoutOptionsCache(FrameworkElement view)
        {
            _snapsToDevicePixels = view.SnapsToDevicePixels;
            _useLayoutRounding = view.UseLayoutRounding;
            _textRenderingMode = TextOptions.GetTextRenderingMode(view);
            _textHintingMode = TextOptions.GetTextHintingMode(view);
            _textFormattingMode = TextOptions.GetTextFormattingMode(view);
        }

        private bool AreViewLayoutOptionsChanged(FrameworkElement view)
        {
            if (view.SnapsToDevicePixels == _snapsToDevicePixels && view.UseLayoutRounding == _useLayoutRounding && (TextOptions.GetTextRenderingMode(view) == _textRenderingMode && TextOptions.GetTextHintingMode(view) == _textHintingMode))
                return TextOptions.GetTextFormattingMode(view) != _textFormattingMode;
            return true;
        }

        private void SetLayoutOptions(CollapsedAdornment adornment)
        {
            adornment.UseLayoutRounding = _useLayoutRounding;
            adornment.SnapsToDevicePixels = _snapsToDevicePixels;
            TextOptions.SetTextFormattingMode(adornment, _textFormattingMode);
            TextOptions.SetTextHintingMode(adornment, _textHintingMode);
            TextOptions.SetTextRenderingMode(adornment, _textRenderingMode);
        }

        private CollapsedAdornment MakeAdornment(ICollapsed collapsed)
        {
            var adornment = new CollapsedAdornment(_viewPrimitives, collapsed, _outliningManager);
            SetLayoutOptions(adornment);
            adornment.Resources.MergedDictionaries.Add(_resources);
            adornment.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            return adornment;
        }

        public IEnumerable<ITagSpan<IntraTextAdornmentTag>> GetTags(NormalizedSnapshotSpanCollection sourceSpans)
        {
            if (sourceSpans == null)
                throw new ArgumentNullException(nameof(sourceSpans));
            if (sourceSpans.Count == 0)
                yield break;
            var sourceSnapshot = sourceSpans[0].Snapshot;
            var collapsedRegions = GetCollapsedRegions(sourceSpans);
            var dictionary = new Dictionary<SnapshotSpan, ICollapsed>(_adornments.Count + 10);
            foreach (var collapsed in collapsedRegions)
            {
                var span = collapsed.Extent.GetSpan(sourceSnapshot);
                if (!dictionary.ContainsKey(span))
                    dictionary.Add(span, collapsed);
            }
            var adornmentRecordList = new List<AdornmentRecord>(_adornments.Count + 10);
            foreach (var adornment in _adornments)
            {
                var span = adornment.SourceSpan.GetSpan(sourceSnapshot);
                var flag = dictionary.TryGetValue(span, out var collapsed);
                if (!sourceSpans.IntersectsWith(span) || flag)
                {
                    UpdateAdornment(adornment.Ui, collapsed);
                    dictionary.Remove(span);
                    adornmentRecordList.Add(adornment);
                }
            }
            foreach (var keyValuePair in dictionary)
            {
                var trackingSpan = sourceSnapshot.CreateTrackingSpan(keyValuePair.Key, SpanTrackingMode.EdgeExclusive);
                var collapsedAdornment = MakeAdornment(keyValuePair.Value);
                adornmentRecordList.Add(new AdornmentRecord()
                {
                    SourceSpan = trackingSpan,
                    Ui = collapsedAdornment
                });
            }
            _adornments = adornmentRecordList;
            foreach (var adornment in _adornments)
            {
                var span = adornment.SourceSpan.GetSpan(sourceSnapshot);
                if (sourceSpans.IntersectsWith(span))
                {
                    var heightAboveBaseline = _view.FormattedLineSource.TextHeightAboveBaseline;
                    var num = heightAboveBaseline + _view.FormattedLineSource.TextHeightBelowBaseline;
                    var tag = new IntraTextAdornmentTag(adornment.Ui, null, 0.0, heightAboveBaseline, num, 0.0, new PositionAffinity?());
                    yield return new TagSpan<IntraTextAdornmentTag>(span, tag);
                }
            }
        }

        private IEnumerable<ICollapsed> GetCollapsedRegions(NormalizedSnapshotSpanCollection sourceSpans)
        {
            var snapshot = sourceSpans[0].Snapshot;
            return _outliningManager.GetCollapsedRegions(sourceSpans, true).ToList();
        }

        private static void UpdateAdornment(CollapsedAdornment adornment, ICollapsible collapsed)
        {
            if (collapsed == null)
                return;
            adornment.Content = collapsed.CollapsedForm;
            if (!adornment.IsToolTipSet)
                return;
            adornment.ToolTip = collapsed.CollapsedHintForm;
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        private void RaiseTagsChanged(SnapshotSpan sourceSpan)
        {
            // ISSUE: reference to a compiler-generated field
            var tagsChanged = TagsChanged;
            tagsChanged?.Invoke(this, new SnapshotSpanEventArgs(sourceSpan));
        }

        private struct AdornmentRecord
        {
            public ITrackingSpan SourceSpan;
            public CollapsedAdornment Ui;
        }
    }
}
