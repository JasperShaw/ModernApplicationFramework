using System;
using System.ComponentModel;
using System.Media;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using ModernApplicationFramework.Native.NativeMethods;
using ModernApplicationFramework.Native.Platform.Structs;
using Point = System.Windows.Point;

namespace ModernApplicationFramework.Controls
{
    public partial class Balloon
    {
        private readonly Control _control;
        private readonly bool _placeInCenter;

        public static readonly DependencyProperty ShowCloseButtonProperty = DependencyProperty.Register("ShowCloseButton", typeof(bool), typeof(Balloon), new PropertyMetadata(OnShowCloseButtonChanged));

        public Balloon(Control control, string caption, BalloonType balloonType)
            : this(control, caption, balloonType, 0)
        {
        }


        public Balloon(Control control, string title, string caption, BalloonType balloonType)
            : this(control, caption, balloonType, 0, 0, false, true, true, title)
        {
        }

        public Balloon(Control control, string caption, BalloonType balloonType, bool placeInCenter, bool showCloseButton)
            : this(control, caption, balloonType, 0, 0, false, placeInCenter, showCloseButton)
        {
        }


        public Balloon(Control control, string title, string caption, BalloonType balloonType, bool placeInCenter, bool showCloseButton)
            : this(control, caption, balloonType, 0, 0, false, placeInCenter, showCloseButton, title)
        {
        }

        public Balloon(Control control, string caption, BalloonType balloonType, double maxHeight = 0, double maxWidth = 0, bool autoWidth = false, 
            bool placeInCenter = true, bool showCloseButton = true, string title = null)
        {
            InitializeComponent();
            _control = control;
            _placeInCenter = placeInCenter;
            ShowCloseButton = showCloseButton;
            Owner = GetWindow(control);

            if (Owner == null)
                return;


            imageClose.Visibility = showCloseButton ? Visibility.Visible : Visibility.Collapsed;

            Owner.Closing += OwnerClosing;
            Owner.LocationChanged += OwnerLocationChanged;
            control.LayoutUpdated += ControlLayoutChangedChanged;
            control.LostKeyboardFocus += OwnerLocationChanged;
            control.PreviewKeyDown += OwnerLocationChanged;


            LinearGradientBrush brush;
            if (balloonType == BalloonType.Help)
            {
                //this.imageType.Source = Properties.Resources.help.ToBitmapImage();
                brush = FindResource("HelpGradient") as LinearGradientBrush;
            }
            else if (balloonType == BalloonType.Information)
            {
                //this.imageType.Source = Properties.Resources.Information.ToBitmapImage();
                brush = FindResource("InfoGradient") as LinearGradientBrush;
            }
            else
            {
                //this.imageType.Source = Properties.Resources.Warning.ToBitmapImage();
                brush = FindResource("WarningGradient") as LinearGradientBrush;
                SystemSounds.Asterisk.Play();
            }

            borderBalloon.SetValue(BackgroundProperty, brush);

            if (autoWidth)
            {
                SizeToContent = SizeToContent.WidthAndHeight;
            }

            textBlockCaption.Text = caption;

            if (maxHeight > 0)
            {
                scrollViewerCaption.MaxHeight = maxHeight;
            }

            if (maxWidth > 0)
            {
                MaxWidth = maxWidth;
            }

            if (!string.IsNullOrWhiteSpace(title))
            {
                textBlockTitle.Text = title;
            }
            else
            {
                textBlockTitle.Visibility = Visibility.Collapsed;
                lineTitle.Visibility = Visibility.Collapsed;
            }

            CalcPosition();
        }


        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            e.Handled = true;
            Close();
        }

        public bool ShowCloseButton
        {
            get => (bool)GetValue(ShowCloseButtonProperty);

            private set => SetValue(ShowCloseButtonProperty, value);
        }

        public bool IsVisibleToUser()
        {
            if (!_control.IsVisible)
            {
                return false;
            }

            var container = (FrameworkElement)VisualTreeHelper.GetParent(_control);
            Rect bounds = _control.TransformToAncestor(container).TransformBounds(new Rect(0.0, 0.0, _control.RenderSize.Width, _control.RenderSize.Height));
            Rect rect = new Rect(0.0, 0.0, container.ActualWidth, container.ActualHeight);
            return rect.IntersectsWith(bounds);
        }

        private void CalcPosition()
        {
            if (!IsVisibleToUser())
            {
                Close();
                return;
            }

            PresentationSource source = PresentationSource.FromVisual(_control);

            if (source != null)
            {
                // Position balloon relative to the help image and screen placement
                // Compensate for the bubble point
                double captionPointMargin = PathPointLeft.Margin.Left;

                Point location = _control.PointToScreen(new Point(0, 0));

                double leftPosition;

                if (_placeInCenter)
                {
                    leftPosition = location.X + (_control.ActualWidth / 2) - captionPointMargin;
                }
                else
                {
                    leftPosition = System.Windows.Forms.Control.MousePosition.X - captionPointMargin;

                    if (leftPosition < location.X)
                    {
                        leftPosition = location.X;
                    }
                    else if (leftPosition > location.X + _control.ActualWidth)
                    {
                        leftPosition = location.X + _control.ActualWidth - (captionPointMargin * 2);
                    }
                }

                //System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.FromHandle(new WindowInteropHelper(Owner).Handle);

                var monitorinfo = MonitorInfoFromWindow(new WindowInteropHelper(Owner).Handle);

                // Check if the window is on the secondary screen.
                if ((leftPosition < 0 && monitorinfo.RcWork.Width + leftPosition + Width < monitorinfo.RcWork.Width) ||
                    leftPosition >= 0 && leftPosition + Width < monitorinfo.RcWork.Width)
                {
                    PathPointRight.Visibility = Visibility.Hidden;
                    PathPointLeft.Visibility = Visibility.Visible;
                    Left = leftPosition;
                }
                else
                {
                    PathPointLeft.Visibility = Visibility.Hidden;
                    PathPointRight.Visibility = Visibility.Visible;
                    Left = location.X + (_control.ActualWidth / 2) + captionPointMargin - Width;
                }

                Top = location.Y + (_control.ActualHeight / 2);
            }
        }

        private static Monitorinfo MonitorInfoFromWindow(IntPtr hWnd)
        {
            var hMonitor = User32.MonitorFromWindow(hWnd, 2);
            var monitorInfo = new Monitorinfo { CbSize = (uint)Marshal.SizeOf(typeof(Monitorinfo)) };
            User32.GetMonitorInfo(hMonitor, ref monitorInfo);
            return monitorInfo;
        }

        private static void OnShowCloseButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Balloon balloon = (Balloon)d;

            balloon.imageClose.Visibility = balloon.ShowCloseButton ? Visibility.Visible : Visibility.Collapsed;
        }

        private void OwnerLocationChanged(object sender, EventArgs e)
        {
            Close();
        }

        private void ControlLayoutChangedChanged(object sender, EventArgs e)
        {
            CalcPosition();
        }    

        private void OwnerClosing(object sender, CancelEventArgs e)
        {
            string name = typeof(Balloon).Name;

            foreach (Window window in Application.Current.Windows)
            {
                string windowType = window.GetType().Name;
                if (windowType.Equals(name))
                {
                    window.Close();
                }
            }
        }


        private void ImageCloseMouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
    }

    public enum BalloonType
    {
        Help = 0,
        Information = 1,
        Warning = 2
    }
}