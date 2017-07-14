using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ModernApplicationFramework.Controls.AutomationPeer;
using ModernApplicationFramework.Native.Standard;

namespace ModernApplicationFramework.Controls.Buttons
{
    public class SplitButtonItem : ListBoxItem
    {
        public static readonly DependencyProperty IsHighlightedProperty;

        public bool IsHighlighted
        {
            get => (bool)GetValue(IsHighlightedProperty);
            set => SetValue(IsHighlightedProperty, Boxes.Box(value));
        }

        static SplitButtonItem()
        {
            IsHighlightedProperty = DependencyProperty.Register("IsHighlighted", typeof(bool), typeof(SplitButtonItem), new FrameworkPropertyMetadata(Boxes.BooleanFalse));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitButtonItem), new FrameworkPropertyMetadata(typeof(SplitButtonItem)));
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            var parentButton = (SplitButton) ItemsControl.ItemsControlFromItemContainer(this);
            double transformedYValueOnMouseEnter;
            if (IsRealMouseEnter(parentButton, out transformedYValueOnMouseEnter))
            {
                parentButton.SelectedIndex = parentButton.ItemContainerGenerator.IndexFromContainer(this);
                parentButton.LastYValueOnMouseEnter = transformedYValueOnMouseEnter;
                Keyboard.Focus(this);
            }
            base.OnMouseEnter(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key != Key.Return)
                return;
            ExecuteAction();
            e.Handled = true;
        }

        protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer()
        {
            return new SplitButtonItemAutomationPeer(this);
        }

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            var splitButton = (SplitButton) ItemsControl.ItemsControlFromItemContainer(this);
            splitButton.SelectedIndex = splitButton.ItemContainerGenerator.IndexFromContainer(this);
            base.OnGotKeyboardFocus(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            ExecuteAction();
            e.Handled = true;
        }

        private void ExecuteAction()
        {
            ((SplitButton) ItemsControl.ItemsControlFromItemContainer(this)).Invoke();
        }

        private bool IsRealMouseEnter(SplitButton parentButton, out double transformedYValueOnMouseEnter)
        {
            var rect = TransformToAncestor(PresentationSource.FromVisual(this).RootVisual)
                .TransformBounds(new Rect(0.0, 0.0, ActualWidth, ActualHeight));
            transformedYValueOnMouseEnter = rect.Y;
            if (!double.IsNaN(parentButton.LastYValueOnMouseEnter))
                return Math.Round(parentButton.LastYValueOnMouseEnter) != Math.Round(rect.Y);
            return true;
        }
    }
}
