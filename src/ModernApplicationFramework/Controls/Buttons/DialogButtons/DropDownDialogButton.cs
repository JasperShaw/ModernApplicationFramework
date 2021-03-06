﻿using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ModernApplicationFramework.Controls.Buttons.DialogButtons
{
    /// <inheritdoc />
    /// <summary>
    /// A drop down button for dialog windows
    /// </summary>
    /// <seealso cref="DialogButton" />
    public class DropDownDialogButton: DialogButton
    {
        /// <summary>
        /// The drop down context menu
        /// </summary>
        public static readonly DependencyProperty DropDownMenuProperty = DependencyProperty.Register("DropDownMenu", typeof(System.Windows.Controls.ContextMenu), typeof(DropDownDialogButton));
        private bool _contentLoaded;

        public System.Windows.Controls.ContextMenu DropDownMenu
        {
            get => (System.Windows.Controls.ContextMenu)GetValue(DropDownMenuProperty);
            set => SetValue(DropDownMenuProperty, value);
        }

        public DropDownDialogButton()
        {
            InitializeComponent();
        }

        protected override void OnClick()
        {
            base.OnClick();
            DropDownMenu.PlacementTarget = this;
            DropDownMenu.Placement = PlacementMode.Bottom;
            DropDownMenu.Closed += OnDropDownMenuClosed;
        }

        private void OnDropDownMenuClosed(object sender, RoutedEventArgs args)
        {
            DropDownMenu.Closed -= OnDropDownMenuClosed;
            DropDownMenu.DataContext = null;
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (DropDownMenu.DataContext != null)
                e.Handled = true;
            else
                base.OnPreviewMouseDown(e);
        }

        public void InitializeComponent()
        {
            if (_contentLoaded)
                return;
            _contentLoaded = true;
            Application.LoadComponent(this,
                new Uri("/ModernApplicationFramework;component/Themes/Generic/DropDownDialogButton.xaml", UriKind.Relative));
        }

        public void Connect(int connectionId, object target)
        {
            _contentLoaded = true;
        }
    }
}
