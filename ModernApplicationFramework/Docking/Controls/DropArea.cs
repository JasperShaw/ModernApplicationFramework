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
	public enum DropAreaType
	{
		DockingManager,

		DocumentPane,

		DocumentPaneGroup,

		AnchorablePane,
	}


	public interface IDropArea
	{
		Rect DetectionRect { get; }
		DropAreaType Type { get; }
	}

	public class DropArea<T> : IDropArea where T : FrameworkElement
	{
		internal DropArea(T areaElement, DropAreaType type)
		{
			AreaElement = areaElement;
			DetectionRect = areaElement.GetScreenArea();
			Type = type;
		}

		public Rect DetectionRect { get; }
		public DropAreaType Type { get; }
		public T AreaElement { get; }
	}
}