using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace ModernApplicationFramework.Extended.Demo.GraphDesigner.Controls
{
    public class ZoomAndPanControl : ContentControl, IScrollInfo
    {
        public event EventHandler ContentOffsetXChanged;
        public event EventHandler ContentOffsetYChanged;
        public event EventHandler ContentScaleChanged;

        private double _constrainedContentViewportHeight;
        private double _constrainedContentViewportWidth;
        private FrameworkElement _content;
        private TranslateTransform _contentOffsetTransform;
        private ScaleTransform _contentScaleTransform;
        private bool _disableContentFocusSync;
        private bool _disableScrollOffsetSync;
        private bool _enableContentOffsetUpdateFromScale;
        private Size _unScaledExtent = new Size(0, 0);
        private Size _viewport = new Size(0, 0);

        public static readonly DependencyProperty ContentScaleProperty =
            DependencyProperty.Register("ContentScale", typeof(double), typeof(ZoomAndPanControl),
                new FrameworkPropertyMetadata(1.0, ContentScale_PropertyChanged, ContentScale_Coerce));

        public static readonly DependencyProperty MinContentScaleProperty =
            DependencyProperty.Register("MinContentScale", typeof(double), typeof(ZoomAndPanControl),
                new FrameworkPropertyMetadata(0.01, MinOrMaxContentScale_PropertyChanged));

        public static readonly DependencyProperty MaxContentScaleProperty =
            DependencyProperty.Register("MaxContentScale", typeof(double), typeof(ZoomAndPanControl),
                new FrameworkPropertyMetadata(10.0, MinOrMaxContentScale_PropertyChanged));

        public static readonly DependencyProperty ContentOffsetXProperty =
            DependencyProperty.Register("ContentOffsetX", typeof(double), typeof(ZoomAndPanControl),
                new FrameworkPropertyMetadata(0.0, ContentOffsetX_PropertyChanged, ContentOffsetX_Coerce));

        public static readonly DependencyProperty ContentOffsetYProperty =
            DependencyProperty.Register("ContentOffsetY", typeof(double), typeof(ZoomAndPanControl),
                new FrameworkPropertyMetadata(0.0, ContentOffsetY_PropertyChanged, ContentOffsetY_Coerce));

        public static readonly DependencyProperty AnimationDurationProperty =
            DependencyProperty.Register("AnimationDuration", typeof(double), typeof(ZoomAndPanControl),
                new FrameworkPropertyMetadata(0.4));

        public static readonly DependencyProperty ContentViewportHeightProperty =
            DependencyProperty.Register("ContentViewportHeight", typeof(double), typeof(ZoomAndPanControl),
                new FrameworkPropertyMetadata(0.0));

        public static readonly DependencyProperty ContentViewportWidthProperty =
            DependencyProperty.Register("ContentViewportWidth", typeof(double), typeof(ZoomAndPanControl),
                new FrameworkPropertyMetadata(0.0));

        public static readonly DependencyProperty ContentZoomFocusXProperty =
            DependencyProperty.Register("ContentZoomFocusX", typeof(double), typeof(ZoomAndPanControl),
                new FrameworkPropertyMetadata(0.0));

        public static readonly DependencyProperty ContentZoomFocusYProperty =
            DependencyProperty.Register("ContentZoomFocusY", typeof(double), typeof(ZoomAndPanControl),
                new FrameworkPropertyMetadata(0.0));

        public static readonly DependencyProperty IsMouseWheelScrollingEnabledProperty =
            DependencyProperty.Register("IsMouseWheelScrollingEnabled", typeof(bool), typeof(ZoomAndPanControl),
                new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty ViewportZoomFocusXProperty =
            DependencyProperty.Register("ViewportZoomFocusX", typeof(double), typeof(ZoomAndPanControl),
                new FrameworkPropertyMetadata(0.0));

        public static readonly DependencyProperty ViewportZoomFocusYProperty =
            DependencyProperty.Register("ViewportZoomFocusY", typeof(double), typeof(ZoomAndPanControl),
                new FrameworkPropertyMetadata(0.0));

        public double AnimationDuration
        {
            get => (double)GetValue(AnimationDurationProperty);
            set => SetValue(AnimationDurationProperty, value);
        }

        public double ContentOffsetX
        {
            get => (double)GetValue(ContentOffsetXProperty);
            set => SetValue(ContentOffsetXProperty, value);
        }

        public double ContentOffsetY
        {
            get => (double)GetValue(ContentOffsetYProperty);
            set => SetValue(ContentOffsetYProperty, value);
        }

        public double ContentScale
        {
            get => (double)GetValue(ContentScaleProperty);
            set => SetValue(ContentScaleProperty, value);
        }

        public double ContentViewportHeight
        {
            get => (double)GetValue(ContentViewportHeightProperty);
            set => SetValue(ContentViewportHeightProperty, value);
        }

        public double ContentViewportWidth
        {
            get => (double)GetValue(ContentViewportWidthProperty);
            set => SetValue(ContentViewportWidthProperty, value);
        }

        public double ContentZoomFocusX
        {
            get => (double)GetValue(ContentZoomFocusXProperty);
            set => SetValue(ContentZoomFocusXProperty, value);
        }

        public double ContentZoomFocusY
        {
            get => (double)GetValue(ContentZoomFocusYProperty);
            set => SetValue(ContentZoomFocusYProperty, value);
        }

        public bool IsMouseWheelScrollingEnabled
        {
            get => (bool)GetValue(IsMouseWheelScrollingEnabledProperty);
            set => SetValue(IsMouseWheelScrollingEnabledProperty, value);
        }

        public double MaxContentScale
        {
            get => (double)GetValue(MaxContentScaleProperty);
            set => SetValue(MaxContentScaleProperty, value);
        }

        public double MinContentScale
        {
            get => (double)GetValue(MinContentScaleProperty);
            set => SetValue(MinContentScaleProperty, value);
        }

        public double ViewportZoomFocusX
        {
            get => (double)GetValue(ViewportZoomFocusXProperty);
            set => SetValue(ViewportZoomFocusXProperty, value);
        }

        public double ViewportZoomFocusY
        {
            get => (double)GetValue(ViewportZoomFocusYProperty);
            set => SetValue(ViewportZoomFocusYProperty, value);
        }

        public bool CanVerticallyScroll { get; set; }

        public bool CanHorizontallyScroll { get; set; }

        public double ExtentWidth => _unScaledExtent.Width * ContentScale;

        public double ExtentHeight => _unScaledExtent.Height * ContentScale;

        public double ViewportWidth => _viewport.Width;

        public double ViewportHeight => _viewport.Height;

        public ScrollViewer ScrollOwner { get; set; }

        public double HorizontalOffset => ContentOffsetX * ContentScale;

        public double VerticalOffset => ContentOffsetY * ContentScale;

        static ZoomAndPanControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ZoomAndPanControl),
                new FrameworkPropertyMetadata(typeof(ZoomAndPanControl)));
        }

        public ZoomAndPanControl()
        {
            ScrollOwner = null;
            CanHorizontallyScroll = false;
            CanVerticallyScroll = false;
        }

        public void AnimatedScaleToFit()
        {
            if (_content == null)
                throw new ApplicationException("PART_Content was not found in the ZoomAndPanControl visual template!");

            AnimatedZoomTo(new Rect(0, 0, _content.ActualWidth, _content.ActualHeight));
        }

        public void AnimatedSnapTo(Point contentPoint)
        {
            var newX = contentPoint.X - ContentViewportWidth / 2;
            var newY = contentPoint.Y - ContentViewportHeight / 2;

            AnimationHelper.StartAnimation(this, ContentOffsetXProperty, newX, AnimationDuration);
            AnimationHelper.StartAnimation(this, ContentOffsetYProperty, newY, AnimationDuration);
        }

        public void AnimatedZoomAboutPoint(double newContentScale, Point contentZoomFocus)
        {
            newContentScale = Math.Min(Math.Max(newContentScale, MinContentScale), MaxContentScale);

            AnimationHelper.CancelAnimation(this, ContentZoomFocusXProperty);
            AnimationHelper.CancelAnimation(this, ContentZoomFocusYProperty);
            AnimationHelper.CancelAnimation(this, ViewportZoomFocusXProperty);
            AnimationHelper.CancelAnimation(this, ViewportZoomFocusYProperty);

            ContentZoomFocusX = contentZoomFocus.X;
            ContentZoomFocusY = contentZoomFocus.Y;
            ViewportZoomFocusX = (ContentZoomFocusX - ContentOffsetX) * ContentScale;
            ViewportZoomFocusY = (ContentZoomFocusY - ContentOffsetY) * ContentScale;

            _enableContentOffsetUpdateFromScale = true;

            AnimationHelper.StartAnimation(this, ContentScaleProperty, newContentScale, AnimationDuration,
                delegate
                {
                    _enableContentOffsetUpdateFromScale = false;

                    ResetViewportZoomFocus();
                });
        }

        public void AnimatedZoomTo(double newScale, Rect contentRect)
        {
            AnimatedZoomPointToViewportCenter(newScale,
                new Point(contentRect.X + contentRect.Width / 2, contentRect.Y + contentRect.Height / 2),
                delegate
                {
                    ContentOffsetX = contentRect.X;
                    ContentOffsetY = contentRect.Y;
                });
        }

        public void AnimatedZoomTo(Rect contentRect)
        {
            var scaleX = ContentViewportWidth / contentRect.Width;
            var scaleY = ContentViewportHeight / contentRect.Height;
            var newScale = ContentScale * Math.Min(scaleX, scaleY);

            AnimatedZoomPointToViewportCenter(newScale,
                new Point(contentRect.X + contentRect.Width / 2, contentRect.Y + contentRect.Height / 2), null);
        }

        public void AnimatedZoomTo(double contentScale)
        {
            var zoomCenter = new Point(ContentOffsetX + ContentViewportWidth / 2,
                ContentOffsetY + ContentViewportHeight / 2);
            AnimatedZoomAboutPoint(contentScale, zoomCenter);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _content = Template.FindName("PART_Content", this) as FrameworkElement;
            if (_content != null)
            {
                _contentScaleTransform = new ScaleTransform(ContentScale, ContentScale);
                _contentOffsetTransform = new TranslateTransform();
                UpdateTranslationX();
                UpdateTranslationY();
                var transformGroup = new TransformGroup();
                transformGroup.Children.Add(_contentOffsetTransform);
                transformGroup.Children.Add(_contentScaleTransform);
                _content.RenderTransform = transformGroup;
            }
        }

        public void ScaleToFit()
        {
            if (_content == null)
                throw new ApplicationException("PART_Content was not found in the ZoomAndPanControl visual template!");

            ZoomTo(new Rect(0, 0, _content.ActualWidth, _content.ActualHeight));
        }

        public void SnapContentOffsetTo(Point contentOffset)
        {
            AnimationHelper.CancelAnimation(this, ContentOffsetXProperty);
            AnimationHelper.CancelAnimation(this, ContentOffsetYProperty);

            ContentOffsetX = contentOffset.X;
            ContentOffsetY = contentOffset.Y;
        }

        public void SnapTo(Point contentPoint)
        {
            AnimationHelper.CancelAnimation(this, ContentOffsetXProperty);
            AnimationHelper.CancelAnimation(this, ContentOffsetYProperty);

            ContentOffsetX = contentPoint.X - ContentViewportWidth / 2;
            ContentOffsetY = contentPoint.Y - ContentViewportHeight / 2;
        }

        public void ZoomAboutPoint(double newContentScale, Point contentZoomFocus)
        {
            newContentScale = Math.Min(Math.Max(newContentScale, MinContentScale), MaxContentScale);

            var screenSpaceZoomOffsetX = (contentZoomFocus.X - ContentOffsetX) * ContentScale;
            var screenSpaceZoomOffsetY = (contentZoomFocus.Y - ContentOffsetY) * ContentScale;
            var contentSpaceZoomOffsetX = screenSpaceZoomOffsetX / newContentScale;
            var contentSpaceZoomOffsetY = screenSpaceZoomOffsetY / newContentScale;
            var newContentOffsetX = contentZoomFocus.X - contentSpaceZoomOffsetX;
            var newContentOffsetY = contentZoomFocus.Y - contentSpaceZoomOffsetY;

            AnimationHelper.CancelAnimation(this, ContentScaleProperty);
            AnimationHelper.CancelAnimation(this, ContentOffsetXProperty);
            AnimationHelper.CancelAnimation(this, ContentOffsetYProperty);

            ContentScale = newContentScale;
            ContentOffsetX = newContentOffsetX;
            ContentOffsetY = newContentOffsetY;
        }

        public void ZoomTo(Rect contentRect)
        {
            var scaleX = ContentViewportWidth / contentRect.Width;
            var scaleY = ContentViewportHeight / contentRect.Height;
            var newScale = ContentScale * Math.Min(scaleX, scaleY);

            ZoomPointToViewportCenter(newScale,
                new Point(contentRect.X + contentRect.Width / 2, contentRect.Y + contentRect.Height / 2));
        }

        public void ZoomTo(double contentScale)
        {
            var zoomCenter = new Point(ContentOffsetX + ContentViewportWidth / 2,
                ContentOffsetY + ContentViewportHeight / 2);
            ZoomAboutPoint(contentScale, zoomCenter);
        }

        public void SetHorizontalOffset(double offset)
        {
            if (_disableScrollOffsetSync)
                return;

            try
            {
                _disableScrollOffsetSync = true;

                ContentOffsetX = offset / ContentScale;
            }
            finally
            {
                _disableScrollOffsetSync = false;
            }
        }

        public void SetVerticalOffset(double offset)
        {
            if (_disableScrollOffsetSync)
                return;

            try
            {
                _disableScrollOffsetSync = true;

                ContentOffsetY = offset / ContentScale;
            }
            finally
            {
                _disableScrollOffsetSync = false;
            }
        }

        public void LineUp()
        {
            ContentOffsetY -= (ContentViewportHeight / 10);
        }

        public void LineDown()
        {
            ContentOffsetY += (ContentViewportHeight / 10);
        }

        public void LineLeft()
        {
            ContentOffsetX -= (ContentViewportWidth / 10);
        }

        public void LineRight()
        {
            ContentOffsetX += (ContentViewportWidth / 10);
        }

        public void PageUp()
        {
            ContentOffsetY -= ContentViewportHeight;
        }

        public void PageDown()
        {
            ContentOffsetY += ContentViewportHeight;
        }

        public void PageLeft()
        {
            ContentOffsetX -= ContentViewportWidth;
        }

        public void PageRight()
        {
            ContentOffsetX += ContentViewportWidth;
        }

        public void MouseWheelDown()
        {
            if (IsMouseWheelScrollingEnabled)
                LineDown();
        }

        public void MouseWheelLeft()
        {
            if (IsMouseWheelScrollingEnabled)
                LineLeft();
        }

        public void MouseWheelRight()
        {
            if (IsMouseWheelScrollingEnabled)
                LineRight();
        }

        public void MouseWheelUp()
        {
            if (IsMouseWheelScrollingEnabled)
                LineUp();
        }

        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            if (_content.IsAncestorOf(visual))
            {
                Rect transformedRect = visual.TransformToAncestor(_content).TransformBounds(rectangle);
                Rect viewportRect = new Rect(ContentOffsetX, ContentOffsetY, ContentViewportWidth, ContentViewportHeight);
                if (!transformedRect.Contains(viewportRect))
                {
                    double horizOffset = 0;
                    double vertOffset = 0;

                    if (transformedRect.Left < viewportRect.Left)
                    {
                        horizOffset = transformedRect.Left - viewportRect.Left;
                    }
                    else if (transformedRect.Right > viewportRect.Right)
                    {
                        horizOffset = transformedRect.Right - viewportRect.Right;
                    }

                    if (transformedRect.Top < viewportRect.Top)
                    {
                        vertOffset = transformedRect.Top - viewportRect.Top;
                    }
                    else if (transformedRect.Bottom > viewportRect.Bottom)
                    {
                        vertOffset = transformedRect.Bottom - viewportRect.Bottom;
                    }

                    SnapContentOffsetTo(new Point(ContentOffsetX + horizOffset, ContentOffsetY + vertOffset));
                }
            }
            return rectangle;
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var size = base.ArrangeOverride(DesiredSize);

            if (_content.DesiredSize != _unScaledExtent)
            {
                _unScaledExtent = _content.DesiredSize;
                ScrollOwner?.InvalidateScrollInfo();
            }
            UpdateViewportSize(arrangeBounds);

            return size;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var infiniteSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
            var childSize = base.MeasureOverride(infiniteSize);

            if (childSize != _unScaledExtent)
            {
                _unScaledExtent = childSize;
                ScrollOwner?.InvalidateScrollInfo();
            }
            UpdateViewportSize(constraint);

            var width = constraint.Width;
            var height = constraint.Height;

            if (double.IsInfinity(width)) width = childSize.Width;

            if (double.IsInfinity(height)) height = childSize.Height;

            UpdateTranslationX();
            UpdateTranslationY();

            return new Size(width, height);
        }

        private static object ContentOffsetX_Coerce(DependencyObject d, object baseValue)
        {
            var c = (ZoomAndPanControl)d;
            var value = (double)baseValue;
            const double minOffsetX = 0.0;
            var maxOffsetX = Math.Max(0.0, c._unScaledExtent.Width - c._constrainedContentViewportWidth);
            value = Math.Min(Math.Max(value, minOffsetX), maxOffsetX);
            return value;
        }

        private static void ContentOffsetX_PropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var c = (ZoomAndPanControl)o;

            c.UpdateTranslationX();

            if (!c._disableContentFocusSync) c.UpdateContentZoomFocusX();

            c.ContentOffsetXChanged?.Invoke(c, EventArgs.Empty);

            if (!c._disableScrollOffsetSync) c.ScrollOwner?.InvalidateScrollInfo();
        }

        private static object ContentOffsetY_Coerce(DependencyObject d, object baseValue)
        {
            var c = (ZoomAndPanControl)d;
            var value = (double)baseValue;
            const double minOffsetY = 0.0;
            var maxOffsetY = Math.Max(0.0, c._unScaledExtent.Height - c._constrainedContentViewportHeight);
            value = Math.Min(Math.Max(value, minOffsetY), maxOffsetY);
            return value;
        }

        private static void ContentOffsetY_PropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var c = (ZoomAndPanControl)o;

            c.UpdateTranslationY();

            if (!c._disableContentFocusSync) c.UpdateContentZoomFocusY();

            c.ContentOffsetYChanged?.Invoke(c, EventArgs.Empty);

            if (!c._disableScrollOffsetSync) c.ScrollOwner?.InvalidateScrollInfo();
        }

        private static object ContentScale_Coerce(DependencyObject d, object baseValue)
        {
            var c = (ZoomAndPanControl)d;
            var value = (double)baseValue;
            value = Math.Min(Math.Max(value, c.MinContentScale), c.MaxContentScale);
            return value;
        }

        private static void ContentScale_PropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var c = (ZoomAndPanControl)o;

            if (c._contentScaleTransform != null)
            {
                c._contentScaleTransform.ScaleX = c.ContentScale;
                c._contentScaleTransform.ScaleY = c.ContentScale;
            }
            c.UpdateContentViewportSize();

            if (c._enableContentOffsetUpdateFromScale)
                try
                {
                    c._disableContentFocusSync = true;
                    var viewportOffsetX = c.ViewportZoomFocusX - c.ViewportWidth / 2;
                    var viewportOffsetY = c.ViewportZoomFocusY - c.ViewportHeight / 2;
                    var contentOffsetX = viewportOffsetX / c.ContentScale;
                    var contentOffsetY = viewportOffsetY / c.ContentScale;
                    c.ContentOffsetX = c.ContentZoomFocusX - c.ContentViewportWidth / 2 - contentOffsetX;
                    c.ContentOffsetY = c.ContentZoomFocusY - c.ContentViewportHeight / 2 - contentOffsetY;
                }
                finally
                {
                    c._disableContentFocusSync = false;
                }

            c.ContentScaleChanged?.Invoke(c, EventArgs.Empty);

            c.ScrollOwner?.InvalidateScrollInfo();
        }

        private static void MinOrMaxContentScale_PropertyChanged(DependencyObject o,
            DependencyPropertyChangedEventArgs e)
        {
            var c = (ZoomAndPanControl)o;
            c.ContentScale = Math.Min(Math.Max(c.ContentScale, c.MinContentScale), c.MaxContentScale);
        }

        private void AnimatedZoomPointToViewportCenter(double newContentScale, Point contentZoomFocus,
            EventHandler callback)
        {
            newContentScale = Math.Min(Math.Max(newContentScale, MinContentScale), MaxContentScale);

            AnimationHelper.CancelAnimation(this, ContentZoomFocusXProperty);
            AnimationHelper.CancelAnimation(this, ContentZoomFocusYProperty);
            AnimationHelper.CancelAnimation(this, ViewportZoomFocusXProperty);
            AnimationHelper.CancelAnimation(this, ViewportZoomFocusYProperty);

            ContentZoomFocusX = contentZoomFocus.X;
            ContentZoomFocusY = contentZoomFocus.Y;
            ViewportZoomFocusX = (ContentZoomFocusX - ContentOffsetX) * ContentScale;
            ViewportZoomFocusY = (ContentZoomFocusY - ContentOffsetY) * ContentScale;

            //
            // When zooming about a point make updates to ContentScale also update content offset.
            //
            _enableContentOffsetUpdateFromScale = true;

            AnimationHelper.StartAnimation(this, ContentScaleProperty, newContentScale, AnimationDuration,
                delegate
                {
                    _enableContentOffsetUpdateFromScale = false;

                    callback?.Invoke(this, EventArgs.Empty);
                });

            AnimationHelper.StartAnimation(this, ViewportZoomFocusXProperty, ViewportWidth / 2, AnimationDuration);
            AnimationHelper.StartAnimation(this, ViewportZoomFocusYProperty, ViewportHeight / 2, AnimationDuration);
        }

        private void ResetViewportZoomFocus()
        {
            ViewportZoomFocusX = ViewportWidth / 2;
            ViewportZoomFocusY = ViewportHeight / 2;
        }

        private void UpdateContentViewportSize()
        {
            ContentViewportWidth = ViewportWidth / ContentScale;
            ContentViewportHeight = ViewportHeight / ContentScale;

            _constrainedContentViewportWidth = Math.Min(ContentViewportWidth, _unScaledExtent.Width);
            _constrainedContentViewportHeight = Math.Min(ContentViewportHeight, _unScaledExtent.Height);

            UpdateTranslationX();
            UpdateTranslationY();
        }

        private void UpdateContentZoomFocusX()
        {
            ContentZoomFocusX = ContentOffsetX + _constrainedContentViewportWidth / 2;
        }

        private void UpdateContentZoomFocusY()
        {
            ContentZoomFocusY = ContentOffsetY + _constrainedContentViewportHeight / 2;
        }

        private void UpdateTranslationX()
        {
            if (_contentOffsetTransform != null)
            {
                var scaledContentWidth = _unScaledExtent.Width * ContentScale;
                if (scaledContentWidth < ViewportWidth)
                    _contentOffsetTransform.X = (ContentViewportWidth - _unScaledExtent.Width) / 2;
                else
                    _contentOffsetTransform.X = -ContentOffsetX;
            }
        }

        private void UpdateTranslationY()
        {
            if (_contentOffsetTransform != null)
            {
                var scaledContentHeight = _unScaledExtent.Height * ContentScale;
                if (scaledContentHeight < ViewportHeight)
                    _contentOffsetTransform.Y = (ContentViewportHeight - _unScaledExtent.Height) / 2;
                else
                    _contentOffsetTransform.Y = -ContentOffsetY;
            }
        }

        private void UpdateViewportSize(Size newSize)
        {
            if (_viewport == newSize) return;

            _viewport = newSize;
            UpdateContentViewportSize();
            UpdateContentZoomFocusX();
            UpdateContentZoomFocusY();

            ResetViewportZoomFocus();

            ContentOffsetX = ContentOffsetX;
            ContentOffsetY = ContentOffsetY;

            ScrollOwner?.InvalidateScrollInfo();
        }

        private void ZoomPointToViewportCenter(double newContentScale, Point contentZoomFocus)
        {
            newContentScale = Math.Min(Math.Max(newContentScale, MinContentScale), MaxContentScale);

            AnimationHelper.CancelAnimation(this, ContentScaleProperty);
            AnimationHelper.CancelAnimation(this, ContentOffsetXProperty);
            AnimationHelper.CancelAnimation(this, ContentOffsetYProperty);

            ContentScale = newContentScale;
            ContentOffsetX = contentZoomFocus.X - ContentViewportWidth / 2;
            ContentOffsetY = contentZoomFocus.Y - ContentViewportHeight / 2;
        }
    }
}
