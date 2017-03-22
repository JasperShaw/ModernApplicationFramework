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
using System.Windows.Threading;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Docking.Layout;

namespace ModernApplicationFramework.Docking.Controls
{
    internal class AutoHideWindowManager
    {
        private readonly DockingManager _manager;
        private DispatcherTimer _closeTimer;
        private WeakReference _currentAutohiddenAnchor;

        internal AutoHideWindowManager(DockingManager manager)
        {
            _manager = manager;
            SetupCloseTimer();
        }

        public void HideAutoWindow(LayoutAnchorControl anchor = null)
        {
            if (anchor == null ||
                Equals(anchor, _currentAutohiddenAnchor.GetValueOrDefault<LayoutAnchorControl>()))
                StopCloseTimer();
            else
            {
                ShowAutoHideWindow(anchor);
                if (anchor.Model is LayoutAnchorable model)
                    model.IsActive = true;
            }
        }

        public void ShowAutoHideWindow(LayoutAnchorControl anchor)
        {
            if (Equals(_currentAutohiddenAnchor.GetValueOrDefault<LayoutAnchorControl>(), anchor))
                return;
            StopCloseTimer();
            _currentAutohiddenAnchor = new WeakReference(anchor);
            _manager.AutoHideWindow.Show(anchor);
            StartCloseTimer();
        }

        private void SetupCloseTimer()
        {
            _closeTimer = new DispatcherTimer(DispatcherPriority.Background) {Interval = TimeSpan.FromMilliseconds(0)};
            //Was 1500
            _closeTimer.Tick += (s, e) =>
            {
                if (_manager.AutoHideWindow.IsWin32MouseOver ||
                    ((LayoutAnchorable) _manager.AutoHideWindow.Model).IsActive ||
                    _manager.AutoHideWindow.IsResizing)
                    return;

                StopCloseTimer();
            };
        }

        private void StartCloseTimer()
        {
            _closeTimer.Start();
        }

        private void StopCloseTimer()
        {
            _closeTimer.Stop();
            _manager.AutoHideWindow.Hide();
            _currentAutohiddenAnchor = null;
        }
    }
}