using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Caliburn.Micro;
using ModernApplicationFramework.Controls.Buttons;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.NativeMethods;
using ModernApplicationFramework.Utilities;
using Action = System.Action;

namespace ModernApplicationFramework.Modules.Toolbox.Controls
{
    [TemplatePart(Name = "PART_EmptyMessage", Type = typeof(UIElement))]
    [TemplatePart(Name = "PART_EditBox", Type = typeof(TextBox))]
    internal class ToolboxTreeViewItem : TreeViewItem
    {
        private DispatcherTimer _clickTimer;
        private IToolboxNode _currentNode;
        private bool _doubleClicked;
        private TextBox _editBox;
        private UIElement _emptyMessageHolder;
        private bool _errorShowing;
        private DispatcherTimer _hoverTimer;

        private bool _isDragHoveringElapsed;

        public ToolboxTreeViewItem()
        {
            DataContextChanged += ToolboxTreeViewItem_DataContextChanged;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _emptyMessageHolder = GetTemplateChild("PART_EmptyMessage") as UIElement;
            _editBox = GetTemplateChild("PART_EditBox") as TextBox;

            if (_editBox != null)
                _editBox.LostKeyboardFocus += _editBox_LostKeyboardFocus;
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
                case Key.Escape:
                    if (_currentNode.IsInRenameMode)
                        _currentNode.ExitRenameMode();
                    e.Handled = true;
                    break;
                case Key.Enter:
                    if (_currentNode.IsInRenameMode)
                        TryCommitRename();
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
            if (DataContext is IToolboxCategory)
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

        private void _editBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (InputManager.Current.IsInMenuMode)
            {
                if (!(e.NewFocus is ContextMenu newFocus) || !Equals(newFocus.PlacementTarget, _editBox))
                    if (!(e.NewFocus is MenuItem || e.NewFocus is CommandDefinitionButton))
                        _currentNode.ExitRenameMode();
            }
            else
            {
                TryCommitRename(true);
            }
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

        private void Node_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IToolboxNode.IsInRenameMode))
                TriggerRename(_currentNode.IsInRenameMode);
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
            _clickTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(User32.GetDoubleClickTime()),
                DispatcherPriority.Input,
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

        private void ToolboxTreeViewItem_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_currentNode != null)
                _currentNode.PropertyChanged -= Node_PropertyChanged;
            if (e.NewValue != null && e.NewValue is IToolboxNode node)
            {
                _currentNode = node;
                node.PropertyChanged += Node_PropertyChanged;
                if (node.IsNewlyCreated)
                    TriggerRename(true);
            }
        }

        private void TriggerRename(bool isRenameMode)
        {
            if (isRenameMode)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Input,
                    new Action(delegate
                    {
                        Focus();
                        Keyboard.Focus(_editBox);
                    }));
                _editBox?.SelectAll();
            }
        }

        private void TryCommitRename(bool silentDiscard = false)
        {
            if (_currentNode == null || _errorShowing)
                return;
            if (_currentNode.IsRenameValid(out var message))
            {
                _currentNode.CommitRename();
            }
            else
            {
                if (silentDiscard)
                {
                    _currentNode.ExitRenameMode();
                    return;
                }

                IoC.Get<IMafUIShell>().GetAppName(out var appName);
                _errorShowing = true;
                MessageBox.Show(message, appName, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                _errorShowing = false;
                TriggerRename(true);
            }
        }
    }
}