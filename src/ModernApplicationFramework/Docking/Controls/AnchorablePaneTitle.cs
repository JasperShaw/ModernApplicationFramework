/************************************************************************

   AvalonDock

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the New BSD
   License (BSD) as published at http://avalondock.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up AvalonDock in Extended WPF Toolkit Plus at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like facebook.com/datagrids

  **********************************************************************/

using System.Linq;
using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Docking.Layout;
using ModernApplicationFramework.Native.NativeMethods;

namespace ModernApplicationFramework.Docking.Controls
{
    public class AnchorablePaneTitle : DragUndockHeader
    {
        private bool _isMouseDown;

        static AnchorablePaneTitle()
        {
            IsHitTestVisibleProperty.OverrideMetadata(typeof (AnchorablePaneTitle), new FrameworkPropertyMetadata(true));
            FocusableProperty.OverrideMetadata(typeof (AnchorablePaneTitle), new FrameworkPropertyMetadata(false));
            DefaultStyleKeyProperty.OverrideMetadata(typeof (AnchorablePaneTitle),
                new FrameworkPropertyMetadata(typeof (AnchorablePaneTitle)));
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            if (_isMouseDown && e.LeftButton == MouseButtonState.Pressed)
            {
                var pane = this.FindVisualAncestor<LayoutAnchorablePaneControl>();
                if (pane?.Model is LayoutAnchorablePane paneModel)
                {
                    var manager = paneModel.Root.Manager;

                    manager.StartDraggingFloatingWindowForPane(paneModel);
                }
            }

            _isMouseDown = false;
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);
            if (e.Handled)
                return;
            Model.IsActive = true;
            Model.IsSelected = true;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.Handled)
                return;

            bool attachFloatingWindow = false;
            var parentFloatingWindow = Model.FindParent<LayoutAnchorableFloatingWindow>();
            if (parentFloatingWindow != null)
                attachFloatingWindow = parentFloatingWindow.Descendents().OfType<LayoutAnchorablePane>().Count() == 1;


            if (parentFloatingWindow != null)
            {
                if (e.ClickCount == 2)
                {
                    if (NativeMethods.IsKeyPressed(17))
                        Model.Root.Manager.GetLayoutItemFromModel(Model).DockAsDocumentCommand.Execute(null);
                    return;
                }

            }

            if (attachFloatingWindow)
            {
                //the pane is hosted inside a floating window that contains only an anchorable pane so drag the floating window itself
                var floatingWndControl =
                    Model.Root.Manager.FloatingWindows.Single(fwc => Equals(fwc.Model, parentFloatingWindow));
                floatingWndControl.AttachDrag(false);
            }
            else
            {
                _isMouseDown = true; //normal drag

                if (e.ClickCount != 2)
                    return;
                if (NativeMethods.IsKeyPressed(17))
                    Model.Float();
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            _isMouseDown = false;
            base.OnMouseLeftButtonUp(e);

            if (Model != null)
                Model.IsActive = true; //FocusElementManager.SetFocusOnLastElement(Model);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                _isMouseDown = false;

            base.OnMouseMove(e);
        }
    }
}