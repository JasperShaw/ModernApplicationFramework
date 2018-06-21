using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Controls.Utilities
{
    public static class TextToolTipService
    {
        public static readonly DependencyProperty AutoShowToolTipWhenObscuredProperty = DependencyProperty.RegisterAttached("AutoShowToolTipWhenObscured", typeof(bool), typeof(TextToolTipService), new FrameworkPropertyMetadata(Boxes.BooleanFalse, OnAutoShowToolTipWhenObscuredChanged));
        public static readonly DependencyProperty ToolTipBoundsProperty = DependencyProperty.RegisterAttached("ToolTipBounds", typeof(Rect), typeof(TextToolTipService), new FrameworkPropertyMetadata(Rect.Empty));
        public static readonly DependencyProperty ToolTipTextProperty = DependencyProperty.RegisterAttached("ToolTipText", typeof(string), typeof(TextToolTipService), new FrameworkPropertyMetadata(null));
        private static readonly DependencyProperty TextToolTipManagerProperty = DependencyProperty.RegisterAttached("TextToolTipManager", typeof(TextToolTipManager), typeof(TextToolTipService), new FrameworkPropertyMetadata(null, OnTextToolTipManagerChanged));

        public static bool GetAutoShowToolTipWhenObscured(FrameworkElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            return (bool)element.GetValue(AutoShowToolTipWhenObscuredProperty);
        }

        public static void SetAutoShowToolTipWhenObscured(FrameworkElement element, bool value)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            element.SetValue(AutoShowToolTipWhenObscuredProperty, Boxes.Box(value));
        }

        public static Rect GetToolTipBounds(FrameworkElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            return (Rect)element.GetValue(ToolTipBoundsProperty);
        }

        public static void SetToolTipBounds(FrameworkElement element, Rect value)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            element.SetValue(ToolTipBoundsProperty, value);
        }

        public static void SetToolTipOffsets(FrameworkElement element, int horizontal, int vertical)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            ToolTip toolTip = element.ToolTip as ToolTip;
            if (toolTip == null)
                return;
            toolTip.HorizontalOffset = horizontal;
            toolTip.VerticalOffset = vertical;
        }

        public static void SetToolTipPlacement(FrameworkElement element, PlacementMode mode)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            ToolTip toolTip = element.ToolTip as ToolTip;
            if (toolTip == null)
                return;
            toolTip.Placement = mode;
        }

        public static string GetToolTipText(FrameworkElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            return (string)element.GetValue(ToolTipTextProperty);
        }

        public static void SetToolTipText(FrameworkElement element, string value)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            element.SetValue(ToolTipTextProperty, value);
        }

        private static TextToolTipManager GetTextToolTipManager(DependencyObject element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            return (TextToolTipManager)element.GetValue(TextToolTipManagerProperty);
        }

        private static void SetTextToolTipManager(DependencyObject element, TextToolTipManager value)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            element.SetValue(TextToolTipManagerProperty, value);
        }

        private static void OnAutoShowToolTipWhenObscuredChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            bool newValue = (bool)e.NewValue;
            if (!(obj is FrameworkElement element))
                return;
            SetTextToolTipManager(element, newValue ? new TextToolTipManager(element) : null);
        }

        private static void OnTextToolTipManagerChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            TextToolTipManager oldValue = (TextToolTipManager)e.OldValue;
            TextToolTipManager newValue = (TextToolTipManager)e.NewValue;
            oldValue?.Detach();
            newValue?.Attach();
        }

        private class TextToolTipManager
        {
            public TextToolTipManager(FrameworkElement element)
            {
                Element = element;
            }

            private FrameworkElement Element { get; }

            public void Attach()
            {
                Element.AddHandler(FrameworkElement.ToolTipOpeningEvent, new ToolTipEventHandler(OnToolTipOpening));
                SetupToolTip();
            }

            public void Detach()
            {
                Element.RemoveHandler(FrameworkElement.ToolTipOpeningEvent, new ToolTipEventHandler(OnToolTipOpening));
            }

            private void SetupToolTip()
            {
                TextBlock textBlock = new TextBlock();
                textBlock.SetBinding(TextBlock.TextProperty, new Binding
                {
                    Source = Element,
                    Path = new PropertyPath(ToolTipTextProperty)
                });
                FrameworkElement element = Element;
                ToolTip toolTip = new ToolTip
                {
                    Placement = PlacementMode.Relative,
                    HorizontalOffset = -5.0,
                    VerticalOffset = -3.0,
                    Content = textBlock
                };
                element.ToolTip = toolTip;
            }

            private void OnToolTipOpening(object sender, ToolTipEventArgs e)
            {
                Rect toolTipBounds = GetToolTipBounds(Element);
                if (!toolTipBounds.IsEmpty)
                {
                    Point position = Mouse.GetPosition(Element);
                    e.Handled = !toolTipBounds.Contains(position);
                }
                else
                    e.Handled = !Element.IsTrimmed() && !Element.IsClipped();
            }
        }
    }
}
