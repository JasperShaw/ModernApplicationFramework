/************************************************************************

   AvalonDock

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the New BSD
   License (BSD) as published at http://avalondock.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up AvalonDock in Extended WPF Toolkit Plus at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like facebook.com/datagrids

  **********************************************************************/

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.Primitives;
using ModernApplicationFramework.Core.Themes;
using ModernApplicationFramework.Docking.Layout;

namespace ModernApplicationFramework.Docking.Controls
{
    public class LayoutDocumentFloatingWindowControl : LayoutFloatingWindowControl, IOverlayWindowHost
    {
        public static readonly DependencyProperty SingleContentLayoutItemProperty =
            DependencyProperty.Register("SingleContentLayoutItem", typeof (LayoutItem),
                typeof (LayoutDocumentFloatingWindowControl),
                new FrameworkPropertyMetadata(null, OnSingleContentLayoutItemChanged));

        private readonly LayoutDocumentFloatingWindow _model;
        private List<IDropArea> _dropAreas;
        private OverlayWindow _overlayWindow;

        internal LayoutDocumentFloatingWindowControl(LayoutDocumentFloatingWindow model)
            : base(model)
        {
            _model = model;
            ShowInTaskbar = true;
            Loaded += LayoutDocumentFloatingWindowControl_Loaded;
        }

        static LayoutDocumentFloatingWindowControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (LayoutDocumentFloatingWindowControl),
                new FrameworkPropertyMetadata(typeof (LayoutDocumentFloatingWindowControl)));
        }

        IEnumerable<IDropArea> IOverlayWindowHost.GetDropAreas(LayoutFloatingWindowControl draggingWindow)
        {
            if (_dropAreas != null)
                return _dropAreas;
            _dropAreas = new List<IDropArea>();

            if (draggingWindow.Model is LayoutAnchorableFloatingWindow)
                return _dropAreas;

            var floatingWindowContentHost = Content as FloatingWindowContentHost;
            if (floatingWindowContentHost != null)
            {
                var rootVisual = floatingWindowContentHost.RootVisual;

                foreach (var areaHost in rootVisual.FindVisualChildren<LayoutAnchorablePaneControl>())
                {
                    _dropAreas.Add(new DropArea<LayoutAnchorablePaneControl>(
                        areaHost,
                        DropAreaType.AnchorablePane));
                }
                foreach (var areaHost in rootVisual.FindVisualChildren<LayoutDocumentPaneControl>())
                {
                    _dropAreas.Add(new DropArea<LayoutDocumentPaneControl>(
                        areaHost,
                        DropAreaType.DocumentPane));
                }
            }

            return _dropAreas;
        }

        void IOverlayWindowHost.HideOverlayWindow()
        {
            _dropAreas = null;
            _overlayWindow.Owner = null;
            _overlayWindow.HideDropTargets();
        }

        bool IOverlayWindowHost.HitTest(Point dragPoint)
        {
            Rect detectionRect = new Rect(this.PointToScreenDpiWithoutFlowDirection(new Point()),
                this.TransformActualSizeToAncestor());
            return detectionRect.Contains(dragPoint);
        }

        DockingManager IOverlayWindowHost.Manager => _model.Root.Manager;

        IOverlayWindow IOverlayWindowHost.ShowOverlayWindow(LayoutFloatingWindowControl draggingWindow)
        {
            CreateOverlayWindow();
            _overlayWindow.Owner = draggingWindow;
            _overlayWindow.EnableDropTargets();
            _overlayWindow.Show();

            return _overlayWindow;
        }

        public override ILayoutElement Model => _model;
        public LayoutItem RootDocumentLayoutItem => _model.Root.Manager.GetLayoutItemFromModel(_model.RootDocument);

        public LayoutItem SingleContentLayoutItem
        {
            get => (LayoutItem) GetValue(SingleContentLayoutItemProperty);
            set => SetValue(SingleContentLayoutItemProperty, value);
        }

        public override void ChangeTheme(Theme oldValue, Theme newValue)
        {
            base.ChangeTheme(oldValue, newValue);
            if (_overlayWindow != null)
                _overlayWindow.Theme = newValue;
        }

        public virtual void MaximizeRestoreButtonClick(object sender, RoutedEventArgs e)
        {
            //WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        public override void OnApplyTemplate()
        {
            var minimizeButton = GetTemplateChild("MaximizeRestoreButton") as System.Windows.Controls.Button;
            if (minimizeButton != null)
                minimizeButton.Click += MaximizeRestoreButtonClick;
            base.OnApplyTemplate();
        }

        protected override IntPtr FilterMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case Win32Helper.WmNclbuttondown: //Left button down on title -> start dragging over docking manager
                    if (wParam.ToInt32() == Win32Helper.HtCaption)
                    {
                        if (_model.RootDocument != null)
                            _model.RootDocument.IsActive = true;
                    }
                    break;
            }

            return base.FilterMessage(hwnd, msg, wParam, lParam, ref handled);
        }

        protected override void RedockWindow()
        {
            Model.Root.ActiveContent.Dock();
        }

        protected override void OnClosed(EventArgs e)
        {
            var root = Model.Root;
            root.Manager.RemoveFloatingWindow(this);
            root.CollectGarbage();

            base.OnClosed(e);

            if (!CloseInitiatedByUser)
            {
                root.FloatingWindows.Remove(_model);
            }

            _model.RootDocumentChanged -= _model_RootDocumentChanged;
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            if (_model.RootDocument == null)
            {
                InternalClose();
            }
            else
            {
                var manager = _model.Root.Manager;

                Content = manager.CreateUIElementForModel(_model.RootDocument);

                _model.RootDocumentChanged += _model_RootDocumentChanged;
            }
        }

        protected virtual void OnSingleContentLayoutItemChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        private static void OnSingleContentLayoutItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutDocumentFloatingWindowControl) d).OnSingleContentLayoutItemChanged(e);
        }

        private void _model_RootDocumentChanged(object sender, EventArgs e)
        {
            if (_model.RootDocument == null)
            {
                InternalClose();
            }
        }

        private void CreateOverlayWindow()
        {
            if (_overlayWindow == null)
                _overlayWindow = new OverlayWindow(this);
            var rectWindow = new Rect(this.PointToScreenDpiWithoutFlowDirection(new Point()),
                this.TransformActualSizeToAncestor());
            _overlayWindow.Left = rectWindow.Left;
            _overlayWindow.Top = rectWindow.Top;
            _overlayWindow.Width = rectWindow.Width;
            _overlayWindow.Height = rectWindow.Height;
        }

        private void LayoutDocumentFloatingWindowControl_Loaded(object sender, RoutedEventArgs e)
        {
           Owner = null;
        }

        private bool OpenContextMenu()
        {
            var ctxMenu = _model.Root.Manager.DocumentContextMenu;
            if (ctxMenu != null && RootDocumentLayoutItem != null)
            {
                ctxMenu.PlacementTarget = null;
                ctxMenu.Placement = PlacementMode.MousePoint;
                ctxMenu.DataContext = RootDocumentLayoutItem;
                ctxMenu.IsOpen = true;
                return true;
            }

            return false;
        }
    }
}