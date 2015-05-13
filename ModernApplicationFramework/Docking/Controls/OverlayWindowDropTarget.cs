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
	public class OverlayWindowDropTarget : IOverlayWindowDropTarget
	{
		private readonly Rect _screenDetectionArea;
		private readonly OverlayWindowDropTargetType _type;

		internal OverlayWindowDropTarget(IOverlayWindowArea overlayArea, OverlayWindowDropTargetType targetType,
			FrameworkElement element)
		{
			_type = targetType;
			_screenDetectionArea = new Rect(element.TransformToDeviceDpi(new Point()), element.TransformActualSizeToAncestor());
		}

		Rect IOverlayWindowDropTarget.ScreenDetectionArea => _screenDetectionArea;
		OverlayWindowDropTargetType IOverlayWindowDropTarget.Type => _type;
	}
}