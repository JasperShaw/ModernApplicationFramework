using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Modules.Toolbox.Controls
{
    [TemplatePart(Name = "PART_EmptyMessage", Type = typeof(UIElement))]
    internal class ToolboxTreeViewItem : TreeViewItem
    {
        private bool _doubleClicked;
        private DispatcherTimer _timer;
        private UIElement _emptyMessageHolder;


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _emptyMessageHolder = GetTemplateChild("PART_EmptyMessage") as UIElement;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Handled)
                return;
            switch (e.Key)
            {
                case Key.Right:
                case Key.Left:
                    if (!IsLogicalLeft(e.Key))
                    {
                        if (DataContext is IToolboxItem)
                            break;
                        if (!IsExpanded)
                        {
                            IsExpanded = true;
                            e.Handled = true;
                            break;
                        }
                        HandleLogicalRight();
                        e.Handled = true;
                        break;
                    }
                    if (DataContext is IToolboxItem)
                        break;
                    if (IsExpanded)
                        IsExpanded = false;
                    else
                        HandleLogicalLeft();
                    e.Handled = true;
                    break;
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (_doubleClicked || _emptyMessageHolder != null && _emptyMessageHolder.IsMouseOver)
                return;
            StartTimer();
            if (DataContext is ToolboxItemCategory)
                IsExpanded = !IsExpanded;
            e.Handled = true;
            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            e.Handled = true;
            base.OnMouseDoubleClick(e);
        }

        private void StartTimer()
        {
            if (_timer != null)
                return;
            _doubleClicked = true;
            _timer = new DispatcherTimer(TimeSpan.FromMilliseconds(GetDoubleClickTime()), DispatcherPriority.Input,
                (sender, e) => StopTimer(), Dispatcher);
            _timer.Start();
        }

        [DllImport("user32.dll")]
        public static extern uint GetDoubleClickTime();

        private void StopTimer()
        {
            _doubleClicked = false;
            if (_timer == null)
                return;
            _timer.Stop();
            _timer = null;
        }

        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            if (!IsKeyboardFocusWithin)
                Focus();
            base.OnPreviewMouseRightButtonDown(e);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ToolboxTreeViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is ToolboxTreeViewItem;
        }

        private bool IsLogicalLeft(Key key)
        {
            if (FlowDirection != FlowDirection.LeftToRight)
                return key == Key.Right;
            return key == Key.Left;
        }

        private void HandleLogicalRight()
        {
            MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
        }

        private void HandleLogicalLeft()
        {
            var parent = this.FindAncestorOrSelf<TreeView>();
            ((UIElement)parent?.ItemContainerGenerator.ContainerFromItem(parent))?.Focus();
        }
    }
}