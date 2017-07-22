using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModernApplicationFramework.Controls.Windows
{
    /// <inheritdoc />
    /// <summary>
    /// Custom implementation of a system drop shadow
    /// </summary>
    /// <seealso cref="T:System.Windows.Controls.Decorator" />
    public sealed class SystemDropShadowChrome : Decorator
    {
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Color),
            typeof(SystemDropShadowChrome),
            new FrameworkPropertyMetadata(Color.FromArgb(113, 0,  0,  0),
                FrameworkPropertyMetadataOptions.AffectsRender,
                ClearBrushes));

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius",
            typeof(CornerRadius), typeof(SystemDropShadowChrome),
            new FrameworkPropertyMetadata(new CornerRadius(), FrameworkPropertyMetadataOptions.AffectsRender,
                ClearBrushes), IsCornerRadiusValid);

        private static readonly object ResourceAccess = new object();
        private static Brush[] _commonBrushes;
        private static CornerRadius _commonCornerRadius;
        private Brush[] _brushes;

        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        private static bool IsCornerRadiusValid(object value)
        {
            CornerRadius cornerRadius = (CornerRadius)value;
            if (cornerRadius.TopLeft >= 0.0 && cornerRadius.TopRight >= 0.0 && (cornerRadius.BottomLeft >= 0.0 && cornerRadius.BottomRight >= 0.0) && (!double.IsNaN(cornerRadius.TopLeft) && !double.IsNaN(cornerRadius.TopRight) && (!double.IsNaN(cornerRadius.BottomLeft) && !double.IsNaN(cornerRadius.BottomRight))) && (!double.IsInfinity(cornerRadius.TopLeft) && !double.IsInfinity(cornerRadius.TopRight) && !double.IsInfinity(cornerRadius.BottomLeft)))
                return !double.IsInfinity(cornerRadius.BottomRight);
            return false;
        }

        private static void ClearBrushes(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((SystemDropShadowChrome)o)._brushes = null;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var cornerRadius = CornerRadius;

            Point location = new Point(5.0, 5.0);
            Size renderSize = RenderSize;


            double width1 = renderSize.Width;
            renderSize = RenderSize;
            double height1 = renderSize.Height;
            Size size = new Size(width1, height1);

            var rect = new Rect(location, size);

            Color color = Color;
            if (rect.Width <= 0.0 || rect.Height <= 0.0 || color.A <= 0)
                return;

            double width2 = rect.Right - rect.Left - 10.0;
            double height2 = rect.Bottom - rect.Top - 10.0;
            double val2 = Math.Min(width2 * 0.5, height2 * 0.5);
            cornerRadius.TopLeft = Math.Min(cornerRadius.TopLeft, val2);
            cornerRadius.TopRight = Math.Min(cornerRadius.TopRight, val2);
            cornerRadius.BottomLeft = Math.Min(cornerRadius.BottomLeft, val2);
            cornerRadius.BottomRight = Math.Min(cornerRadius.BottomRight, val2);
            Brush[] brushes = GetBrushes(color, cornerRadius);
            double num1 = rect.Top + 5.0;
            double num2 = rect.Left + 5.0;
            double num3 = rect.Right - 5.0;
            double num4 = rect.Bottom - 5.0;
            double[] guidelinesX = {
                num2,
                num2 + cornerRadius.TopLeft,
                num3 - cornerRadius.TopRight,
                num2 + cornerRadius.BottomLeft,
                num3 - cornerRadius.BottomRight,
                num3
            };
            double[] guidelinesY = {
                num1,
                num1 + cornerRadius.TopLeft,
                num1 + cornerRadius.TopRight,
                num4 - cornerRadius.BottomLeft,
                num4 - cornerRadius.BottomRight,
                num4
            };
            drawingContext.PushGuidelineSet(new GuidelineSet(guidelinesX, guidelinesY));
            cornerRadius.TopLeft = cornerRadius.TopLeft + 5.0;
            cornerRadius.TopRight = cornerRadius.TopRight + 5.0;
            cornerRadius.BottomLeft = cornerRadius.BottomLeft + 5.0;
            cornerRadius.BottomRight = cornerRadius.BottomRight + 5.0;
            Rect rectangle1 = new Rect(rect.Left, rect.Top, cornerRadius.TopLeft, cornerRadius.TopLeft);
            drawingContext.DrawRectangle(brushes[0], null, rectangle1);
            double width3 = guidelinesX[2] - guidelinesX[1];
            if (width3 > 0.0)
            {
                Rect rectangle2 = new Rect(guidelinesX[1], rect.Top, width3, 5.0);
                drawingContext.DrawRectangle(brushes[1], null, rectangle2);
            }
            Rect rectangle3 = new Rect(guidelinesX[2], rect.Top, cornerRadius.TopRight, cornerRadius.TopRight);
            drawingContext.DrawRectangle(brushes[2], null, rectangle3);
            double height3 = guidelinesY[3] - guidelinesY[1];
            if (height3 > 0.0)
            {
                Rect rectangle2 = new Rect(rect.Left, guidelinesY[1], 5.0, height3);
                drawingContext.DrawRectangle(brushes[3], null, rectangle2);
            }
            double height4 = guidelinesY[4] - guidelinesY[2];
            if (height4 > 0.0)
            {
                Rect rectangle2 = new Rect(guidelinesX[5], guidelinesY[2], 5.0, height4);
                drawingContext.DrawRectangle(brushes[5], null, rectangle2);
            }
            Rect rectangle4 = new Rect(rect.Left, guidelinesY[3], cornerRadius.BottomLeft, cornerRadius.BottomLeft);
            drawingContext.DrawRectangle(brushes[6], null, rectangle4);
            double width4 = guidelinesX[4] - guidelinesX[3];
            if (width4 > 0.0)
            {
                Rect rectangle2 = new Rect(guidelinesX[3], guidelinesY[5], width4, 5.0);
                drawingContext.DrawRectangle(brushes[7], null, rectangle2);
            }
            Rect rectangle5 = new Rect(guidelinesX[4], guidelinesY[4], cornerRadius.BottomRight, cornerRadius.BottomRight);
            drawingContext.DrawRectangle(brushes[8], null, rectangle5);
            if (cornerRadius.TopLeft == 5.0 && cornerRadius.TopLeft == cornerRadius.TopRight && (cornerRadius.TopLeft == cornerRadius.BottomLeft && cornerRadius.TopLeft == cornerRadius.BottomRight))
            {
                Rect rectangle2 = new Rect(guidelinesX[0], guidelinesY[0], width2, height2);
                drawingContext.DrawRectangle(brushes[4], null, rectangle2);
            }
            else
            {
                PathFigure pathFigure = new PathFigure();
                if (cornerRadius.TopLeft > 5.0)
                {
                    pathFigure.StartPoint = new Point(guidelinesX[1], guidelinesY[0]);
                    pathFigure.Segments.Add(new LineSegment(new Point(guidelinesX[1], guidelinesY[1]), true));
                    pathFigure.Segments.Add(new LineSegment(new Point(guidelinesX[0], guidelinesY[1]), true));
                }
                else
                    pathFigure.StartPoint = new Point(guidelinesX[0], guidelinesY[0]);
                if (cornerRadius.BottomLeft > 5.0)
                {
                    pathFigure.Segments.Add(new LineSegment(new Point(guidelinesX[0], guidelinesY[3]), true));
                    pathFigure.Segments.Add(new LineSegment(new Point(guidelinesX[3], guidelinesY[3]), true));
                    pathFigure.Segments.Add(new LineSegment(new Point(guidelinesX[3], guidelinesY[5]), true));
                }
                else
                    pathFigure.Segments.Add(new LineSegment(new Point(guidelinesX[0], guidelinesY[5]), true));
                if (cornerRadius.BottomRight > 5.0)
                {
                    pathFigure.Segments.Add(new LineSegment(new Point(guidelinesX[4], guidelinesY[5]), true));
                    pathFigure.Segments.Add(new LineSegment(new Point(guidelinesX[4], guidelinesY[4]), true));
                    pathFigure.Segments.Add(new LineSegment(new Point(guidelinesX[5], guidelinesY[4]), true));
                }
                else
                    pathFigure.Segments.Add(new LineSegment(new Point(guidelinesX[5], guidelinesY[5]), true));
                if (cornerRadius.TopRight > 5.0)
                {
                    pathFigure.Segments.Add(new LineSegment(new Point(guidelinesX[5], guidelinesY[2]), true));
                    pathFigure.Segments.Add(new LineSegment(new Point(guidelinesX[2], guidelinesY[2]), true));
                    pathFigure.Segments.Add(new LineSegment(new Point(guidelinesX[2], guidelinesY[0]), true));
                }
                else
                    pathFigure.Segments.Add(new LineSegment(new Point(guidelinesX[5], guidelinesY[0]), true));
                pathFigure.IsClosed = true;
                pathFigure.Freeze();
                PathGeometry pathGeometry = new PathGeometry();
                pathGeometry.Figures.Add(pathFigure);
                pathGeometry.Freeze();
                drawingContext.DrawGeometry(brushes[4], null, pathGeometry);
            }
            drawingContext.Pop();
        }

        private static GradientStopCollection CreateStops(Color c, double cornerRadius)
        {
            double num = 1.0 / (cornerRadius + 5.0);
            GradientStopCollection gradientStopCollection =
                new GradientStopCollection {new GradientStop(c, (0.5 + cornerRadius) * num)};
            Color color = c;
            color.A = (byte)(0.74336 * c.A);
            gradientStopCollection.Add(new GradientStop(color, (1.5 + cornerRadius) * num));
            color.A = (byte)(0.38053 * c.A);
            gradientStopCollection.Add(new GradientStop(color, (2.5 + cornerRadius) * num));
            color.A = (byte)(0.12389 * c.A);
            gradientStopCollection.Add(new GradientStop(color, (3.5 + cornerRadius) * num));
            color.A = (byte)(0.02654 * c.A);
            gradientStopCollection.Add(new GradientStop(color, (4.5 + cornerRadius) * num));
            color.A = 0;
            gradientStopCollection.Add(new GradientStop(color, (5.0 + cornerRadius) * num));
            gradientStopCollection.Freeze();
            return gradientStopCollection;
        }

        private static Brush[] CreateBrushes(Color c, CornerRadius cornerRadius)
        {
            Brush[] brushArray = new Brush[9];
            brushArray[4] = new SolidColorBrush(c);
            brushArray[4].Freeze();
            GradientStopCollection stops = CreateStops(c, 0.0);
            LinearGradientBrush linearGradientBrush1 = new LinearGradientBrush(stops, new Point(0.0, 1.0), new Point(0.0, 0.0));
            linearGradientBrush1.Freeze();
            brushArray[1] = linearGradientBrush1;
            LinearGradientBrush linearGradientBrush2 = new LinearGradientBrush(stops, new Point(1.0, 0.0), new Point(0.0, 0.0));
            linearGradientBrush2.Freeze();
            brushArray[3] = linearGradientBrush2;
            LinearGradientBrush linearGradientBrush3 = new LinearGradientBrush(stops, new Point(0.0, 0.0), new Point(1.0, 0.0));
            linearGradientBrush3.Freeze();
            brushArray[5] = linearGradientBrush3;
            LinearGradientBrush linearGradientBrush4 = new LinearGradientBrush(stops, new Point(0.0, 0.0), new Point(0.0, 1.0));
            linearGradientBrush4.Freeze();
            brushArray[7] = linearGradientBrush4;
            GradientStopCollection gradientStopCollection1 = cornerRadius.TopLeft != 0.0 ? CreateStops(c, cornerRadius.TopLeft) : stops;
            RadialGradientBrush radialGradientBrush1 = new RadialGradientBrush(gradientStopCollection1)
            {
                RadiusX = 1.0,
                RadiusY = 1.0,
                Center = new Point(1.0, 1.0),
                GradientOrigin = new Point(1.0, 1.0)
            };
            radialGradientBrush1.Freeze();
            brushArray[0] = radialGradientBrush1;
            GradientStopCollection gradientStopCollection2 = cornerRadius.TopRight != 0.0 ? (cornerRadius.TopRight != cornerRadius.TopLeft ? CreateStops(c, cornerRadius.TopRight) : gradientStopCollection1) : stops;
            RadialGradientBrush radialGradientBrush2 = new RadialGradientBrush(gradientStopCollection2)
            {
                RadiusX = 1.0,
                RadiusY = 1.0,
                Center = new Point(0.0, 1.0),
                GradientOrigin = new Point(0.0, 1.0)
            };
            radialGradientBrush2.Freeze();
            brushArray[2] = radialGradientBrush2;
            GradientStopCollection gradientStopCollection3 = cornerRadius.BottomLeft != 0.0 ? (cornerRadius.BottomLeft != cornerRadius.TopLeft ? (cornerRadius.BottomLeft != cornerRadius.TopRight ? CreateStops(c, cornerRadius.BottomLeft) : gradientStopCollection2) : gradientStopCollection1) : stops;
            RadialGradientBrush radialGradientBrush3 = new RadialGradientBrush(gradientStopCollection3)
            {
                RadiusX = 1.0,
                RadiusY = 1.0,
                Center = new Point(1.0, 0.0),
                GradientOrigin = new Point(1.0, 0.0)
            };
            radialGradientBrush3.Freeze();
            brushArray[6] = radialGradientBrush3;
            RadialGradientBrush radialGradientBrush4 = new RadialGradientBrush(cornerRadius.BottomRight != 0.0
                ? (cornerRadius.BottomRight != cornerRadius.TopLeft
                    ? (cornerRadius.BottomRight != cornerRadius.TopRight
                        ? (cornerRadius.BottomRight != cornerRadius.BottomLeft
                            ? CreateStops(c, cornerRadius.BottomRight)
                            : gradientStopCollection3)
                        : gradientStopCollection2)
                    : gradientStopCollection1)
                : stops)
            {
                RadiusX = 1.0,
                RadiusY = 1.0,
                Center = new Point(0.0, 0.0),
                GradientOrigin = new Point(0.0, 0.0)
            };
            radialGradientBrush4.Freeze();
            brushArray[8] = radialGradientBrush4;
            return brushArray;
        }

        private Brush[] GetBrushes(Color c, CornerRadius cornerRadius)
        {
            if (_commonBrushes == null)
            {
                lock (ResourceAccess)
                {
                    if (_commonBrushes == null)
                    {
                        _commonBrushes = CreateBrushes(c, cornerRadius);
                        _commonCornerRadius = cornerRadius;
                    }
                }
            }
            if (c == ((SolidColorBrush)_commonBrushes[4]).Color && cornerRadius == _commonCornerRadius)
            {
                _brushes = null;
                return _commonBrushes;
            }
            return _brushes ?? (_brushes = CreateBrushes(c, cornerRadius));
        }

    }
}
