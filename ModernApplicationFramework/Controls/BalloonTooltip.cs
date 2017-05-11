using System;
using System.ComponentModel;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using ModernApplicationFramework.Controls.Utilities;
using ModernApplicationFramework.Native;
using Point = System.Windows.Point;

namespace ModernApplicationFramework.Controls
{
    public class BalloonTooltip : ContentControl
    {
        private readonly Control _control;
        private Path _pathLeft;
        private Path _pathRight;
        private Path _pathLeftBottom;
        private Path _pathRightBottom;

        public static readonly DependencyProperty CaptionTextProperty = DependencyProperty.Register(
            "CaptionText", typeof(string), typeof(BalloonTooltip), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty TitleTextProperty = DependencyProperty.Register(
            "TitleText", typeof(string), typeof(BalloonTooltip), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty TitleForegroundProperty = DependencyProperty.Register(
            "TitleForeground", typeof(Brush), typeof(BalloonTooltip), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(00, 33, 99))));

        public static readonly DependencyProperty BalloonTypeProperty = DependencyProperty.Register(
            "BalloonType", typeof(BalloonType), typeof(BalloonTooltip), new PropertyMetadata(default(BalloonType)));

        public BalloonType BalloonType
        {
            get => (BalloonType)GetValue(BalloonTypeProperty);
            set => SetValue(BalloonTypeProperty, value);
        }

        public Brush TitleForeground
        {
            get => (Brush)GetValue(TitleForegroundProperty);
            set => SetValue(TitleForegroundProperty, value);
        }

        public string TitleText
        {
            get => (string)GetValue(TitleTextProperty);
            set => SetValue(TitleTextProperty, value);
        }

        public string CaptionText
        {
            get => (string)GetValue(CaptionTextProperty);
            set => SetValue(CaptionTextProperty, value);
        }

        internal Window OwnerWindow { get; }

        static BalloonTooltip()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BalloonTooltip), new FrameworkPropertyMetadata(typeof(BalloonTooltip)));
        }

        public BalloonTooltip(Control control, string caption) : this(control, caption, null, BalloonType.None)
        {
        }

        public BalloonTooltip(Control control, string caption, string title, BalloonType balloonType)
            : this(control, caption, title, balloonType, null)
        {
        }

        public BalloonTooltip(Control control, string caption, string title, BalloonType balloonType, SystemSound sound = null, double maxHeight = 0, double maxWidth = 0)
        {
            _control = control;
            OwnerWindow = Window.GetWindow(control);
            if (OwnerWindow == null)
                throw new ArgumentNullException("There must be at least one Owner-Window");

            SetUpPopup();

            OwnerWindow.Closing += OwnerClosing;
            OwnerWindow.LocationChanged += OwnerLocationChanged;
            control.LostKeyboardFocus += OwnerLocationChanged;
            control.PreviewKeyDown += OwnerLocationChanged;

            if (maxHeight > 0)
                MaxHeight = maxHeight;

            if (maxWidth > 0)
                MaxWidth = maxWidth;

            CaptionText = caption;
            TitleText = title;

            BalloonType = balloonType;
            sound?.Play();
            Loaded += Balloon_Loaded;

        }

        private void SetUpPopup()
        {
            _parentPopup = new TopmostPopup
            {
                AllowsTransparency = true,
                Child = this,
                Placement = PlacementMode.Custom,
                PlacementTarget = _control,
                CustomPopupPlacementCallback = CustomPopupPlacementCallback
            };
        }

        private CustomPopupPlacement[] CustomPopupPlacementCallback(Size popupSize, Size targetSize, Point offset)
        {
            var p = CalcPosition();
            return new[]
            {
                new CustomPopupPlacement(p, PopupPrimaryAxis.Vertical),
            };
        }


        private Popup _parentPopup;

        public void Show()
        {
            _parentPopup.IsOpen = true;
        }

        private void Balloon_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= Balloon_Loaded;
            CalcPosition();
        }

        public override void OnApplyTemplate()
        {
            var pathLeft = GetTemplateChild("PathPointLeft") as Path;
            if (pathLeft != null)
                _pathLeft = pathLeft;
            var pathLeftBottom = GetTemplateChild("PathPointLeftBottom") as Path;
            if (pathLeftBottom != null)
                _pathLeftBottom = pathLeftBottom;
            var pathRight = GetTemplateChild("PathPointRight") as Path;
            if (pathRight != null)
                _pathRight = pathRight;
            var pathRightBottom = GetTemplateChild("PathPointRightBottom") as Path;
            if (pathRightBottom != null)
                _pathRightBottom = pathRightBottom;
            base.OnApplyTemplate();
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            e.Handled = true;
            InternalClose();
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

        private Point CalcPosition()
        {

            var point = new Point();

            if (!IsVisibleToUser())
            {
                InternalClose();
                return point;
            }

            var source = PresentationSource.FromVisual(_control);

            if (source != null)
            {
                double captionPointMargin = _pathLeft.Margin.Left;

                var location = _control.PointToScreen(new Point(0, 0));

                var leftPosition = location.X + _control.ActualWidth / 2;
                var topPosition = location.Y + _control.ActualHeight / 2;


                double caretAddition = 0;
                if (_control is System.Windows.Controls.TextBox textbox)
                {
                    var r = textbox.GetRectFromCharacterIndex(textbox.CaretIndex);
                    caretAddition = r.Left - captionPointMargin;
                }

                Screen.FindMonitorRectsFromPoint(location, out Rect monitor, out Rect work);

                bool flag = false;
                if (topPosition < 0 && work.Height + topPosition + ActualHeight < work.Height ||
                    topPosition >= 0 && topPosition + ActualHeight < work.Height)
                {
                    _pathLeft.Visibility = Visibility.Hidden;
                    _pathLeftBottom.Visibility = Visibility.Collapsed;
                    _pathRight.Visibility = Visibility.Hidden;
                    _pathRightBottom.Visibility = Visibility.Collapsed;
                    point.Y = _control.ActualHeight * 0.75;
                }
                else
                {
                    _pathRight.Visibility = Visibility.Collapsed;
                    _pathRightBottom.Visibility = Visibility.Hidden;
                    _pathLeft.Visibility = Visibility.Collapsed;
                    _pathLeftBottom.Visibility = Visibility.Hidden;
                    point.Y = -(_control.ActualHeight * 2) - _control.ActualHeight * 0.75;
                    flag = true;
                }


                if (leftPosition < 0 && work.Width + leftPosition + ActualWidth < work.Width ||
                    leftPosition >= 0 && leftPosition + ActualWidth < work.Width)
                {
                    if (flag)
                        _pathLeftBottom.Visibility = Visibility.Visible;
                    else
                        _pathLeft.Visibility = Visibility.Visible;
                    point.X = _control.ActualWidth / 2 - captionPointMargin + caretAddition;
                }
                else
                {
                    if (flag)
                        _pathRightBottom.Visibility = Visibility.Visible;
                    else
                        _pathRight.Visibility = Visibility.Visible;
                    point.X = _control.ActualWidth / 2 + captionPointMargin - ActualWidth + caretAddition;
                }
            }
            return point;
        }

        private void OwnerLocationChanged(object sender, EventArgs e)
        {
            InternalClose();
        }

        private static void OwnerClosing(object sender, CancelEventArgs e)
        {
            var name = typeof(BalloonTooltip).Name;

            foreach (Window window in Application.Current.Windows)
            {
                var windowType = window.GetType().Name;
                if (windowType.Equals(name))
                    window.Close();
            }
        }

        private void InternalClose()
        {
            OwnerWindow.Closing -= OwnerClosing;
            OwnerWindow.LocationChanged -= OwnerLocationChanged;
            _control.LostKeyboardFocus -= OwnerLocationChanged;
            _control.PreviewKeyDown -= OwnerLocationChanged;
            _parentPopup.IsOpen = false;
        }
    }
}