using System;
using System.Threading;
using System.Windows.Threading;
using ModernApplicationFramework.Docking.Controls;

namespace ModernApplicationFramework.Docking
{
    public static class KeyboardStateManager
    {
        private static bool _isControlPressed;
        private static Timer _keyboardStateTimer;
        private static DragUndockHeader _currentDragUndockHeader;

        public static DragUndockHeader CurrentDragUndockHeader
        {
            get => _currentDragUndockHeader;
            set
            {
                _currentDragUndockHeader = value;
                if (_currentDragUndockHeader != null)
                {
                    IsMonitoringKeyboard = true;
                }
                else
                {
                    IsMonitoringKeyboard = false;
                    _isControlPressed = false;
                }
            }
        }

        private static bool IsMonitoringKeyboard
        {
            get => _keyboardStateTimer != null;
            set
            {
                if (value)
                {
                    _keyboardStateTimer?.Dispose();
                    _keyboardStateTimer = new Timer(OnKeyboardStateTimer, null, 0, 200);
                }
                else
                {
                    if (_keyboardStateTimer == null)
                        return;
                    _keyboardStateTimer.Dispose();
                    _keyboardStateTimer = null;
                }
            }
        }

        private static void OnKeyboardStateTimer(object state)
        {
            if (NativeMethods.NativeMethods.IsKeyPressed(17))
            {
                if (!_isControlPressed)
                    CurrentDragUndockHeader?.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action) (() =>
                    {
                        DockingManager.Instance.ClearAdorners();
                    }));
                _isControlPressed = true;
            }
            else
            {
                if (_isControlPressed)
                    CurrentDragUndockHeader?.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action) (() =>
                    {
                        CurrentDragUndockHeader?.RaiseDragAbsolute(NativeMethods.NativeMethods.GetCursorPos());
                    }));
                _isControlPressed = false;
            }
        }
    }
}