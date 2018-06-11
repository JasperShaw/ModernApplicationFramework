using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.DragDrop;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Modules.Toolbox.CommandBar;
using ModernApplicationFramework.Modules.Toolbox.Commands;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Items;
using ModernApplicationFramework.Modules.Toolbox.NativeMethods;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Modules.Toolbox.Controls
{
    internal class ToolboxTreeView : TreeView
    {
        private static readonly DependencyPropertyKey IsContextMenuOpenPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly("IsContextMenuOpen", typeof(bool), typeof(ToolboxTreeView),
                new FrameworkPropertyMetadata(Boxes.BooleanFalse,
                    FrameworkPropertyMetadataOptions.Inherits));


        private ContextMenuScope _contextMenuScope;

        public static readonly DependencyProperty IsContextMenuOpenProperty = IsContextMenuOpenPropertyKey.DependencyProperty;

        public ToolboxTreeView()
        {
            RegisterCommandHandlers(typeof(ToolboxTreeView));
        }

        private void EnterContextMenuVisualState()
        {
            _contextMenuScope = new ContextMenuScope(this);
        }

        private void LeaveContextMenuVisualState()
        {
            _contextMenuScope?.Dispose();
            _contextMenuScope = null;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ToolboxTreeViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is ToolboxTreeViewItem;
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);          
            var dragData = e.Data.GetData(DragDrop.DragDrop.DataFormat.Name);
            if (dragData is IToolboxCategory)
                return;
            if (this.IsDragElementUnderLastTreeItem(e, out var element))
                if (element is TreeViewItem tvi)
                    tvi.IsExpanded = true;
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            var dropInfo = new DropInfo(this, e, null);
            if (!IsTextObjectInDragSource(dropInfo, out var dataObject))
                return;

            dropInfo.Effects = DragDropEffects.Copy | DragDropEffects.Move;
            var tempItem = new ToolboxItem(string.Empty, dataObject, new[] { typeof(object) });

            if (!ToolboxDropHandler.CanDropToolboxItem(dropInfo, tempItem))
                e.Effects = DragDropEffects.None;
            e.Handled = true;
        }


        private static bool IsTextObjectInDragSource(IDropInfo dropInfo, out IDataObject stringData)
        {
            stringData = null;
            if (dropInfo.DragInfo == null && dropInfo.Data is IDataObject dataObject &&
                dropInfo.IsSameDragDropContextAsSource)
            {
                if (!dataObject.GetDataPresent(DataFormats.Text))
                {
                    return false;
                }
                stringData = dataObject;
            }
            return true;
        }


        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            if (!(e.OriginalSource is DependencyObject))
                return;
            if (ContextMenu == null)
                return;

            EnterContextMenuVisualState();
            var point = GetContextMenuLocation();

            ContextMenu.Placement = PlacementMode.Absolute;
            ContextMenu.VerticalOffset = point.Y;
            ContextMenu.HorizontalOffset = point.X;
            ContextMenu.PlacementTarget = this;
            ContextMenu.Closed += ContextMenu_Closed;

            ContextMenu.IsOpen = true;
            e.Handled = true;
        }

        private void ContextMenu_Closed(object sender, RoutedEventArgs e)
        {
            if (sender is ContextMenu contextMenu)
                contextMenu.Closed -= ContextMenu_Closed;
            LeaveContextMenuVisualState();
        }

        private Point GetContextMenuLocation()
        {
            if (InputManager.Current.MostRecentInputDevice is KeyboardDevice)
            {
                if (Keyboard.FocusedElement is UIElement focusedElement)
                    return focusedElement.PointToScreen(new Point(0, focusedElement.RenderSize.Height));
            }
            var messagePos = User32.GetMessagePos();
            return new Point(NativeMethods.NativeMethods.SignedLoword(messagePos), NativeMethods.NativeMethods.SignedHiword(messagePos));
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            switch (e.Key)
            {
                case Key.Delete:
                    //Required as the Edit Command is not used for Categories
                    if (SelectedItem is IToolboxCategory)
                    {
                        OnDelete();
                        e.Handled = true;
                    }
                    break;
            }
        }

        private static void CanDelete(object sender, CanExecuteRoutedEventArgs e)
        {
            if (sender is ToolboxTreeView tv)
                e.CanExecute = tv.CanDelete();
        }

        private static void OnDelete(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is ToolboxTreeView tv)
            {
                tv.OnDelete();
                e.Handled = true;
            }
        }

        private static void RegisterCommandHandlers(Type controlType)
        {
            CommandHelpers.RegisterCommandHandler(controlType, EditingCommands.Delete, OnDelete, CanDelete);
        }



        private bool CanDelete()
        {
            return SelectedItem is IToolboxItem;
        }

        private void OnDelete()
        {
            if (SelectedItem is IToolboxCategory)
                IoC.Get<DeleteActiveToolbarCategoryCommandDefinition>().Command.Execute(null);
            if (SelectedItem is IToolboxItem item)
                item.Parent?.RemoveItem(item);
        }

        private static void SetIsContextMenuOpen(UIElement element, bool value)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            element.SetValue(IsContextMenuOpenPropertyKey, Boxes.Box(value));
        }


        private class ContextMenuScope : DisposableObject
        {
            private readonly ToolboxTreeView _view;

            public ContextMenuScope(ToolboxTreeView view)
            {
                _view = view;
                SetIsContextMenuOpen(view, true);
            }

            protected override void DisposeManagedResources()
            {
                SetIsContextMenuOpen(_view, false);
            }
        }
    }
}