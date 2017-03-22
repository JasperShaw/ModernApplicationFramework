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
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Core.Themes;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Docking.Converters;
using ModernApplicationFramework.Docking.Layout;
using ModernApplicationFramework.Native.Platform.Enums;

namespace ModernApplicationFramework.Docking.Controls
{
    public class LayoutAnchorableFloatingWindowControl : LayoutFloatingWindowControl, IOverlayWindowHost
    {
        public static readonly DependencyProperty SingleContentLayoutItemProperty =
            DependencyProperty.Register("SingleContentLayoutItem", typeof (LayoutItem),
                typeof (LayoutAnchorableFloatingWindowControl),
                new FrameworkPropertyMetadata(null, OnSingleContentLayoutItemChanged));

        private readonly LayoutAnchorableFloatingWindow _model;
        private List<IDropArea> _dropAreas;
        private OverlayWindow _overlayWindow;

        internal LayoutAnchorableFloatingWindowControl(LayoutAnchorableFloatingWindow model)
            : base(model)
        {
            _model = model;
            HideWindowCommand = new RelayCommand(OnExecuteHideWindowCommand, CanExecuteHideWindowCommand);
            Loaded += LayoutAnchorableFloatingWindowControl_Loaded;
        }

        static LayoutAnchorableFloatingWindowControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (LayoutAnchorableFloatingWindowControl),
                new FrameworkPropertyMetadata(typeof (LayoutAnchorableFloatingWindowControl)));
        }

        IEnumerable<IDropArea> IOverlayWindowHost.GetDropAreas(LayoutFloatingWindowControl draggingWindow)
        {
            if (_dropAreas != null)
                return _dropAreas;

            _dropAreas = new List<IDropArea>();

            if (draggingWindow.Model is LayoutDocumentFloatingWindow)
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
        public ICommand HideWindowCommand { get; }

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

        protected override IntPtr FilterMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case (int)WindowsMessage.WmNclbuttondown: //Left button down on title -> start dragging over docking manager
                    if (wParam.ToInt32() == (int) HitTestValues.Htcaption)
                    {
                        _model.Descendents()
                            .OfType<LayoutAnchorablePane>()
                            .First(p => p.ChildrenCount > 0 && p.SelectedContent != null)
                            .SelectedContent.IsActive = true;
                        handled = true;
                    }
                    break;
            }
            return base.FilterMessage(hwnd, msg, wParam, lParam, ref handled);
        }

        protected override void RedockWindow()
        {
           var list = _model.Descendents().OfType<LayoutAnchorablePane>().ToList();
            foreach (var layoutAnchorablePane in list)
                layoutAnchorablePane?.SelectedContent.Dock();

        }

        protected override void OnClosed(EventArgs e)
        {
            var root = Model.Root;
            root.Manager.RemoveFloatingWindow(this);
            root.CollectGarbage();
            if (_overlayWindow != null)
            {
                _overlayWindow.Close();
                _overlayWindow = null;
            }

            base.OnClosed(e);

            if (!CloseInitiatedByUser)
                root.FloatingWindows.Remove(_model);

            _model.PropertyChanged -= _model_PropertyChanged;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (CloseInitiatedByUser && !KeepContentVisibleOnClose)
            {
                e.Cancel = true;
                _model.Descendents().OfType<LayoutAnchorable>().ToArray().ForEach(a => a.Hide());
            }

            base.OnClosing(e);
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            var manager = _model.Root.Manager;

            Content = manager.CreateUIElementForModel(_model.RootPanel);

            //SetBinding(VisibilityProperty, new Binding("IsVisible") { Source = _model, Converter = new BoolToVisibilityConverter(), Mode = BindingMode.OneWay, ConverterParameter = Visible.Hidden });

            //Issue: http://avalondock.codeplex.com/workitem/15036
            IsVisibleChanged += (s, args) =>
            {
                var visibilityBinding = GetBindingExpression(VisibilityProperty);
                if (IsVisible && (visibilityBinding == null))
                {
                    SetBinding(VisibilityProperty,
                        new Binding("IsVisible")
                        {
                            Source = _model,
                            Converter = new BoolToVisibilityConverter(),
                            Mode = BindingMode.OneWay,
                            ConverterParameter = Visibility.Hidden
                        });
                }
            };

            SetBinding(SingleContentLayoutItemProperty,
                new Binding("Model.SinglePane.SelectedContent")
                {
                    Source = this,
                    Converter = new LayoutItemFromLayoutModelConverter()
                });

            _model.PropertyChanged += _model_PropertyChanged;
        }

        protected virtual void OnSingleContentLayoutItemChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        private static void OnSingleContentLayoutItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutAnchorableFloatingWindowControl) d).OnSingleContentLayoutItemChanged(e);
        }

        private void _model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "RootPanel" &&
                _model.RootPanel == null)
            {
                InternalClose();
            }
        }

        private bool CanExecuteHideWindowCommand(object parameter)
        {
            var root = Model?.Root;

            var manager = root?.Manager;
            if (manager == null)
                return false;

            var canExecute = false;
            foreach (var anchorable in Model.Descendents().OfType<LayoutAnchorable>().ToArray())
            {
                if (!anchorable.CanHide)
                {
                    canExecute = false;
                    break;
                }

                var anchorableLayoutItem = manager.GetLayoutItemFromModel(anchorable) as LayoutAnchorableItem;
                if (anchorableLayoutItem?.HideCommand == null || !anchorableLayoutItem.HideCommand.CanExecute(parameter))
                {
                    canExecute = false;
                    break;
                }

                canExecute = true;
            }

            return canExecute;
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

        private void KeepOnTop(object sender, EventArgs e)
        {
            Topmost = true;
            Topmost = false;
        }

        private void LayoutAnchorableFloatingWindowControl_Loaded(object sender, RoutedEventArgs e)
        {
            Owner = GetWindow(Model.Root.Manager);
            if (Owner != null)
            {
                Owner.Activated += KeepOnTop;
                Owner.Deactivated += KeepOnTop;
            }

            //Do this so Shadow and border gets not clipped when this window is inside MainWindow
            ChangeOwner(new WindowInteropHelper(this).Handle);
        }

        private void OnExecuteHideWindowCommand(object parameter)
        {
            var manager = Model.Root.Manager;
            foreach (
                var anchorableLayoutItem in
                    Model.Descendents()
                        .OfType<LayoutAnchorable>()
                        .ToArray()
                        .Select(anchorable => manager.GetLayoutItemFromModel(anchorable) as LayoutAnchorableItem))
            {
                anchorableLayoutItem?.HideCommand.Execute(parameter);
            }
        }

        private bool OpenContextMenu()
        {
            var ctxMenu = _model.Root.Manager.AnchorableContextMenu;
            if (ctxMenu != null && SingleContentLayoutItem != null)
            {
                ctxMenu.PlacementTarget = null;
                ctxMenu.Placement = PlacementMode.MousePoint;
                ctxMenu.DataContext = SingleContentLayoutItem;
                ctxMenu.IsOpen = true;
                return true;
            }

            return false;
        }
    }
}