﻿using System.Windows.Media.TextFormatting;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Modules.Editor.Formatting
{
    internal struct TextLineData
    {
        public readonly double Left;
        public readonly TextLine TextLine;
        public readonly int TextLineIndex;
        public readonly Span TokenSpan;

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