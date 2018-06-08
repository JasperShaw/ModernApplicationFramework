using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Modules.Toolbox.CommandBar;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
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

        public static readonly DependencyProperty IsContextMenuOpenProperty = IsContextMenuOpenPropertyKey.DependencyProperty;

        public ToolboxTreeView()
        {
            RegisterCommandHandlers(typeof(ToolboxTreeView));
        }

        public IDisposable EnterContextMenuVisualState()
        {
            return new ContextMenuScope(this);
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

        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            if (!(e.OriginalSource is DependencyObject))
                return;
            if (ContextMenu == null)
                return;
            using (EnterContextMenuVisualState())
            {
                var point = GetContextMenuLocation();
                ContextMenu.Placement = PlacementMode.AbsolutePoint;
                ContextMenu.VerticalOffset = point.Y;
                ContextMenu.HorizontalOffset = point.X;
                ContextMenu.IsOpen = true;
                e.Handled = true;
            }
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