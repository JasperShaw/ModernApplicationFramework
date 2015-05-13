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
using System.Linq;
using System.Windows;
using System.Windows.Media;
using ModernApplicationFramework.Docking.Layout;

namespace ModernApplicationFramework.Docking.Controls
{
	internal class DocumentPaneDropAsAnchorableTarget : DropTarget<LayoutDocumentPaneControl>
	{
		private readonly LayoutDocumentPaneControl _targetPane;
		private int _tabIndex = -1;

		internal DocumentPaneDropAsAnchorableTarget(LayoutDocumentPaneControl paneControl, Rect detectionRect,
			DropTargetType type)
			: base(paneControl, detectionRect, type)
		{
			_targetPane = paneControl;
		}

		internal DocumentPaneDropAsAnchorableTarget(LayoutDocumentPaneControl paneControl, Rect detectionRect,
			DropTargetType type, int tabIndex)
			: base(paneControl, detectionRect, type)
		{
			_targetPane = paneControl;
			_tabIndex = tabIndex;
		}

		public override Geometry GetPreviewPath(OverlayWindow overlayWindow, LayoutFloatingWindow floatingWindowModel)
		{
			ILayoutDocumentPane targetModel = _targetPane.Model as ILayoutDocumentPane;
			if (targetModel != null)
			{
				var manager = targetModel.Root.Manager;

				//ILayoutDocumentPane targetModel = _targetPane.Model as ILayoutDocumentPane;
				LayoutDocumentPaneGroup parentGroup;
				LayoutPanel parentGroupPanel;
				if (!FindParentLayoutDocumentPane(targetModel, out parentGroup, out parentGroupPanel))
					return null;
				var documentPaneControl =
					manager.FindLogicalChildren<FrameworkElement>()
						.OfType<ILayoutControl>()
						.First(d => parentGroup != null ? Equals(d.Model, parentGroup) : Equals(d.Model, parentGroupPanel)) as
						FrameworkElement;
				var targetScreenRect = documentPaneControl.GetScreenArea();

				switch (Type)
				{
					case DropTargetType.DocumentPaneDockAsAnchorableBottom:
					{
						targetScreenRect.Offset(-overlayWindow.Left, -overlayWindow.Top);
						targetScreenRect.Offset(0.0, targetScreenRect.Height - targetScreenRect.Height/3.0);
						targetScreenRect.Height /= 3.0;
						return new RectangleGeometry(targetScreenRect);
					}
					case DropTargetType.DocumentPaneDockAsAnchorableTop:
					{
						targetScreenRect.Offset(-overlayWindow.Left, -overlayWindow.Top);
						targetScreenRect.Height /= 3.0;
						return new RectangleGeometry(targetScreenRect);
					}
					case DropTargetType.DocumentPaneDockAsAnchorableRight:
					{
						targetScreenRect.Offset(-overlayWindow.Left, -overlayWindow.Top);
						targetScreenRect.Offset(targetScreenRect.Width - targetScreenRect.Width/3.0, 0.0);
						targetScreenRect.Width /= 3.0;
						return new RectangleGeometry(targetScreenRect);
					}
					case DropTargetType.DocumentPaneDockAsAnchorableLeft:
					{
						targetScreenRect.Offset(-overlayWindow.Left, -overlayWindow.Top);
						targetScreenRect.Width /= 3.0;
						return new RectangleGeometry(targetScreenRect);
					}
				}
			}

			return null;
		}

		protected override void Drop(LayoutAnchorableFloatingWindow floatingWindow)
		{
			ILayoutDocumentPane targetModel = _targetPane.Model as ILayoutDocumentPane;
			LayoutDocumentPaneGroup parentGroup;
			LayoutPanel parentGroupPanel;
			FindParentLayoutDocumentPane(targetModel, out parentGroup, out parentGroupPanel);

			switch (Type)
			{
				case DropTargetType.DocumentPaneDockAsAnchorableBottom:
				{
					if (parentGroupPanel != null &&
					    parentGroupPanel.ChildrenCount == 1)
						parentGroupPanel.Orientation = System.Windows.Controls.Orientation.Vertical;

					if (parentGroupPanel != null &&
					    parentGroupPanel.Orientation == System.Windows.Controls.Orientation.Vertical)
					{
						parentGroupPanel.Children.Insert(
							parentGroupPanel.IndexOfChild(parentGroup ?? targetModel) + 1,
							floatingWindow.RootPanel);
					}
					else if (parentGroupPanel != null)
					{
						var newParentPanel = new LayoutPanel() {Orientation = System.Windows.Controls.Orientation.Vertical};
						parentGroupPanel.ReplaceChild(parentGroup ?? targetModel, newParentPanel);
						newParentPanel.Children.Add(parentGroup ?? targetModel);
						newParentPanel.Children.Add(floatingWindow.RootPanel);
					}
					else
					{
						throw new NotImplementedException();
					}
				}
					break;
				case DropTargetType.DocumentPaneDockAsAnchorableTop:
				{
					if (parentGroupPanel != null &&
					    parentGroupPanel.ChildrenCount == 1)
						parentGroupPanel.Orientation = System.Windows.Controls.Orientation.Vertical;

					if (parentGroupPanel != null &&
					    parentGroupPanel.Orientation == System.Windows.Controls.Orientation.Vertical)
					{
						parentGroupPanel.Children.Insert(
							parentGroupPanel.IndexOfChild(parentGroup ?? targetModel),
							floatingWindow.RootPanel);
					}
					else if (parentGroupPanel != null)
					{
						var newParentPanel = new LayoutPanel() {Orientation = System.Windows.Controls.Orientation.Vertical};
						parentGroupPanel.ReplaceChild(parentGroup ?? targetModel, newParentPanel);
						newParentPanel.Children.Add(parentGroup ?? targetModel);
						newParentPanel.Children.Insert(0, floatingWindow.RootPanel);
					}
					else
					{
						throw new NotImplementedException();
					}
				}
					break;
				case DropTargetType.DocumentPaneDockAsAnchorableLeft:
				{
					if (parentGroupPanel != null &&
					    parentGroupPanel.ChildrenCount == 1)
						parentGroupPanel.Orientation = System.Windows.Controls.Orientation.Horizontal;

					if (parentGroupPanel != null &&
					    parentGroupPanel.Orientation == System.Windows.Controls.Orientation.Horizontal)
					{
						parentGroupPanel.Children.Insert(
							parentGroupPanel.IndexOfChild(parentGroup ?? targetModel),
							floatingWindow.RootPanel);
					}
					else if (parentGroupPanel != null)
					{
						var newParentPanel = new LayoutPanel() {Orientation = System.Windows.Controls.Orientation.Horizontal};
						parentGroupPanel.ReplaceChild(parentGroup ?? targetModel, newParentPanel);
						newParentPanel.Children.Add(parentGroup ?? targetModel);
						newParentPanel.Children.Insert(0, floatingWindow.RootPanel);
					}
					else
					{
						throw new NotImplementedException();
					}
				}
					break;
				case DropTargetType.DocumentPaneDockAsAnchorableRight:
				{
					if (parentGroupPanel != null &&
					    parentGroupPanel.ChildrenCount == 1)
						parentGroupPanel.Orientation = System.Windows.Controls.Orientation.Horizontal;

					if (parentGroupPanel != null &&
					    parentGroupPanel.Orientation == System.Windows.Controls.Orientation.Horizontal)
					{
						parentGroupPanel.Children.Insert(
							parentGroupPanel.IndexOfChild(parentGroup ?? targetModel) + 1,
							floatingWindow.RootPanel);
					}
					else if (parentGroupPanel != null)
					{
						var newParentPanel = new LayoutPanel() {Orientation = System.Windows.Controls.Orientation.Horizontal};
						parentGroupPanel.ReplaceChild(parentGroup ?? targetModel, newParentPanel);
						newParentPanel.Children.Add(parentGroup ?? targetModel);
						newParentPanel.Children.Add(floatingWindow.RootPanel);
					}
					else
					{
						throw new NotImplementedException();
					}
				}
					break;
			}

			base.Drop(floatingWindow);
		}

		private bool FindParentLayoutDocumentPane(ILayoutDocumentPane documentPane,
			out LayoutDocumentPaneGroup containerPaneGroup, out LayoutPanel containerPanel)
		{
			containerPaneGroup = null;
			containerPanel = null;

			var panel = documentPane.Parent as LayoutPanel;
			if (panel != null)
			{
				containerPanel = panel;
				return true;
			}
			if (!(documentPane.Parent is LayoutDocumentPaneGroup))
				return false;
			var currentDocumentPaneGroup = (LayoutDocumentPaneGroup) documentPane.Parent;
			while (!(currentDocumentPaneGroup.Parent is LayoutPanel))
			{
				currentDocumentPaneGroup = currentDocumentPaneGroup.Parent as LayoutDocumentPaneGroup;

				if (currentDocumentPaneGroup == null)
					break;
			}

			if (currentDocumentPaneGroup == null)
				return false;

			containerPaneGroup = currentDocumentPaneGroup;
			containerPanel = (LayoutPanel) currentDocumentPaneGroup.Parent;
			return true;
		}
	}
}