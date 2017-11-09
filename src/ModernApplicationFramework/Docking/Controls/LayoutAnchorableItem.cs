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
using System.Windows.Data;
using System.Windows.Input;
using ModernApplicationFramework.Docking.Layout;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Docking.Controls
{
    public class LayoutAnchorableItem : LayoutItem
    {
        public static readonly DependencyProperty HideCommandProperty =
            DependencyProperty.Register("HideCommand", typeof(ICommand), typeof(LayoutAnchorableItem),
                new FrameworkPropertyMetadata(null,
                    OnHideCommandChanged,
                    CoerceHideCommandValue));

        public static readonly DependencyProperty AutoHideCommandProperty =
            DependencyProperty.Register("AutoHideCommand", typeof(ICommand), typeof(LayoutAnchorableItem),
                new FrameworkPropertyMetadata(null,
                    OnAutoHideCommandChanged,
                    CoerceAutoHideCommandValue));

        public static readonly DependencyProperty DockCommandProperty =
            DependencyProperty.Register("DockCommand", typeof(ICommand), typeof(LayoutAnchorableItem),
                new FrameworkPropertyMetadata(null,
                    OnDockCommandChanged,
                    CoerceDockCommandValue));

        public static readonly DependencyProperty CanHideProperty =
            DependencyProperty.Register("CanHide", typeof(bool), typeof(LayoutAnchorableItem),
                new FrameworkPropertyMetadata(true, OnCanHideChanged));


        private readonly ReentrantFlag _visibilityReentrantFlag = new ReentrantFlag();
        private LayoutAnchorable _anchorable;

        private ICommand _defaultAutoHideCommand;
        private ICommand _defaultDockCommand;
        private ICommand _defaultHideCommand;


        public ICommand AutoHideCommand
        {
            get => (ICommand) GetValue(AutoHideCommandProperty);
            set => SetValue(AutoHideCommandProperty, value);
        }

        public bool CanHide
        {
            get => (bool) GetValue(CanHideProperty);
            set => SetValue(CanHideProperty, value);
        }

        public ICommand DockCommand
        {
            get => (ICommand) GetValue(DockCommandProperty);
            set => SetValue(DockCommandProperty, value);
        }

        public ICommand HideCommand
        {
            get => (ICommand) GetValue(HideCommandProperty);
            set => SetValue(HideCommandProperty, value);
        }

        internal LayoutAnchorableItem()
        {
        }

        internal override void Attach(LayoutContent model)
        {
            _anchorable = model as LayoutAnchorable;
            if (_anchorable != null)
                _anchorable.IsVisibleChanged += _anchorable_IsVisibleChanged;

            base.Attach(model);
        }

        internal override void Detach()
        {
            _anchorable.IsVisibleChanged -= _anchorable_IsVisibleChanged;
            _anchorable = null;
            base.Detach();
        }

        protected override void ClearDefaultBindings()
        {
            if (HideCommand == _defaultHideCommand)
                BindingOperations.ClearBinding(this, HideCommandProperty);
            if (AutoHideCommand == _defaultAutoHideCommand)
                BindingOperations.ClearBinding(this, AutoHideCommandProperty);
            if (DockCommand == _defaultDockCommand)
                BindingOperations.ClearBinding(this, DockCommandProperty);

            base.ClearDefaultBindings();
        }

        protected override void Close()
        {
            var dockingManager = _anchorable.Root.Manager;
            dockingManager._ExecuteCloseCommand(_anchorable);
        }

        protected override void Pin()
        {
            var dockingManager = _anchorable.Root.Manager;
            dockingManager._ExecutePinCommand(_anchorable);
        }

        protected override void InitDefaultCommands()
        {
            _defaultHideCommand = new DelegateCommand(ExecuteHideCommand, CanExecuteHideCommand);
            _defaultAutoHideCommand = new DelegateCommand(ExecuteAutoHideCommand, CanExecuteAutoHideCommand);
            _defaultDockCommand = new DelegateCommand(ExecuteDockCommand, CanExecuteDockCommand);

            base.InitDefaultCommands();
        }

        protected virtual void OnAutoHideCommandChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        protected virtual void OnCanHideChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_anchorable != null)
                _anchorable.CanHide = (bool) e.NewValue;
        }

        protected virtual void OnDockCommandChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        protected virtual void OnHideCommandChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        protected override void OnVisibilityChanged()
        {
            if (_anchorable?.Root != null)
                if (_visibilityReentrantFlag.CanEnter)
                    using (_visibilityReentrantFlag.Enter())
                    {
                        if (Visibility == Visibility.Hidden)
                            _anchorable.Hide(false);
                        else if (Visibility == Visibility.Visible)
                            _anchorable.Show();
                    }

            base.OnVisibilityChanged();
        }

        protected override void SetDefaultBindings()
        {
            if (HideCommand == null)
                HideCommand = _defaultHideCommand;
            if (AutoHideCommand == null)
                AutoHideCommand = _defaultAutoHideCommand;
            if (DockCommand == null)
                DockCommand = _defaultDockCommand;

            Visibility = _anchorable.IsVisible ? Visibility.Visible : Visibility.Hidden;
            base.SetDefaultBindings();
        }

        private static object CoerceAutoHideCommandValue(DependencyObject d, object value)
        {
            return value;
        }

        private static object CoerceDockCommandValue(DependencyObject d, object value)
        {
            return value;
        }

        private static object CoerceHideCommandValue(DependencyObject d, object value)
        {
            return value;
        }

        private static void OnAutoHideCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutAnchorableItem) d).OnAutoHideCommandChanged(e);
        }

        private static void OnCanHideChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutAnchorableItem) d).OnCanHideChanged(e);
        }

        private static void OnDockCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutAnchorableItem) d).OnDockCommandChanged(e);
        }

        private static void OnHideCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutAnchorableItem) d).OnHideCommandChanged(e);
        }

        private void _anchorable_IsVisibleChanged(object sender, EventArgs e)
        {
            if (_anchorable?.Root == null)
                return;
            if (!_visibilityReentrantFlag.CanEnter)
                return;
            using (_visibilityReentrantFlag.Enter())
            {
                Visibility = _anchorable.IsVisible ? Visibility.Visible : Visibility.Hidden;
            }
        }

        private bool CanExecuteAutoHideCommand(object parameter)
        {
            if (LayoutElement == null)
                return false;

            return LayoutElement.FindParent<LayoutAnchorableFloatingWindow>() == null && _anchorable.CanAutoHide;
        }

        private bool CanExecuteDockCommand(object parameter)
        {
            return LayoutElement?.FindParent<LayoutAnchorableFloatingWindow>() != null;
        }

        private bool CanExecuteHideCommand(object parameter)
        {
            return LayoutElement != null && _anchorable.CanHide;
        }

        private void ExecuteAutoHideCommand(object parameter)
        {
            _anchorable?.Root?.Manager?._ExecuteAutoHideCommand(_anchorable);
        }

        private void ExecuteDockCommand(object parameter)
        {
            LayoutElement.Root.Manager._ExecuteDockCommand(_anchorable);
            IsFloating = false;
            IsActive = true;
        }

        private void ExecuteHideCommand(object parameter)
        {
            _anchorable?.Root?.Manager?._ExecuteHideCommand(_anchorable);
        }
    }
}