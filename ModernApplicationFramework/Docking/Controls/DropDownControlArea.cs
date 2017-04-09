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
using System.Windows.Controls.Primitives;

namespace ModernApplicationFramework.Docking.Controls
{
    public class DropDownControlArea : UserControl
    {
        public static readonly DependencyProperty DropDownContextMenuProperty =
            DependencyProperty.Register("DropDownContextMenu", typeof (ContextMenu), typeof (DropDownControlArea),
                new FrameworkPropertyMetadata((ContextMenu) null));

        public ContextMenu DropDownContextMenu
        {
            get => (ContextMenu) GetValue(DropDownContextMenuProperty);
            set => SetValue(DropDownContextMenuProperty, value);
        }

        protected override void OnPreviewMouseRightButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnPreviewMouseRightButtonUp(e);

            if (e.Handled)
                return;
            if (DropDownContextMenu == null)
                return;
            DropDownContextMenu.PlacementTarget = null;
            DropDownContextMenu.Placement = PlacementMode.MousePoint;
            DropDownContextMenu.IsOpen = true;
        }
    }
}