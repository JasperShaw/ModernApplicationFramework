using System;
using System.Collections.ObjectModel;
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
        public const double TargetLineHeight = 4.0;
        public const double MinLineHeight = 3.0;
        public const double MaxLineHeight = 5.0;
        public const double MinImageHeight = 2.0;
        public const double MaxImageHeight = 4.0;

        public Span LineSpan { get; private set; }

        public bool IsFormatted { get; private set; }

        public bool IsDirty { get; set; }

        public void SetLineSpan(int start, int end)
        {
            this.LineSpan = Span.FromBounds(start, end);
            this.IsDirty = true;
        }

        public int DoFormat(SourceImageMarginElement parent, double xScale)
        {
            ITextSnapshotLine lineFromLineNumber1 = parent.CurrentSnapshot.GetLineFromLineNumber(this.LineSpan.Start);
            ITextSnapshotLine textSnapshotLine = this.LineSpan.Length == 0 ? lineFromLineNumber1 : parent.CurrentSnapshot.GetLineFromLineNumber(this.LineSpan.End);
            ITextSnapshotLine visualLine = lineFromLineNumber1;
            for (int lineNumber = this.LineSpan.Start + 1; lineNumber <= this.LineSpan.End; ++lineNumber)
            {
                ITextSnapshotLine lineFromLineNumber2 = parent.CurrentSnapshot.GetLineFromLineNumber(lineNumber);
                if (lineFromLineNumber2.Length > visualLine.Length)
                    visualLine = lineFromLineNumber2;
            }
            if (visualLine.Length == 0)
            {
                this.Children.Clear();
            }
            else
            {
                Collection<IFormattedLine> collection = parent.Source.FormatLineInVisualBuffer(visualLine);
                if (parent.Source == null)
                {
                    foreach (IDisposable disposable in collection)
                        disposable.Dispose();
                    return 2000;
                }
                double ytop = parent.GetYTop(lineFromLineNumber1.Start);
                double num = parent.GetYBottom(textSnapshotLine.End) - ytop;
                double imageHeight = Math.Min(Math.Max(2.0, Math.Round(num / (double)collection.Count)), 4.0);
                int index1 = Math.Min(Math.Max(1, (int)Math.Round(num / imageHeight)), collection.Count);
                for (int index2 = 0; index2 < index1; ++index2)
                {
                    LineTile.LineImage lineImage = index2 < this.Children.Count ? (LineTile.LineImage)this.Children[index2] : new LineTile.LineImage();
                    int index3 = index1 == 1 ? 0 : index2 * (collection.Count - 1) / (index1 - 1);
                    if (!lineImage.FormatTextViewLine(parent, xScale, imageHeight, collection[index3]))
                    {
                        index1 = index2;
                        break;
                    }
                    if (index2 >= this.Children.Count)
                        this.Children.Add((UIElement)lineImage);
                }
                if (index1 < this.Children.Count)
                    this.Children.RemoveRange(index1, this.Children.Count - index1);
                foreach (IDisposable disposable in collection)
                    disposable.Dispose();
                this.SetPosition(parent);
            }
            this.IsFormatted = true;
            this.IsDirty = false;
            return 100 + visualLine.Length;
        }

        public bool SetSnapshot(ITextSnapshot oldSnapshot, ITextSnapshot newSnapshot, ref int startingLine)
        {
            SnapshotPoint end = oldSnapshot.GetLineFromLineNumber(this.LineSpan.End).End;
            end = end.TranslateTo(newSnapshot, PointTrackingMode.Positive);
            int lineNumber = end.GetContainingLine().LineNumber;
            if (lineNumber < startingLine)
                return false;
            foreach (LineTile.LineImage child in this.Children)
                child.SetSnapshot(newSnapshot);
            this.LineSpan = Span.FromBounds(startingLine, lineNumber);
            startingLine = lineNumber + 1;
            return true;
        }

        public void SetPosition(SourceImageMarginElement parent)
        {
            foreach (LineTile.LineImage child in this.Children)
                child.SetPosition(parent);
        }

        private class LineImage : Image
        {
            public LineImage()
            {
                this.Stretch = Stretch.None;
            }

            public SnapshotSpan Span { get; private set; }

            public void SetSnapshot(ITextSnapshot snapshot)
            {
                this.Span = this.Span.TranslateTo(snapshot, SpanTrackingMode.EdgePositive);
            }

            public void SetPosition(SourceImageMarginElement parent)
            {
                Canvas.SetTop((UIElement)this, Math.Round((parent.GetYTop(this.Span.Start) + parent.GetYBottom(this.Span.End)) * 0.5));
            }

            public bool FormatTextViewLine(SourceImageMarginElement parent, double xScale, double imageHeight, IFormattedLine line)
            {
                if (parent.Bitmap == null || parent.Bitmap.Height != imageHeight || parent.Bitmap.Width != parent.Width)
                {
                    if (parent.Bitmap != null)
                    {
                        parent.Bitmap.Clear();
                        parent.Bitmap.Freeze();
                        parent.Bitmap = (RenderTargetBitmap)null;
                    }
                    try
                    {
                        parent.Bitmap = new RenderTargetBitmap((int)parent.Width, (int)imageHeight, 96.0, 96.0, PixelFormats.Pbgra32);
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
                else
                    parent.Bitmap.Clear();
                this.Span = line.Extent;
                DrawingVisual drawingVisual = new DrawingVisual();
                DrawingContext drawingContext = drawingVisual.RenderOpen();
                double textLeft = line.TextLeft;
                foreach (TextLine textLine in line.TextLines)
                {
                    textLine.Draw(drawingContext, new Point(textLeft, 0.0), InvertAxes.None);
                    textLeft += textLine.WidthIncludingTrailingWhitespace;
                    if (textLeft >= parent.Width)
                        break;
                }
                drawingContext.Close();
                drawingVisual.Transform = (Transform)new ScaleTransform(xScale, imageHeight / line.TextHeight);
                drawingVisual.Transform.Freeze();
                parent.Bitmap.Render((Visual)drawingVisual);
                BitmapImage bitmapImage = new BitmapImage();
                PngBitmapEncoder pngBitmapEncoder = new PngBitmapEncoder();
                pngBitmapEncoder.Frames.Add(BitmapFrame.Create((BitmapSource)parent.Bitmap));
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    pngBitmapEncoder.Save((Stream)memoryStream);
                    memoryStream.Seek(0L, SeekOrigin.Begin);
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = (Stream)memoryStream;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();
                }
                this.Source = (ImageSource)bitmapImage;
                this.RenderTransform = (Transform)new TranslateTransform(0.0, imageHeight * -0.5);
                this.RenderTransform.Freeze();
                return true;
            }
        }
    }
}
