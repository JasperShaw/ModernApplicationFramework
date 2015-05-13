using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

namespace ModernApplicationFramework.Controls
{
    [TemplatePart(Name = BorderName, Type = typeof(Border))]
    internal abstract class ChromeWindowBorders : Window, IDisposable
    {
        protected bool _isActive;
        protected bool _isVisible;
        protected readonly ModernChromeWindow _targetWindow;
        protected const string BorderName = "PART_Border";

        protected IntPtr TargetWindowHandle
        {
            get { return new WindowInteropHelper(_targetWindow).Handle; }
        }

        public Brush ActiveBorderBrush { get; set; }
        public Brush InactiveBorderBrush { get; set; }

        public new bool IsActive
        {
            get { return _isActive;}
            set
            {
                _isActive = value;
                CommitChange();
            }
        }

        public new bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                CommitChange();
            }
        }

        protected ChromeWindowBorders(ModernChromeWindow owner)
        {
            if (owner == null)
                throw new ArgumentNullException("owner");

            ResizeMode = ResizeMode.NoResize;
            WindowStyle = WindowStyle.None;
            AllowsTransparency = true;

            _targetWindow = owner;

            ActiveBorderBrush = _targetWindow.ActiveBorderColor ?? Brushes.Black;
            InactiveBorderBrush = _targetWindow.InactiveBorderColor ?? Brushes.DarkGray;

            Foreground = IsActive ? ActiveBorderBrush : InactiveBorderBrush;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwndSource = HwndSource.FromHwnd((new WindowInteropHelper(this).Handle));
            if (hwndSource != null)
                hwndSource.AddHook(WndProc);
        }

        protected virtual IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
        {
            return IntPtr.Zero;
        }

        public virtual void CommitChange()
        {
            try
            {
                if (_targetWindow.WindowState != WindowState.Normal)
                    return;
                Owner = _targetWindow;

                if (!_isVisible)
                    Hide();
                else
                {
                    Foreground = _isActive ? ActiveBorderBrush : InactiveBorderBrush;
                    Show();
                }
            }
            catch
            {
            }
        }

        public virtual void Dispose()
        {
            Close();
        }

        public abstract void UpdateWindowPos();
    }
}
