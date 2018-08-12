using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Formatting;

namespace ModernApplicationFramework.Modules.Editor.OverviewMargin
{
    internal class LineTile : Canvas
    {
        public const double MaxImageHeight = 4.0;
        public const double MaxLineHeight = 5.0;
        public const double MinImageHeight = 2.0;
        public const double MinLineHeight = 3.0;
        public const double TargetLineHeight = 4.0;

        public bool IsDirty { get; set; }

        public bool IsFormatted { get; private set; }

        public Span LineSpan { get; private set; }

        public int DoFormat(SourceImageMarginElement parent, double xScale)
        {
            var lineFromLineNumber1 = parent.CurrentSnapshot.GetLineFromLineNumber(LineSpan.Start);
            var textSnapshotLine = LineSpan.Length == 0
                ? lineFromLineNumber1
                : parent.CurrentSnapshot.GetLineFromLineNumber(LineSpan.End);
            var visualLine = lineFromLineNumber1;
            for (var lineNumber = LineSpan.Start + 1; lineNumber <= LineSpan.End; ++lineNumber)
            {
                var lineFromLineNumber2 = parent.CurrentSnapshot.GetLineFromLineNumber(lineNumber);
                if (lineFromLineNumber2.Length > visualLine.Length)
                    visualLine = lineFromLineNumber2;
            }

            if (visualLine.Length == 0)
            {
                Children.Clear();
            }
            else
            {
                var collection = parent.Source.FormatLineInVisualBuffer(visualLine);
                if (parent.Source == null)
                {
                    foreach (IDisposable disposable in collection)
                        disposable.Dispose();
                    return 2000;
                }

                var ytop = parent.GetYTop(lineFromLineNumber1.Start);
                var num = parent.GetYBottom(textSnapshotLine.End) - ytop;
                var imageHeight = Math.Min(Math.Max(2.0, Math.Round(num / collection.Count)), 4.0);
                var index1 = Math.Min(Math.Max(1, (int) Math.Round(num / imageHeight)), collection.Count);
                for (var index2 = 0; index2 < index1; ++index2)
                {
                    var lineImage = index2 < Children.Count ? (LineImage) Children[index2] : new LineImage();
                    var index3 = index1 == 1 ? 0 : index2 * (collection.Count - 1) / (index1 - 1);
                    if (!lineImage.FormatTextViewLine(parent, xScale, imageHeight, collection[index3]))
                    {
                        index1 = index2;
                        break;
                    }

                    if (index2 >= Children.Count)
                        Children.Add(lineImage);
                }

                if (index1 < Children.Count)
                    Children.RemoveRange(index1, Children.Count - index1);
                foreach (IDisposable disposable in collection)
                    disposable.Dispose();
                SetPosition(parent);
            }

            IsFormatted = true;
            IsDirty = false;
            return 100 + visualLine.Length;
        }

        public void SetLineSpan(int start, int end)
        {
            LineSpan = Span.FromBounds(start, end);
            IsDirty = true;
        }

        public void SetPosition(SourceImageMarginElement parent)
        {
            foreach (LineImage child in Children)
                child.SetPosition(parent);
        }

        public bool SetSnapshot(ITextSnapshot oldSnapshot, ITextSnapshot newSnapshot, ref int startingLine)
        {
            var end = oldSnapshot.GetLineFromLineNumber(LineSpan.End).End;
            end = end.TranslateTo(newSnapshot, PointTrackingMode.Positive);
            var lineNumber = end.GetContainingLine().LineNumber;
            if (lineNumber < startingLine)
                return false;
            foreach (LineImage child in Children)
                child.SetSnapshot(newSnapshot);
            LineSpan = Span.FromBounds(startingLine, lineNumber);
            startingLine = lineNumber + 1;
            return true;
        }

        private class LineImage : Image
        {
            public SnapshotSpan Span { get; private set; }

            public LineImage()
            {
                Stretch = Stretch.None;
            }

            public bool FormatTextViewLine(SourceImageMarginElement parent, double xScale, double imageHeight,
                IFormattedLine line)
            {
                if (parent.Bitmap == null || parent.Bitmap.Height != imageHeight || parent.Bitmap.Width != parent.Width)
                {
                    if (parent.Bitmap != null)
                    {
                        parent.Bitmap.Clear();
                        parent.Bitmap.Freeze();
                        parent.Bitmap = null;
                    }

                    try
                    {
                        parent.Bitmap = new RenderTargetBitmap((int) parent.Width, (int) imageHeight, 96.0, 96.0,
                            PixelFormats.Pbgra32);
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
                else
                {
                    parent.Bitmap.Clear();
                }

                Span = line.Extent;
                var drawingVisual = new DrawingVisual();
                var drawingContext = drawingVisual.RenderOpen();
                var textLeft = line.TextLeft;
                foreach (var textLine in line.TextLines)
                {
                    textLine.Draw(drawingContext, new Point(textLeft, 0.0), InvertAxes.None);
                    textLeft += textLine.WidthIncludingTrailingWhitespace;
                    if (textLeft >= parent.Width)
                        break;
                }

                drawingContext.Close();
                drawingVisual.Transform = new ScaleTransform(xScale, imageHeight / line.TextHeight);
                drawingVisual.Transform.Freeze();
                parent.Bitmap.Render(drawingVisual);
                var bitmapImage = new BitmapImage();
                var pngBitmapEncoder = new PngBitmapEncoder();
                pngBitmapEncoder.Frames.Add(BitmapFrame.Create(parent.Bitmap));
                using (var memoryStream = new MemoryStream())
                {
                    pngBitmapEncoder.Save(memoryStream);
                    memoryStream.Seek(0L, SeekOrigin.Begin);
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = memoryStream;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();
                }

                Source = bitmapImage;
                RenderTransform = new TranslateTransform(0.0, imageHeight * -0.5);
                RenderTransform.Freeze();
                return true;
            }

            public void SetPosition(SourceImageMarginElement parent)
            {
                SetTop(this, Math.Round((parent.GetYTop(Span.Start) + parent.GetYBottom(Span.End)) * 0.5));
            }

            public void SetSnapshot(ITextSnapshot snapshot)
            {
                Span = Span.TranslateTo(snapshot, SpanTrackingMode.EdgePositive);
            }
        }
    }
}