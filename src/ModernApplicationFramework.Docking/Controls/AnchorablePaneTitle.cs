/************************************************************************

   AvalonDock

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the New BSD
   License (BSD) as published at http://avalondock.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up AvalonDock in Extended WPF Toolkit Plus at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like facebook.com/datagrids

  **********************************************************************/

using System.Windows;
using System.Windows.Controls;
using ModernApplicationFramework.Docking.Layout;

namespace ModernApplicationFramework.Docking.Controls
{
    public class AnchorablePaneTitle : Control //DragUndockHeader
    {
        public static readonly DependencyProperty ModelProperty =
            DependencyProperty.Register("Model", typeof(LayoutContent), typeof(AnchorablePaneTitle),
                new FrameworkPropertyMetadata(null, OnModelChanged));

        private static readonly DependencyPropertyKey LayoutItemPropertyKey
            = DependencyProperty.RegisterReadOnly("LayoutItem", typeof(LayoutItem), typeof(AnchorablePaneTitle),
                new FrameworkPropertyMetadata((LayoutItem)null));

        public static readonly DependencyProperty LayoutItemProperty
            = LayoutItemPropertyKey.DependencyProperty;

        public LayoutItem LayoutItem => (LayoutItem)GetValue(LayoutItemProperty);

        public LayoutContent Model
        {
            get => (LayoutContent)GetValue(ModelProperty);
            set => SetValue(ModelProperty, value);
        }

        private bool _isMouseDown;

        static AnchorablePaneTitle()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (AnchorablePaneTitle),
                new FrameworkPropertyMetadata(typeof (AnchorablePaneTitle)));
        }

        //protected override void OnMouseLeave(MouseEventArgs e)
        //{
        //    base.OnMouseLeave(e);

        //    if (_isMouseDown && e.LeftButton == MouseButtonState.Pressed)
        //    {
        //        var pane = this.FindVisualAncestor<LayoutAnchorablePaneControl>();
        //        if (pane?.Model is LayoutAnchorablePane paneModel)
        //        {
        //            var manager = paneModel.Root.Manager;

        //            manager.StartDraggingFloatingWindowForPane(paneModel);
        //        }
        //    }

        //    _isMouseDown = false;
        //}

        //protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        //{
        //    base.OnPreviewMouseLeftButtonDown(e);
        //    if (e.Handled)
        //        return;
        //    Model.IsActive = true;
        //    Model.IsSelected = true;
        //}

        //protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        //{
        //    base.OnMouseLeftButtonDown(e);
        //    if (e.Handled)
        //        return;

        //    bool attachFloatingWindow = false;
        //    var parentFloatingWindow = Model.FindParent<LayoutAnchorableFloatingWindow>();
        //    if (parentFloatingWindow != null)
        //        attachFloatingWindow = parentFloatingWindow.Descendents().OfType<LayoutAnchorablePane>().Count() == 1;


        //    if (parentFloatingWindow != null)
        //    {
        //        if (e.ClickCount == 2)
        //        {
        //            if (NativeMethods.NativeMethods.IsKeyPressed(17))
        //                Model.Root.Manager.GetLayoutItemFromModel(Model).DockAsDocumentCommand.Execute(null);
        //            return;
        //        }

        //    }

        //    if (attachFloatingWindow)
        //    {
        //        //the pane is hosted inside a floating window that contains only an anchorable pane so drag the floating window itself
        //        var floatingWndControl =
        //            Model.Root.Manager.FloatingWindows.Single(fwc => Equals(fwc.Model, parentFloatingWindow));
        //        floatingWndControl.AttachDrag(false);
        //    }
        //    else
        //    {
        //        _isMouseDown = true; //normal drag

        //        if (e.ClickCount != 2)
        //            return;
        //        if (NativeMethods.NativeMethods.IsKeyPressed(17))
        //            Model.Float();
        //    }
        //}

        //protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        //{
        //    _isMouseDown = false;
        //    base.OnMouseLeftButtonUp(e);

        //    if (Model != null)
        //        Model.IsActive = true; //FocusElementManager.SetFocusOnLastElement(Model);
        //}

        //protected override void OnMouseMove(MouseEventArgs e)
        //{
        //    if (e.LeftButton != MouseButtonState.Pressed)
        //        _isMouseDown = false;

        //    base.OnMouseMove(e);
        //}

        private static void OnModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AnchorablePaneTitle)d).OnModelChanged(e);
        }

        private void OnModelChanged(DependencyPropertyChangedEventArgs e)
        {
            SetLayoutItem(Model?.Root.Manager.GetLayoutItemFromModel(Model));
        }

        protected void SetLayoutItem(LayoutItem value)
        {
            SetValue(LayoutItemPropertyKey, value);
        }

    }
}