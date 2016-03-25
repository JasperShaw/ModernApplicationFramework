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

namespace ModernApplicationFramework.Docking.Controls
{
    internal abstract class DropTargetBase : DependencyObject
    {
        public static readonly DependencyProperty IsDraggingOverProperty =
            DependencyProperty.RegisterAttached("IsDraggingOver", typeof (bool), typeof (DropTargetBase),
                new FrameworkPropertyMetadata((bool) false));

        public static bool GetIsDraggingOver(DependencyObject d)
        {
            return (bool) d.GetValue(IsDraggingOverProperty);
        }

        public static void SetIsDraggingOver(DependencyObject d, bool value)
        {
            d.SetValue(IsDraggingOverProperty, value);
        }
    }
}