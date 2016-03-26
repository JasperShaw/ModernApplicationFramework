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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using ModernApplicationFramework.Docking.Layout;

namespace ModernApplicationFramework.Docking.Controls
{
    public class LayoutAnchorControl : Control, ILayoutControl
    {
        private static readonly DependencyPropertyKey SidePropertyKey
            = DependencyProperty.RegisterReadOnly("Side", typeof (AnchorSide), typeof (LayoutAnchorControl),
                new FrameworkPropertyMetadata(AnchorSide.Left));

        public static readonly DependencyProperty SideProperty = SidePropertyKey.DependencyProperty;


        private readonly LayoutAnchorable _model;

        DispatcherTimer _openUpTimer;

        internal LayoutAnchorControl(LayoutAnchorable model)
        {
            _model = model;
            _model.IsActiveChanged += _model_IsActiveChanged;
            _model.IsSelectedChanged += _model_IsSelectedChanged;

            SetSide(_model.FindParent<LayoutAnchorSide>().Side);
        }

        static LayoutAnchorControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (LayoutAnchorControl),
                new FrameworkPropertyMetadata(typeof (LayoutAnchorControl)));
            IsHitTestVisibleProperty.AddOwner(typeof (LayoutAnchorControl), new FrameworkPropertyMetadata(true));
        }

        public ILayoutElement Model => _model;
        public AnchorSide Side => (AnchorSide) GetValue(SideProperty);

        protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Handled)
                return;
            _model.Root.Manager.ShowAutoHideWindow(this);
            _model.IsActive = true;
        }

        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            if (!_model.ShowOnMouseOver)
                return;
            if (e.Handled)
                return;
            _openUpTimer = new DispatcherTimer(DispatcherPriority.ApplicationIdle)
            {
                Interval = TimeSpan.FromMilliseconds(400)
            };
            _openUpTimer.Tick += _openUpTimer_Tick;
            _openUpTimer.Start();
        }

        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
        {
            if (!_model.ShowOnMouseOver)
                return;
            if (_openUpTimer != null)
            {
                _openUpTimer.Tick -= _openUpTimer_Tick;
                _openUpTimer.Stop();
                _openUpTimer = null;
            }
            base.OnMouseLeave(e);
        }

        protected void SetSide(AnchorSide value)
        {
            SetValue(SidePropertyKey, value);
        }

        private void _model_IsActiveChanged(object sender, EventArgs e)
        {
            if (!_model.IsAutoHidden)
                _model.IsActiveChanged -= _model_IsActiveChanged;
            else if (_model.IsActive)
                _model.Root.Manager.ShowAutoHideWindow(this);
        }

        private void _model_IsSelectedChanged(object sender, EventArgs e)
        {
            if (!_model.IsAutoHidden)
                _model.IsSelectedChanged -= _model_IsSelectedChanged;
            else if (_model.IsSelected)
            {
                _model.Root.Manager.ShowAutoHideWindow(this);
                _model.IsSelected = false;
            }
        }

        void _openUpTimer_Tick(object sender, EventArgs e)
        {
            _openUpTimer.Tick -= _openUpTimer_Tick;
            _openUpTimer.Stop();
            _openUpTimer = null;
            _model.Root.Manager.ShowAutoHideWindow(this);
        }
    }
}