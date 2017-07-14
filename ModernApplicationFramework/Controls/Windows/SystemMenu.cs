using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ModernApplicationFramework.Interfaces.Controls;
using ModernApplicationFramework.Interfaces.ViewModels;
using ModernApplicationFramework.Native;

namespace ModernApplicationFramework.Controls.Windows
{
    public sealed class SystemMenu : Control, INonClientArea
    {
        public static readonly DependencyProperty SourceProperty = Image.SourceProperty.AddOwner(typeof(SystemMenu),
            new FrameworkPropertyMetadata(OnSourceChanged));

        public static readonly DependencyProperty VectorIconProperty = DependencyProperty.Register("VectorIcon",
            typeof(Geometry), typeof(SystemMenu),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty VectorFillProperty = DependencyProperty.Register("VectorFill",
            typeof(Brush), typeof(SystemMenu));

        private ImageSource _optimalImageForSize;

        static SystemMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SystemMenu),
                new FrameworkPropertyMetadata(typeof(SystemMenu)));
        }

        public ImageSource Source
        {
            get => (ImageSource) GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public Brush VectorFill
        {
            get => (Brush) GetValue(VectorFillProperty);
            set => SetValue(VectorFillProperty, value);
        }

        public Geometry VectorIcon
        {
            get => (Geometry) GetValue(VectorIconProperty);
            set => SetValue(VectorIconProperty, value);
        }

        public int HitTest(Point point)
        {
            return 3;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            var w = System.Windows.Window.GetWindow(this) as MainWindow;

            if (w == null)
                return;

            if (e.ClickCount == 1)
            {
                Point p;
                if (e.ChangedButton == MouseButton.Left)
                {
                    p = PointToScreen(e.GetPosition(this));
                    p.X += 1;
                    p.Y += 1;
                }
                else
                {
                    p = PointToScreen(e.GetPosition(this));
                    p.X += 1;
                    p.Y += 1;
                }
                SystemCommands.ShowSystemMenu(w, p);
            }
            if (e.ClickCount == 2 && e.ChangedButton == MouseButton.Left)
                ((IMainWindowViewModel) DataContext).CloseCommand.Execute(null);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (VectorIcon != null)
                return;
            var size = new Size(Math.Max(0.0, RenderSize.Width - Padding.Left - Padding.Right),
                Math.Max(0.0, RenderSize.Height - Padding.Top - Padding.Bottom));
            CoerceOptimalImageForSize(size);
            if (_optimalImageForSize == null)
                return;
            drawingContext.DrawImage(_optimalImageForSize, new Rect(new Point(Padding.Left, Padding.Top), size));
        }

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SystemMenu) d).OnSourceChanged();
        }

        private void CoerceOptimalImageForSize(Size size)
        {
            if (_optimalImageForSize != null)
                return;
            var frame = Source as BitmapFrame;
            var num1 = (int) size.LogicalToDeviceUnits().Width;
            var num2 = -1;
            if (frame != null)
            {
                if (frame.Decoder == null)
                    return;
                foreach (var bitmapFrame in frame.Decoder.Frames)
                {
                    var pixelWidth = bitmapFrame.PixelWidth;
                    if (pixelWidth == num1)
                    {
                        _optimalImageForSize = bitmapFrame;
                        break;
                    }
                    if (pixelWidth > num1)
                    {
                        if (num2 >= num1 && pixelWidth >= num2)
                            continue;
                        num2 = pixelWidth;
                        _optimalImageForSize = bitmapFrame;
                    }
                    else
                        if (pixelWidth > num2)
                        {
                            num2 = pixelWidth;
                            _optimalImageForSize = bitmapFrame;
                        }
                }
            }
            else
                _optimalImageForSize = Source;
        }

        private void OnSourceChanged()
        {
            _optimalImageForSize = null;
            InvalidateVisual();
        }
    }
}