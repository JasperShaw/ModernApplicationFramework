using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Tagging;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Tagging;

namespace ModernApplicationFramework.Modules.Editor.IntraTextAdornmentSupport
{
    internal class IntraTextAdornmentManager
    {
        internal static readonly DependencyProperty ScalingElementContainerProperty = DependencyProperty.RegisterAttached("ScalingElementContainer", typeof(ScalingElementContainer), typeof(UIElement));
        private readonly List<IMappingTagSpan<IntraTextAdornmentTag>> _displayedAdornments = new List<IMappingTagSpan<IntraTextAdornmentTag>>();
        private IAdornmentLayer _layer;
        private readonly ITextView _view;
        private readonly ITagAggregator<IntraTextAdornmentTag> _adornmentTagAggregator;
        private readonly SpaceNegotiationTaggerType _spaceNegotiationTagger;
        private readonly HiddenRegionTaggerType _hiddenRegionTagger;
        private bool _listeningOnSelection;

        internal IntraTextAdornmentManager(ITextView view, ITagAggregator<IntraTextAdornmentTag> adornmentTags)
        {
            _spaceNegotiationTagger = new SpaceNegotiationTaggerType(this);
            _hiddenRegionTagger = new HiddenRegionTaggerType(this);
            _adornmentTagAggregator = adornmentTags;
            _view = view;
            RegisterForEvents();
            _view.Closed += HandleViewClosed;
        }

        private IAdornmentLayer AdornmentLayer => _layer ?? (_layer = _view.GetAdornmentLayer("Intra Text Adornment"));

        private void HandleViewClosed(object sender, EventArgs args)
        {
            _view.Closed -= HandleViewClosed;
            UnregisterFromEvents();
            _adornmentTagAggregator.Dispose();
        }

        private void UnregisterFromEvents()
        {
            _view.LayoutChanged -= HandleLayoutChanged;
            _view.Selection.SelectionChanged -= HandleSelectionChanged;
            _adornmentTagAggregator.TagsChanged -= HandleTagsChanged;
        }

        private void RegisterForEvents()
        {
            _view.LayoutChanged += HandleLayoutChanged;
            _adornmentTagAggregator.TagsChanged += HandleTagsChanged;
        }

        private void HandleSelectionChanged(object sender, EventArgs args)
        {
            foreach (var displayedAdornment in _displayedAdornments)
            {
                var singleSpan = GetSingleSpan(displayedAdornment.Span.GetSpans(_view.TextSnapshot));
                var containingBufferPosition = _view.TextViewLines.GetTextViewLineContainingBufferPosition(singleSpan.Start);
                if (containingBufferPosition != null)
                {
                    var selectionOnTextViewLine = _view.Selection.GetSelectionOnTextViewLine(containingBufferPosition);
                    var isSelected = selectionOnTextViewLine.HasValue && selectionOnTextViewLine.Value.SnapshotSpan.Contains(singleSpan);
                    IntraTextAdornment.SetIsSelected(displayedAdornment.Tag.Adornment, isSelected);
                }
            }
        }

        private static SnapshotSpan GetSingleSpan(NormalizedSnapshotSpanCollection spans)
        {
            return spans[0];
        }

        private void HandleLayoutChanged(object sender, TextViewLayoutChangedEventArgs layoutChangedArgs)
        {
            if (_view.Selection != null && !_listeningOnSelection)
            {
                _view.Selection.SelectionChanged += HandleSelectionChanged;
                _listeningOnSelection = true;
            }
            var reformattedLines = layoutChangedArgs.NewOrReformattedLines;
            foreach (var line in reformattedLines)
            {
                if (line != null && line.IsValid)
                {
                    var adornmentTags = line.GetAdornmentTags(this);
                    foreach (var tag in adornmentTags)
                    {
                        if (tag is UIElement uiElement)
                        {
                            if (uiElement.GetValue(ScalingElementContainerProperty) is ScalingElementContainer elementContainer1 && !elementContainer1.IsAddedToLayer)
                            {
                                var adornmentBounds1 = line.GetAdornmentBounds(tag);
                                if (adornmentBounds1.HasValue)
                                {
                                    Canvas.SetTop(elementContainer1, adornmentBounds1.Value.TextTop + line.Baseline - line.LineTransform.VerticalScale * elementContainer1.Baseline);
                                    Canvas.SetLeft(elementContainer1, adornmentBounds1.Value.Left);
                                    elementContainer1.VerticalScale = line.LineTransform.VerticalScale;
                                    var adornmentSourceSpan = GetSingleSpan(elementContainer1.Span.Span.GetSpans(_view.TextSnapshot));
                                    var selectionOnTextViewLine = _view.Selection.GetSelectionOnTextViewLine(line);
                                    var isSelected = selectionOnTextViewLine.HasValue && selectionOnTextViewLine.Value.SnapshotSpan.Contains(adornmentSourceSpan);
                                    IntraTextAdornment.SetIsSelected(elementContainer1, isSelected);
                                    var capturedLine = line;

                                    void SizeChangedEventHandler(object senderAdornment, SizeChangedEventArgs sizeChangedArgs)
                                    {
                                        var adornmentBounds = capturedLine.GetAdornmentBounds(senderAdornment);
                                        if (!adornmentBounds.HasValue) return;
                                        var width1 = adornmentBounds.Value.Width;
                                        var newSize = sizeChangedArgs.NewSize;
                                        var width2 = newSize.Width;
                                        if (Math.Abs(width1 - width2) < 1.0)
                                        {
                                            var height1 = adornmentBounds.Value.Height;
                                            newSize = sizeChangedArgs.NewSize;
                                            var height2 = newSize.Height;
                                            if (height1 >= height2) return;
                                        }

                                        var snapshotPoint = adornmentSourceSpan.Start;
                                        var start = snapshotPoint.GetContainingLine().Start;
                                        snapshotPoint = adornmentSourceSpan.End;
                                        var end = snapshotPoint.GetContainingLine().End;
                                        _spaceNegotiationTagger.RaiseTagsChanged(new SnapshotSpanEventArgs(new SnapshotSpan(start, end)));
                                    }

                                    elementContainer1.AddToLayer();
                                    AdornmentLayer.AddAdornment(AdornmentPositioningBehavior.TextRelative, new SnapshotSpan?(adornmentSourceSpan), null, elementContainer1, (adornmentIdentity, element) =>
                                    {
                                        var elementContainer = element as ScalingElementContainer;
                                        if (elementContainer.Span.Tag.Adornment is FrameworkElement adornment)
                                            adornment.SizeChanged -= SizeChangedEventHandler;
                                        _displayedAdornments.Remove(elementContainer.Span);
                                        var removalCallback = elementContainer.Span.Tag.RemovalCallback;
                                        removalCallback?.Invoke(elementContainer.Span, element);
                                        elementContainer.RemoveFromLayer();
                                    });
                                    _displayedAdornments.Add(elementContainer1.Span);
                                    if (elementContainer1.Span.Tag.Adornment is FrameworkElement adornment1)
                                        adornment1.SizeChanged += SizeChangedEventHandler;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void HandleTagsChanged(object sender, TagsChangedEventArgs args)
        {
            var currentSnapshot = _view.TextBuffer.CurrentSnapshot;
            var spans = args.Span.GetSpans(currentSnapshot);
            foreach (var span in spans)
                _spaceNegotiationTagger.RaiseTagsChanged(new SnapshotSpanEventArgs(span));
            foreach (var span in spans)
                _hiddenRegionTagger.RaiseTagsChanged(new SnapshotSpanEventArgs(span));
        }

        private IEnumerable<ITagSpan<SpaceNegotiatingAdornmentTag>> GetSpaceNegotiationTags(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count != 0)
            {
                foreach (var span in spans)
                {
                    foreach (var tag1 in _adornmentTagAggregator.GetTags(span))
                    {
                        var adornmentSourceSpans = tag1.Span.GetSpans(_view.TextSnapshot);
                        if (adornmentSourceSpans.Count != 0)
                        {
                            var singleSpan = new SnapshotSpan(adornmentSourceSpans[0].Start, adornmentSourceSpans[adornmentSourceSpans.Count - 1].End);
                            var intraTextTag = tag1.Tag;
                            var adornment = intraTextTag.Adornment;
                            var desiredSize = adornment.DesiredSize;
                            if (double.IsInfinity(desiredSize.Height) || double.IsNaN(desiredSize.Height))
                                desiredSize.Height = 0.0;
                            if (double.IsInfinity(desiredSize.Width) || double.IsNaN(desiredSize.Width))
                                desiredSize.Width = 0.0;
                            var topSpace = intraTextTag.TopSpace ?? 0.0;
                            var bottomSpace = intraTextTag.BottomSpace ?? 0.0;
                            var textHeight = intraTextTag.TextHeight ?? desiredSize.Height - topSpace - bottomSpace;
                            var baseline = intraTextTag.Baseline ?? textHeight * 0.75;
                            var scaledAdornment = new ScalingElementContainer(tag1, baseline);
                            adornment.SetValue(ScalingElementContainerProperty, scaledAdornment);
                            var width = desiredSize.Width;
                            var topSpace1 = topSpace;
                            var baseline1 = baseline;
                            var textHeight1 = textHeight;
                            var bottomSpace1 = bottomSpace;
                            var affinity = intraTextTag.Affinity;
                            var num = affinity.HasValue ? (int)affinity.GetValueOrDefault() : 0;
                            var uiElement = adornment;
                            var tag = new SpaceNegotiatingAdornmentTag(width, topSpace1, baseline1, textHeight1, bottomSpace1, (PositionAffinity)num, uiElement, this);
                            yield return new TagSpan<SpaceNegotiatingAdornmentTag>(singleSpan, tag);
                        }
                    }
                }
            }
        }

        private IEnumerable<ITagSpan<IElisionTag>> GetHiddenRegionTags(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count != 0)
            {
                foreach (var span in spans)
                {
                    foreach (var tag in _adornmentTagAggregator.GetTags(span))
                    {
                        var adornmentSpans = tag.Span.GetSpans(_view.TextBuffer);
                        if (adornmentSpans.Count != 0)
                        {
                            var singleSpan = new SnapshotSpan(adornmentSpans[0].Start, adornmentSpans[adornmentSpans.Count - 1].End);
                            yield return new TagSpan<IElisionTag>(singleSpan, new ElisionTag());
                        }
                    }
                }
            }
        }

        internal ITagger<SpaceNegotiatingAdornmentTag> SpaceNegotiationTagger => _spaceNegotiationTagger;

        internal ITagger<IElisionTag> HiddenRegionTagger => _hiddenRegionTagger;

        private class ElisionTag : IElisionTag, ITag
        {
        }

        private class SpaceNegotiationTaggerType : ITagger<SpaceNegotiatingAdornmentTag>
        {
            private readonly IntraTextAdornmentManager manager;

            internal SpaceNegotiationTaggerType(IntraTextAdornmentManager manager)
            {
                this.manager = manager;
            }

            internal void RaiseTagsChanged(SnapshotSpanEventArgs args)
            {
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                TagsChanged?.Invoke(manager, args);
            }

            public IEnumerable<ITagSpan<SpaceNegotiatingAdornmentTag>> GetTags(NormalizedSnapshotSpanCollection spans)
            {
                return manager.GetSpaceNegotiationTags(spans);
            }

            public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
        }

        private class HiddenRegionTaggerType : ITagger<IElisionTag>
        {
            private readonly IntraTextAdornmentManager manager;

            internal HiddenRegionTaggerType(IntraTextAdornmentManager manager)
            {
                this.manager = manager;
            }

            internal void RaiseTagsChanged(SnapshotSpanEventArgs args)
            {
                TagsChanged?.Invoke(manager, args);
            }

            public IEnumerable<ITagSpan<IElisionTag>> GetTags(NormalizedSnapshotSpanCollection spans)
            {
                return manager.GetHiddenRegionTags(spans);
            }

            public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
        }

        internal class ScalingElementContainer : Canvas
        {
            private double verticalScale = 1.0;
            public readonly IMappingTagSpan<IntraTextAdornmentTag> Span;
            public readonly double Baseline;

            public ScalingElementContainer(IMappingTagSpan<IntraTextAdornmentTag> span, double baseline)
            {
                Span = span;
                Baseline = baseline;
            }

            public void AddToLayer()
            {
                Children.Add(Span.Tag.Adornment);
            }

            public void RemoveFromLayer()
            {
                Children.Clear();
            }

            public bool IsAddedToLayer => (uint)Children.Count > 0U;

            public double VerticalScale
            {
                get => verticalScale;
                set
                {
                    if (value == verticalScale)
                        return;
                    verticalScale = value;
                    RenderTransform = new ScaleTransform(1.0, value);
                    RenderTransform.Freeze();
                }
            }
        }
    }

    public class IntraTextAdornmentTag : ITag
    {
        public IntraTextAdornmentTag(UIElement adornment, AdornmentRemovedCallback removalCallback, double? topSpace, double? baseline, double? textHeight, double? bottomSpace, PositionAffinity? affinity)
        {
            Adornment = adornment ?? throw new ArgumentNullException(nameof(adornment));
            RemovalCallback = removalCallback;
            TopSpace = topSpace;
            Baseline = baseline;
            TextHeight = textHeight;
            BottomSpace = bottomSpace;
            Affinity = affinity;
        }

        public IntraTextAdornmentTag(UIElement adornment, AdornmentRemovedCallback removalCallback, PositionAffinity? affinity)
          : this(adornment, removalCallback, new double?(), new double?(), new double?(), new double?(), affinity)
        {
        }

        public IntraTextAdornmentTag(UIElement adornment, AdornmentRemovedCallback removalCallback)
          : this(adornment, removalCallback, new double?(), new double?(), new double?(), new double?(), new PositionAffinity?())
        {
        }

        public UIElement Adornment { get; }

        public AdornmentRemovedCallback RemovalCallback { get; }

        public double? TopSpace { get; }

        public double? Baseline { get; }

        public double? TextHeight { get; }

        public double? BottomSpace { get; }

        public PositionAffinity? Affinity { get; }
    }

    public static class IntraTextAdornment
    {
        public static readonly DependencyProperty IsSelected = DependencyProperty.RegisterAttached(nameof(IsSelected), typeof(bool), typeof(IntraTextAdornment));

        public static void SetIsSelected(UIElement element, bool isSelected)
        {
            element.SetValue(IsSelected, isSelected);
        }

        public static bool GetIsSelected(UIElement element)
        {
            return true.Equals(element.GetValue(IsSelected));
        }
    }
}