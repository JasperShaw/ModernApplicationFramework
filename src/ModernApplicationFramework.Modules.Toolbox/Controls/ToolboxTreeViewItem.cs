using System;
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
        private DispatcherTimer _clickTimer;
        private bool _doubleClicked;
        private UIElement _emptyMessageHolder;
        private DispatcherTimer _hoverTimer;

        private bool _isDragHoveringElapsed;

        

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _emptyMessageHolder = GetTemplateChild("PART_EmptyMessage") as UIElement;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ToolboxTreeViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is ToolboxTreeViewItem;
        }

        protected override void OnDragLeave(DragEventArgs e)
        {
            base.OnDragLeave(e);
            if (!(DataContext is IToolboxCategory))
                return;
            StopHoverTimer();
            _isDragHoveringElapsed = false;
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

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            e.Handled = true;
            base.OnMouseDoubleClick(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (_doubleClicked || _emptyMessageHolder != null && _emptyMessageHolder.IsMouseOver)
                return;
            StartClickExpandTimer();
            if (DataContext is ToolboxItemCategory)
                IsExpanded = !IsExpanded;
            e.Handled = true;
            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnPreviewDragOver(DragEventArgs e)
        {
            base.OnPreviewDragOver(e);
            if (!(DataContext is IToolboxCategory))
                return;
            var dragData = e.Data.GetData(DragDrop.DragDrop.DataFormat.Name);
            if (dragData is IToolboxCategory)
                return;
            StartHoverTimer();
            if (!_isDragHoveringElapsed)
                return;
            IsExpanded = true;
        }

        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            if (!IsKeyboardFocusWithin)
                Focus();
            base.OnPreviewMouseRightButtonDown(e);
        }

        private void HandleLogicalLeft()
        {
            var parent = this.FindAncestorOrSelf<TreeView>();
            ((UIElement) parent?.ItemContainerGenerator.ContainerFromItem(parent))?.Focus();
        }

        private void HandleLogicalRight()
        {
            MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
        }

        private bool IsLogicalLeft(Key key)
        {
            if (FlowDirection != FlowDirection.LeftToRight)
                return key == Key.Right;
            return key == Key.Left;
        }

        private void SetHoverTimeElapsed()
        {
            _isDragHoveringElapsed = true;
            StopHoverTimer();
        }

        private void StartClickExpandTimer()
        {
            if (_clickTimer != null)
                return;
            _doubleClicked = true;
            _clickTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(NativeMethods.User32.GetDoubleClickTime()), DispatcherPriority.Input,
                (sender, e) => StopClickExpandTimer(), Dispatcher);
            _clickTimer.Start();
        }

        private void StartHoverTimer()
        {
            if (_hoverTimer != null)
                return;
            _hoverTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(1500), DispatcherPriority.Input,
                (sender, e) => SetHoverTimeElapsed(), Dispatcher);
        }

        private void StopClickExpandTimer()
        {
            _doubleClicked = false;
            if (_clickTimer == null)
                return;
            _clickTimer.Stop();
            _clickTimer = null;
        }

        private void StopHoverTimer()
        {
            if (_hoverTimer == null)
                return;
            _hoverTimer.Stop();
            _hoverTimer = null;
        }
    }
}