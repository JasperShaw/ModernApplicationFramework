using System;
using System.Windows;
using ModernApplicationFramework.Text.Logic.Tagging;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public class InterLineAdornmentTag : DependencyObject, ITag
    {
        public static readonly DependencyProperty HeightProperty = DependencyProperty.Register(nameof(Height),
            typeof(double), typeof(InterLineAdornmentTag), new PropertyMetadata(0.0, HeightChangedCallback));

        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register(nameof(HorizontalOffset), typeof(double), typeof(InterLineAdornmentTag),
                new PropertyMetadata(0.0, HorizontalOffsetChangedCallback));

        public event EventHandler<DependencyPropertyChangedEventArgs> HeightChanged;

        public event EventHandler<DependencyPropertyChangedEventArgs> HorizontalOffsetChanged;

        public InterLineAdornmentFactory AdornmentFactory { get; }

        public double Height
        {
            get => (double) GetValue(HeightProperty);
            set => SetValue(HeightProperty, value);
        }

        public double HorizontalOffset
        {
            get => (double) GetValue(HorizontalOffsetProperty);
            set => SetValue(HorizontalOffsetProperty, value);
        }

        public HorizontalPositioningMode HorizontalPositioningMode { get; }

        public bool IsAboveLine { get; }

        public bool IsAnimating { get; set; } = true;

        public AdornmentRemovedCallback RemovalCallback { get; }

        public InterLineAdornmentTag(InterLineAdornmentFactory adornmentFactory, bool isAboveLine, double initialHeight,
            HorizontalPositioningMode horizontalPositioningMode, double horizontalOffset,
            AdornmentRemovedCallback removalCallback = null)
        {
            if (adornmentFactory == null && removalCallback != null)
                throw new ArgumentException(
                    "Either adornmentFactory must be non-null or removalCallback must be null.");
            if (initialHeight < 0.0)
                throw new ArgumentException("initialHeight must be >= 0.");
            AdornmentFactory = adornmentFactory;
            IsAboveLine = isAboveLine;
            Height = initialHeight;
            HorizontalPositioningMode = horizontalPositioningMode;
            HorizontalOffset = horizontalOffset;
            RemovalCallback = removalCallback;
        }

        private static void HeightChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is InterLineAdornmentTag lineAdornmentTag))
                return;
            var heightChanged = lineAdornmentTag.HeightChanged;
            heightChanged?.Invoke(d, e);
        }

        private static void HorizontalOffsetChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var lineAdornmentTag = d as InterLineAdornmentTag;
            var horizontalOffsetChanged = lineAdornmentTag?.HorizontalOffsetChanged;
            horizontalOffsetChanged?.Invoke(d, e);
        }
    }
}