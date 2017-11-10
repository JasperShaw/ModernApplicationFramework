using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ModernApplicationFramework.Controls.Utilities;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Controls.InfoBar.Internal
{
    internal sealed class InfoBarTextPanel : FrameworkElement
    {
        public static readonly DependencyProperty MainTextControlProperty = DependencyProperty.Register(nameof(MainTextControl), typeof(FrameworkElement), typeof(InfoBarTextPanel), new FrameworkPropertyMetadata(OnChildElementPropertyChanged));
        public static readonly DependencyProperty ActionItemsControlProperty = DependencyProperty.Register(nameof(ActionItemsControl), typeof(FrameworkElement), typeof(InfoBarTextPanel), new FrameworkPropertyMetadata(OnChildElementPropertyChanged));
        public static readonly DependencyProperty MainTextMinWidthProperty = DependencyProperty.Register(nameof(MainTextMinWidth), typeof(double), typeof(InfoBarTextPanel), new FrameworkPropertyMetadata(150.0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange), IsNonNegativeValueValid);
        public static readonly DependencyProperty ActionItemsMinWidthProperty = DependencyProperty.Register(nameof(ActionItemsMinWidth), typeof(double), typeof(InfoBarTextPanel), new FrameworkPropertyMetadata(72.0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange), IsNonNegativeValueValid);
        public static readonly DependencyProperty HorizontalSpacingProperty = DependencyProperty.Register(nameof(HorizontalSpacing), typeof(double), typeof(InfoBarTextPanel), new FrameworkPropertyMetadata(22.0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange), IsNonNegativeValueValid);
        public static readonly DependencyProperty VerticalSpacingProperty = DependencyProperty.Register(nameof(VerticalSpacing), typeof(double), typeof(InfoBarTextPanel), new FrameworkPropertyMetadata(4.0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange), IsNonNegativeValueValid);
        private readonly UIElementCollection _children;

        public FrameworkElement MainTextControl
        {
            get => (FrameworkElement)GetValue(MainTextControlProperty);
            set => SetValue(MainTextControlProperty, value);
        }

        public FrameworkElement ActionItemsControl
        {
            get => (FrameworkElement)GetValue(ActionItemsControlProperty);
            set => SetValue(ActionItemsControlProperty, value);
        }

        public double MainTextMinWidth
        {
            get => (double)GetValue(MainTextMinWidthProperty);
            set => SetValue(MainTextMinWidthProperty, Boxes.Box(value));
        }

        public double ActionItemsMinWidth
        {
            get => (double)GetValue(ActionItemsMinWidthProperty);
            set => SetValue(ActionItemsMinWidthProperty, Boxes.Box(value));
        }

        public double HorizontalSpacing
        {
            get => (double)GetValue(HorizontalSpacingProperty);
            set => SetValue(HorizontalSpacingProperty, Boxes.Box(value));
        }

        public double VerticalSpacing
        {
            get => (double)GetValue(VerticalSpacingProperty);
            set => SetValue(VerticalSpacingProperty, Boxes.Box(value));
        }

        private double MinimumSingleLineWidth => MainTextMinWidth + ActionItemsMinWidth + HorizontalSpacing;

        public InfoBarTextPanel()
        {
            _children = new UIElementCollection(this, null);
        }

        protected override int VisualChildrenCount => _children.Count;

        protected override Visual GetVisualChild(int index)
        {
            return _children[index];
        }

        private static bool IsNonNegativeValueValid(object value)
        {
            double num = (double)value;
            if (!ExtensionMethods.IsNonreal(num))
                return num >= 0.0;
            return false;
        }

        private static void OnChildElementPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            InfoBarTextPanel infoBarTextPanel = (InfoBarTextPanel)obj;
            FrameworkElement oldValue = (FrameworkElement)args.OldValue;
            FrameworkElement newValue = (FrameworkElement)args.NewValue;
            if (oldValue != null)
                infoBarTextPanel._children.Remove(oldValue);
            if (newValue == null)
                return;
            infoBarTextPanel._children.Add(newValue);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            FrameworkElement mainTextControl = MainTextControl;
            FrameworkElement actionItemsControl = ActionItemsControl;
            if (mainTextControl == null || actionItemsControl == null)
                return base.MeasureOverride(availableSize);
            bool flag = false;
            if (ExtensionMethods.IsNonreal(availableSize.Width))
            {
                mainTextControl.Measure(availableSize);
                actionItemsControl.Measure(availableSize);
            }
            else if (availableSize.Width < MinimumSingleLineWidth)
            {
                mainTextControl.Measure(availableSize);
                actionItemsControl.Measure(availableSize);
                flag = true;
            }
            else
            {
                Size availableSize1 = new Size(double.PositiveInfinity, availableSize.Height);
                actionItemsControl.Measure(availableSize1);
                double width = Math.Max(ActionItemsMinWidth, availableSize.Width - MainTextMinWidth - HorizontalSpacing);
                if (actionItemsControl.DesiredSize.Width > width)
                {
                    Size availableSize2 = new Size(width, availableSize.Height);
                    actionItemsControl.Measure(availableSize2);
                }
                Size availableSize3 = new Size(Math.Max(MainTextMinWidth, availableSize.Width - actionItemsControl.DesiredSize.Width - HorizontalSpacing), availableSize.Height);
                mainTextControl.Measure(availableSize3);
            }
            Size desiredSize1 = mainTextControl.DesiredSize;
            Size desiredSize2 = actionItemsControl.DesiredSize;
            Size size = new Size();
            if (flag)
            {
                size.Width = Math.Max(desiredSize1.Width, desiredSize2.Width);
                size.Height = desiredSize1.Height + desiredSize2.Height;
                if (desiredSize1.Height != 0.0 && desiredSize2.Height != 0.0)
                    size.Height += VerticalSpacing;
            }
            else
            {
                size.Width = desiredSize1.Width + desiredSize2.Width;
                size.Height = Math.Max(desiredSize1.Height, desiredSize2.Height);
            }
            return size;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            FrameworkElement mainTextControl = MainTextControl;
            FrameworkElement actionItemsControl = ActionItemsControl;
            if (mainTextControl == null || actionItemsControl == null)
                return base.ArrangeOverride(finalSize);
            bool flag = finalSize.Width < MinimumSingleLineWidth;
            Size desiredSize1 = mainTextControl.DesiredSize;
            Size desiredSize2 = actionItemsControl.DesiredSize;
            Rect finalRect1;
            Rect finalRect2;
            if (flag)
            {
                finalRect1 = new Rect(0.0, 0.0, finalSize.Width, desiredSize1.Height);
                finalRect2 = new Rect(0.0, desiredSize1.Height + VerticalSpacing, finalSize.Width, desiredSize2.Height);
            }
            else
            {
                finalRect1 = new Rect(0.0, 0.0, desiredSize1.Width, finalSize.Height);
                int num = 0;
                if (finalSize.Height > desiredSize2.Height)
                    num = (int)(finalSize.Height - desiredSize2.Height) / 2 + 1;
                finalRect2 = new Rect(desiredSize1.Width + HorizontalSpacing, num, desiredSize2.Width, desiredSize2.Height);
            }
            mainTextControl.Arrange(finalRect1);
            actionItemsControl.Arrange(finalRect2);
            return finalSize;
        }
    }
}
