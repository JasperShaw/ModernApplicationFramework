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
using System.Windows.Input;
using ModernApplicationFramework.Docking.Layout;

namespace ModernApplicationFramework.Docking.Controls
{
    public class AnchorablePaneTitle : Control
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

        static AnchorablePaneTitle()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (AnchorablePaneTitle),
                new FrameworkPropertyMetadata(typeof (AnchorablePaneTitle)));
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (!NativeMethods.NativeMethods.IsKeyPressed(17))
                    return;
                Model.Float();
                e.Handled = true;
            }
            base.OnMouseDoubleClick(e);
        }

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