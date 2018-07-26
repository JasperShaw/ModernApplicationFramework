using System.Windows.Media.TextFormatting;

namespace ModernApplicationFramework.TextEditor
{
    internal struct TextLineData
    {
        public readonly Span TokenSpan;
        public readonly int TextLineIndex;
        public readonly double Left;
        public readonly TextLine TextLine;

        public double Right => Left + TextLine.WidthIncludingTrailingWhitespace;

        public TextLineData(Span span, int textLineIndex, double left, TextLine textLine)
        {
            TokenSpan = span;
            TextLineIndex = textLineIndex;
            Left = left;
            TextLine = textLine;
        }
    }
}