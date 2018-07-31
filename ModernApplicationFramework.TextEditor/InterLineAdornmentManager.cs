using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using ModernApplicationFramework.TextEditor.Text.Formatting;

namespace ModernApplicationFramework.TextEditor
{
    internal sealed class InterLineAdornmentManager : ILineTransformSource
    {
        internal readonly List<ActiveTag> Tags = new List<ActiveTag>();
        private readonly ITextView _view;
        private ITagAggregator<InterLineAdornmentTag> _tagAggregator;
        private readonly IAdornmentLayer _layer;
        private NormalizedSpanCollection _invalidatedSpans;
        internal bool NeedsLayout;
        private bool _delayTagChanges;
        private ITextSnapshot _currentSnapshot;

        public static InterLineAdornmentManager Create(ITextView view, InterLineAdornmentManagerFactory factory)
        {
            return view.Properties.GetOrCreateSingletonProperty(() => new InterLineAdornmentManager(view, factory));
        }

        public InterLineAdornmentManager(ITextView view, InterLineAdornmentManagerFactory factory)
        {
            _view = view;
            _tagAggregator = factory.TagAggregatorFactoryService.CreateTagAggregator<InterLineAdornmentTag>(view, (TagAggregatorOptions)2);
            _layer = view.GetAdornmentLayer("Inter Line Adornment");
            _currentSnapshot = view.TextSnapshot;
            _tagAggregator.BatchedTagsChanged += OnBatchedTagsChanged;
            _view.LayoutChanged += OnLayoutChanged;
            _view.Closed += OnTextViewClosed;
        }

        public LineTransform GetLineTransform(ITextViewLine line, double yPosition, ViewRelativePosition placement)
        {
            if (_currentSnapshot != line.Snapshot)
            {
                _delayTagChanges = true;
                foreach (var tag in Tags)
                    tag.Position = Tracking.TrackPositionForwardInTime(PointTrackingMode.Negative, tag.Position, _currentSnapshot.Version, line.Snapshot.Version);
                _currentSnapshot = line.Snapshot;
            }
            var spans1 = line.ExtentAsMappingSpan.GetSpans(_currentSnapshot);
            if (line.Change == TextViewLineChange.NewOrReformatted || _invalidatedSpans != null && _invalidatedSpans.IntersectsWith(line.Extent))
            {
                foreach (var tag in Tags)
                {
                    if (line.ContainsBufferPosition(new SnapshotPoint(_currentSnapshot, tag.Position)))
                        tag.NeedsToBeDeleted = true;
                }
                foreach (var tag1 in _tagAggregator.GetTags(spans1))
                {
                    var t = tag1;
                    var spans2 = t.Span.GetSpans(line.Snapshot);
                    if (spans2.Count > 0)
                    {
                        int start = spans2[0].Start;
                        var activeTag = Tags.Find(tag => t.Tag == tag.Tag);
                        if (activeTag != null)
                        {
                            activeTag.NeedsToBeDeleted = false;
                            activeTag.Position = start;
                        }
                        else
                            Tags.Add(new ActiveTag(start, t.Tag)
                            {
                                DelayTagCreation = _delayTagChanges
                            });
                    }
                }
            }
            var num1 = 0.0;
            var num2 = 0.0;
            foreach (var tag in Tags)
            {
                if (!tag.DelayTagCreation && (_delayTagChanges || !tag.NeedsToBeDeleted) && line.ContainsBufferPosition(new SnapshotPoint(_currentSnapshot, tag.Position)))
                {
                    if (!_delayTagChanges)
                        tag.Height = tag.Tag.Height;
                    if (tag.Tag.IsAboveLine)
                        num1 = Math.Max(num1, tag.Height);
                    else
                        num2 = Math.Max(num2, tag.Height);
                }
            }
            return new LineTransform(num1, num2, 1.0);
        }

        private void OnTextViewClosed(object sender, EventArgs e)
        {
            _tagAggregator.BatchedTagsChanged -= OnBatchedTagsChanged;
            _view.LayoutChanged -= OnLayoutChanged;
            _view.Closed -= OnTextViewClosed;
            for (var i = Tags.Count - 1; i >= 0; --i)
                RemoveTagAt(i);
            _tagAggregator.Dispose();
            _tagAggregator = null;
        }

        private void OnBatchedTagsChanged(object sender, BatchedTagsChangedEventArgs e)
        {
            if (_view.IsClosed || e.Spans.Count <= 0)
                return;
            var spanList = new List<Span>();
            foreach (var span1 in e.Spans)
            {
                foreach (var span2 in span1.GetSpans(_currentSnapshot))
                    spanList.Add(span2);
            }
            try
            {
                NeedsLayout = true;
                _invalidatedSpans = new NormalizedSpanCollection(spanList);
                PerformLayout(_view.Caret.Position.BufferPosition);
            }
            finally
            {
                _invalidatedSpans = null;
            }
        }

        internal void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            NeedsLayout = false;
            for (var i = Tags.Count - 1; i >= 0; --i)
            {
                var tag = Tags[i];
                if (tag.NeedsToBeDeleted)
                {
                    if (_delayTagChanges)
                        NeedsLayout = true;
                    else
                        RemoveTagAt(i);
                }
                else
                {
                    var snapshotPoint = new SnapshotPoint(_currentSnapshot, tag.Position);
                    var containingBufferPosition = _view.TextViewLines.GetTextViewLineContainingBufferPosition(snapshotPoint);
                    if (containingBufferPosition == null)
                        RemoveTagAt(i);
                    else if (tag.DelayTagCreation)
                    {
                        NeedsLayout = true;
                    }
                    else
                    {
                        if (!tag.IsAddedToAdornmentLayer)
                        {
                            tag.IsAddedToAdornmentLayer = true;
                            var child = tag.Tag.AdornmentFactory?.Invoke(tag.Tag, _view, snapshotPoint);
                            if (child != null)
                            {
                                tag.Adornment = new AdornmentWrapper(child);
                                tag.Tag.HorizontalOffsetChanged += OnHorizontalOffsetPropertyChanged;
                                _layer.AddAdornment(AdornmentPositioningBehavior.OwnerControlled, new SnapshotSpan?(), null, tag.Adornment, null);
                            }
                            tag.Tag.HeightChanged += OnHeightPropertyChanged;
                        }
                        if (tag.Adornment != null)
                            PositionAndScaleTag(containingBufferPosition, snapshotPoint, tag);
                    }
                }
            }
            if (NeedsLayout)
                _view.VisualElement.Dispatcher.BeginInvoke((Action)(() => PerformLayout(_view.Caret.Position.BufferPosition)), DispatcherPriority.Normal, Array.Empty<object>());
            else
                _delayTagChanges = false;
        }

        private void OnHeightPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is InterLineAdornmentTag lineAdornmentTag) || !lineAdornmentTag.IsAnimating)
                return;
            foreach (var tag in Tags)
            {
                if (tag.Tag == lineAdornmentTag)
                {
                    NeedsLayout = true;
                    PerformLayout(new SnapshotPoint(_currentSnapshot, tag.Position).TranslateTo(_view.TextSnapshot, PointTrackingMode.Negative));
                    break;
                }
            }
        }

        private void OnHorizontalOffsetPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_currentSnapshot != _view.TextSnapshot)
                return;
            if (!(sender is InterLineAdornmentTag lineAdornmentTag))
                return;
            foreach (var tag in Tags)
            {
                if (tag.Tag == lineAdornmentTag)
                {
                    var snapshotPoint = new SnapshotPoint(_currentSnapshot, tag.Position);
                    PositionAndScaleTag(_view.TextViewLines.GetTextViewLineContainingBufferPosition(snapshotPoint), snapshotPoint, tag);
                    break;
                }
            }
        }

        private void PositionAndScaleTag(ITextViewLine line, SnapshotPoint position, ActiveTag tag)
        {
            var horizontalOffset = tag.Tag.HorizontalOffset;
            if (tag.Tag.HorizontalPositioningMode == HorizontalPositioningMode.TextRelative)
                horizontalOffset += line.GetCharacterBounds(position).Left;
            else if (tag.Tag.HorizontalPositioningMode == HorizontalPositioningMode.ViewRelative)
                horizontalOffset += _view.ViewportLeft;
            tag.Adornment.RenderTransform = new TranslateTransform(horizontalOffset, tag.Tag.IsAboveLine ? line.TextTop - tag.Tag.Height : line.TextBottom);
            tag.Adornment.RenderTransform.Freeze();
        }

        private void RemoveTagAt(int i)
        {
            var tag = Tags[i];
            Tags.RemoveAt(i);
            if (!tag.IsAddedToAdornmentLayer)
                return;
            tag.Tag.HeightChanged -= OnHeightPropertyChanged;
            if (tag.Adornment == null)
                return;
            var child = tag.Adornment.Child;
            tag.Tag.RemovalCallback?.Invoke(tag.Tag, child);
            tag.Adornment.Clear();
            tag.Tag.HorizontalOffsetChanged -= OnHorizontalOffsetPropertyChanged;
            _layer.RemoveAdornment(tag.Adornment);
        }

        private void PerformLayout(SnapshotPoint trackingPoint)
        {
            if (_view.IsClosed || !NeedsLayout || _view.InOuterLayout)
                return;
            NeedsLayout = false;
            if (_delayTagChanges)
            {
                _delayTagChanges = false;
                for (var i = Tags.Count - 1; i >= 0; --i)
                {
                    var tag = Tags[i];
                    if (tag.NeedsToBeDeleted)
                        RemoveTagAt(i);
                    else
                        tag.DelayTagCreation = false;
                }
            }
            var textViewLine = _view.TextViewLines.GetTextViewLineContainingBufferPosition(trackingPoint);
            var relativeTo = ViewRelativePosition.Top;
            double verticalDistance;
            if (textViewLine == null)
            {
                textViewLine = _view.TextViewLines.FirstVisibleLine;
                verticalDistance = textViewLine.Top - _view.ViewportTop;
            }
            else
            {
                relativeTo = (ViewRelativePosition)2;
                verticalDistance = textViewLine.TextTop - _view.ViewportTop;
            }
            _view.DisplayTextLineContainingBufferPosition(textViewLine.Start, verticalDistance, relativeTo);
        }

        internal class ActiveTag
        {
            public readonly InterLineAdornmentTag Tag;
            public AdornmentWrapper Adornment;
            public int Position;
            public double Height;
            public bool IsAddedToAdornmentLayer;
            public bool DelayTagCreation;
            public bool NeedsToBeDeleted;

            public ActiveTag(int position, InterLineAdornmentTag tag)
            {
                Position = position;
                Tag = tag;
            }
        }

        internal class AdornmentWrapper : Canvas
        {
            public UIElement Child => Children[0];

            public AdornmentWrapper(UIElement child)
            {
                Children.Add(child);
            }

            public void Clear()
            {
                Children.Clear();
            }
        }
    }
}