using System;
using System.Windows;

namespace ModernApplicationFramework.TextEditor
{
    public class InterLineAdornmentTag : DependencyObject, ITag
    {
        public static readonly DependencyProperty HeightProperty = DependencyProperty.Register(nameof(Height), typeof(double), typeof(InterLineAdornmentTag), new PropertyMetadata(0.0, HeightChangedCallback));
        public static readonly DependencyProperty HorizontalOffsetProperty = DependencyProperty.Register(nameof(HorizontalOffset), typeof(double), typeof(InterLineAdornmentTag), new PropertyMetadata(0.0, HorizontalOffsetChangedCallback));

        public InterLineAdornmentTag(InterLineAdornmentFactory adornmentFactory, bool isAboveLine, double initialHeight, HorizontalPositioningMode horizontalPositioningMode, double horizontalOffset, AdornmentRemovedCallback removalCallback = null)
        {
            if (adornmentFactory == null && removalCallback != null)
                throw new ArgumentException("Either adornmentFactory must be non-null or removalCallback must be null.");
            if (initialHeight < 0.0)
                throw new ArgumentException("initialHeight must be >= 0.");
            AdornmentFactory = adornmentFactory;
            IsAboveLine = isAboveLine;
            Height = initialHeight;
            HorizontalPositioningMode = horizontalPositioningMode;
            HorizontalOffset = horizontalOffset;
            RemovalCallback = removalCallback;
        }

        public InterLineAdornmentFactory AdornmentFactory { get; }

        public bool IsAboveLine { get; }

        public double Height
        {
            get => (double)GetValue(HeightProperty);
            set => SetValue(HeightProperty, value);
        }

        public event EventHandler<DependencyPropertyChangedEventArgs> HeightChanged;

        public bool IsAnimating { get; set; } = true;

        public HorizontalPositioningMode HorizontalPositioningMode { get; }

        public double HorizontalOffset
        {
            get => (double)GetValue(HorizontalOffsetProperty);
            set => SetValue(HorizontalOffsetProperty, value);
        }

        public event EventHandler<DependencyPropertyChangedEventArgs> HorizontalOffsetChanged;

        public AdornmentRemovedCallback RemovalCallback { get; }

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