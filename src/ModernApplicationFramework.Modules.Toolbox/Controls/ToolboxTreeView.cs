using System;
using System.Collections.Generic;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.DragDrop;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Modules.Toolbox.CommandDefinitions;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Interfaces.Commands;
using ModernApplicationFramework.Modules.Toolbox.Items;
using ModernApplicationFramework.Modules.Toolbox.NativeMethods;
using ModernApplicationFramework.Modules.Toolbox.Utilities;
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
        private IToolbox _toolboxModel;

        private IToolbox ToolboxModel => _toolboxModel ?? (_toolboxModel = IoC.Get<IToolbox>());

        public ToolboxTreeView()
        {
            RegisterCommandHandlers(typeof(ToolboxTreeView));
        }

        private IDisposable EnterContextMenuVisualState()
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

        protected override void OnDragEnter(DragEventArgs e)
        {
            var dropInfo = new DropInfo(this, e, null);
            if (!IsTextObjectInDragSource(dropInfo, out _))
                return;

            dropInfo.Effects = DragDropEffects.Copy | DragDropEffects.Move;

            var tempItem = new ToolboxItem(new ToolboxItemDefinition(string.Empty, null, new List<Type>()));

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

            using (EnterContextMenuVisualState())
            {
                IoC.Get<IMafUIShell>().ShowContextMenu(point, ToolboxModel.ContextMenuId, this);
                e.Handled = true;
            }
        }

        private Point GetContextMenuLocation()
        {
            var p = new Point();
            if (InputManager.Current.MostRecentInputDevice is KeyboardDevice)
            {
                if (Keyboard.FocusedElement is UIElement focusedElement)
                    p = focusedElement.PointToScreen(new Point(0, focusedElement.RenderSize.Height));
            }
            else
            {
                var messagePos = User32.GetMessagePos();
                p = new Point(NativeMethods.NativeMethods.SignedLoword(messagePos), NativeMethods.NativeMethods.SignedHiword(messagePos));
            }
            return DpiHelper.Default.DeviceToLogicalUnits(p);
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

        private static void OnPaste(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is ToolboxTreeView tv)
            {
                tv.OnPaste();
                e.Handled = true;
            }

        }

        private static void CanPaste(object sender, CanExecuteRoutedEventArgs e)
        {
            if (sender is ToolboxTreeView tv)
                e.CanExecute = tv.CanPaste();

        }

        private static void RegisterCommandHandlers(Type controlType)
        {
            CommandHelpers.RegisterCommandHandler(controlType, EditingCommands.Delete, OnDelete, CanDelete);
            CommandHelpers.RegisterCommandHandler(controlType, ApplicationCommands.Paste, OnPaste, CanPaste);
            CommandHelpers.RegisterCommandHandler(controlType, ApplicationCommands.Copy, OnCopy, CanCopy);
            CommandHelpers.RegisterCommandHandler(controlType, ApplicationCommands.Cut, OnCut, CanCut);
        }

        private static void CanCut(object sender, CanExecuteRoutedEventArgs e)
        {
            if (sender is ToolboxTreeView tv)
                e.CanExecute = tv.CanCopy();
        }

        private static void OnCut(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is ToolboxTreeView tv)
            {
                tv.OnCopy(true);
                e.Handled = true;
            }
        }

        private static void CanCopy(object sender, CanExecuteRoutedEventArgs e)
        {
            if (sender is ToolboxTreeView tv)
                e.CanExecute = tv.CanCopy();
        }

        private static void OnCopy(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is ToolboxTreeView tv)
            {
                tv.OnCopy();
                e.Handled = true;
            }
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
            {
                var command = DeleteActiveItemCommandDefinition.Instance;
                command.Command.Execute(item);
            }
        }

        private bool CanPaste()
        {
            if (SelectedItem == null)
                return false;

            if (Clipboard.ContainsData(ToolboxItemDataFormats.DataSource) || Clipboard.ContainsData(DataFormats.Text))
                return true;
            return false;

            //var t = Clipboard.GetDataObject();
            //if (t == null)
            //    return false;

            //var f = t.GetFormats();

            //if (f.Any(x => x == ToolboxItemDataFormats.DataSource))
            //    return true;

            //if (f.Any(x => x == DataFormats.Text))
            //    return true;

            //return false;
        }

        [SecurityCritical]
        private void OnPaste()
        {
            IoC.Get<IAddItemToSelectedNodeCommand>().Execute(Clipboard.GetDataObject());
        }

        private bool CanCopy()
        {
            if (SelectedItem == null)
                return false;
            if (!(SelectedItem is IToolboxItem))
                return false;
            return true;
        }

        private void OnCopy(bool cut = false)
        {
            IoC.Get<ICopySelectedItemCommand>().Execute(cut);
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