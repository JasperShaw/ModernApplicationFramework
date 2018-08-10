using System;
using System.Globalization;

namespace ModernApplicationFramework.Text.Data
{
    public struct TextImageLine : IEquatable<TextImageLine>
    {
        public static readonly TextImageLine Invalid;
        public readonly ITextImage Image;
        public readonly Span Extent;
        public readonly int LineNumber;
        public readonly int LineBreakLength;

        public TextImageLine(ITextImage image, int lineNumber, Span extent, int lineBreakLength)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            if (lineNumber < 0 || lineNumber >= image.LineCount)
                throw new ArgumentOutOfRangeException(nameof(lineNumber));
            if (extent.End > image.Length)
                throw new ArgumentOutOfRangeException(nameof(extent));
            if (lineBreakLength < 0 || lineBreakLength > 2 || extent.End + lineBreakLength > image.Length)
                throw new ArgumentOutOfRangeException(nameof(lineBreakLength));
            Image = image;
            LineNumber = lineNumber;
            Extent = extent;
            LineBreakLength = lineBreakLength;
        }

        public Span ExtentIncludingLineBreak => new Span(Extent.Start, LengthIncludingLineBreak);

        public int Start => Extent.Start;

        public int Length => Extent.Length;

        public int LengthIncludingLineBreak => Extent.Length + LineBreakLength;

        public int End => Extent.End;

        public int EndIncludingLineBreak => Extent.End + LineBreakLength;

        public string GetText()
        {
            return Image.GetText(Extent);
        }

        public string GetTextIncludingLineBreak()
        {
            return Image.GetText(ExtentIncludingLineBreak);
        }

        public string GetLineBreakText()
        {
            return Image.GetText(new Span(Extent.End, LineBreakLength));
        }

        public override int GetHashCode()
        {
            if (Image == null)
                return 0;
            return LineNumber ^ Image.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is TextImageLine line)
                return Equals(line);
            return false;
        }

        public bool Equals(TextImageLine other)
        {
            if (other.Image == Image)
                return other.LineNumber == LineNumber;
            return false;
        }

        public static bool operator ==(TextImageLine left, TextImageLine right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TextImageLine left, TextImageLine right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            if (Image == null)
                return "Invalid";
            var currentCulture = CultureInfo.CurrentCulture;
            var format = "v{0}[{1}, {2}+{3}]";
            var objArray = new object[]
            {
                Image.Version?.VersionNumber,
                null,
                null,
                null
            };
            var index1 = 1;
            var extent = Extent;
            var start = (ValueType)extent.Start;
            objArray[index1] = start;
            var index2 = 2;
            extent = Extent;
            var end = (ValueType)extent.End;
            objArray[index2] = end;
            objArray[3] = LineBreakLength;
            return string.Format(currentCulture, format, objArray);
        }
    }
}