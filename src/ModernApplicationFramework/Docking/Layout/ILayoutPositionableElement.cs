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

namespace ModernApplicationFramework.Docking.Layout
{
    internal interface ILayoutPositionableElement : ILayoutElement, ILayoutElementForFloatingWindow
    {
        bool IsVisible { get; }

        GridLength DockHeight { get; set; }

        double DockMinHeight { get; set; }

        double DockMinWidth { get; set; }

        GridLength DockWidth { get; set; }
    }


    internal interface ILayoutPositionableElementWithActualSize
    {
        double ActualHeight { get; set; }
        double ActualWidth { get; set; }
    }

    internal interface ILayoutElementForFloatingWindow
    {
        double FloatingHeight { get; set; }
        double FloatingLeft { get; set; }
        double FloatingTop { get; set; }
        double FloatingWidth { get; set; }
        bool IsMaximized { get; set; }
    }
}