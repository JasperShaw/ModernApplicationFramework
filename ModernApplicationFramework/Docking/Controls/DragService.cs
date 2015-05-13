/************************************************************************

   AvalonDock

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the New BSD
   License (BSD) as published at http://avalondock.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up AvalonDock in Extended WPF Toolkit Plus at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like facebook.com/datagrids

  **********************************************************************/

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ModernApplicationFramework.Docking.Layout;

namespace ModernApplicationFramework.Docking.Controls
{
	internal class DragService
	{
		private readonly List<IDropArea> _currentWindowAreas = new List<IDropArea>();
		private readonly LayoutFloatingWindowControl _floatingWindow;
		private readonly DockingManager _manager;
		private readonly List<IOverlayWindowHost> _overlayWindowHosts = new List<IOverlayWindowHost>();
		private IDropTarget _currentDropTarget;
		private IOverlayWindowHost _currentHost;
		private IOverlayWindow _currentWindow;

		public DragService(LayoutFloatingWindowControl floatingWindow)
		{
			_floatingWindow = floatingWindow;
			_manager = floatingWindow.Model.Root.Manager;


			GetOverlayWindowHosts();
		}

		public void Drop(Point dropLocation, out bool dropHandled)
		{
			dropHandled = false;

			UpdateMouseLocation(dropLocation);

			var floatingWindowModel = _floatingWindow.Model as LayoutFloatingWindow;
			if (floatingWindowModel != null)
			{
				var root = floatingWindowModel.Root;

				_currentHost?.HideOverlayWindow();

				if (_currentDropTarget != null)
				{
					_currentWindow.DragDrop(_currentDropTarget);
					root.CollectGarbage();
					dropHandled = true;
				}
			}


			_currentWindowAreas.ForEach(a => _currentWindow.DragLeave(a));

			if (_currentDropTarget != null)
				_currentWindow.DragLeave(_currentDropTarget);
			_currentWindow?.DragLeave(_floatingWindow);
			_currentWindow = null;

			_currentHost = null;
		}

		public void UpdateMouseLocation(Point dragPosition)
		{
			var newHost = _overlayWindowHosts.FirstOrDefault(oh => oh.HitTest(dragPosition));

			if (_currentHost != null || _currentHost != newHost)
			{
				//is mouse still inside current overlay window host?
				if ((_currentHost != null && !_currentHost.HitTest(dragPosition)) ||
				    _currentHost != newHost)
				{
					//esit drop target
					if (_currentDropTarget != null)
						_currentWindow.DragLeave(_currentDropTarget);
					_currentDropTarget = null;

					//exit area
					_currentWindowAreas.ForEach(a =>
						_currentWindow.DragLeave(a));
					_currentWindowAreas.Clear();

					//hide current overlay window
					_currentWindow?.DragLeave(_floatingWindow);
					_currentHost?.HideOverlayWindow();
					_currentHost = null;
				}

				if (_currentHost != newHost)
				{
					_currentHost = newHost;
					if (_currentHost != null) _currentWindow = _currentHost.ShowOverlayWindow(_floatingWindow);
					_currentWindow?.DragEnter(_floatingWindow);
				}
			}

			if (_currentHost == null)
				return;

			if (_currentDropTarget != null &&
			    !_currentDropTarget.HitTest(dragPosition))
			{
				_currentWindow?.DragLeave(_currentDropTarget);
				_currentDropTarget = null;
			}

			List<IDropArea> areasToRemove = new List<IDropArea>();
			_currentWindowAreas.ForEach(a =>
			{
				//is mouse still inside this area?
				if (!a.DetectionRect.Contains(dragPosition))
				{
					_currentWindow.DragLeave(a);
					areasToRemove.Add(a);
				}
			});

			areasToRemove.ForEach(a =>
				_currentWindowAreas.Remove(a));


			var areasToAdd =
				_currentHost.GetDropAreas(_floatingWindow)
					.Where(cw => !_currentWindowAreas.Contains(cw) && cw.DetectionRect.Contains(dragPosition))
					.ToList();

			_currentWindowAreas.AddRange(areasToAdd);

			areasToAdd.ForEach(a =>
				_currentWindow.DragEnter(a));

			if (_currentDropTarget == null)
			{
				_currentWindowAreas.ForEach(wa =>
				{
					if (_currentDropTarget != null)
						return;

					_currentDropTarget = _currentWindow.GetTargets().FirstOrDefault(dt => dt.HitTest(dragPosition));
					if (_currentDropTarget == null)
						return;
					_currentWindow.DragEnter(_currentDropTarget);
				});
			}
		}

		internal void Abort()
		{
			_currentWindowAreas.ForEach(a => _currentWindow.DragLeave(a));

			if (_currentDropTarget != null)
				_currentWindow.DragLeave(_currentDropTarget);
			_currentWindow?.DragLeave(_floatingWindow);
			_currentWindow = null;
			_currentHost?.HideOverlayWindow();
			_currentHost = null;
		}

		private void GetOverlayWindowHosts()
		{
			_overlayWindowHosts.AddRange(
				_manager.GetFloatingWindowsByZOrder()
					.OfType<LayoutAnchorableFloatingWindowControl>()
					.Where(fw => !Equals(fw, _floatingWindow) && fw.IsVisible));
			_overlayWindowHosts.Add(_manager);
		}
	}
}