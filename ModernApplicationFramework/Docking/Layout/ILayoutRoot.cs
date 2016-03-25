/************************************************************************

   AvalonDock

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the New BSD
   License (BSD) as published at http://avalondock.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up AvalonDock in Extended WPF Toolkit Plus at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like facebook.com/datagrids

  **********************************************************************/

using System.Collections.ObjectModel;

namespace ModernApplicationFramework.Docking.Layout
{
    public interface ILayoutRoot
    {
        LayoutAnchorSide BottomSide { get; }

        ObservableCollection<LayoutFloatingWindow> FloatingWindows { get; }
        ObservableCollection<LayoutAnchorable> Hidden { get; }
        LayoutAnchorSide LeftSide { get; }
        DockingManager Manager { get; }
        LayoutAnchorSide RightSide { get; }

        LayoutPanel RootPanel { get; }

        LayoutAnchorSide TopSide { get; }

        LayoutContent ActiveContent { get; set; }

        void CollectGarbage();
    }
}