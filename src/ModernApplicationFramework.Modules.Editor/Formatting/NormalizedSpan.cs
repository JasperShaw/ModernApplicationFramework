using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Formatting;

namespace ModernApplicationFramework.Modules.Editor.Formatting
{
    internal class NormalizedSpan
    {
        public static int MaxTextLineLength = 9500;
        public readonly PositionAffinity Affinity;
        public readonly Span BufferSpan;
        public readonly ClassifiedRun ClassifiedRun;
        public readonly IAdornmentElement Element;
        public readonly TextRun GlyphOrFormattingRun;
        public readonly Span TokenSpan;

        public NormalizedSpan(ClassifiedRun classifiedRun, ref int offset)
        {
            ClassifiedRun = classifiedRun;
            BufferSpan = classifiedRun.Span;
            TokenSpan = new Span(offset, BufferSpan.Length);
            offset = TokenSpan.End;
        }

        public NormalizedSpan(TextRun glyphOrFormattingRun, PositionAffinity affinity, Span bufferSpan, ref int offset)
        {
            GlyphOrFormattingRun = glyphOrFormattingRun;
            Affinity = affinity;
            BufferSpan = bufferSpan;
            TokenSpan = new Span(offset, 1);
            offset = TokenSpan.End;
        }

        public NormalizedSpan(IAdornmentElement element, TextFormattingRunProperties defaultProperties, Span bufferSpan,
            ref int offset)
        {
            GlyphOrFormattingRun = new TextEmbeddedSpace(element, defaultProperties);
            Element = element;
            Affinity = element.Affinity;
            BufferSpan = bufferSpan;
            TokenSpan = new Span(offset, 1);
            offset = TokenSpan.End;
        }

        public TextRun GetTextRun(int tokenIndex, int textLineStartIndex, int forcedLineBreakTokenIndex)
        {
            if (GlyphOrFormattingRun != null)
                return GlyphOrFormattingRun;
            var num = tokenIndex - TokenSpan.Start;
            var length = Math.Min(TokenSpan.Length - num, forcedLineBreakTokenIndex - tokenIndex);
            if (tokenIndex + length - textLineStartIndex > MaxTextLineLength)
                return NormalizedSpanTextSource.LineBreak;
            return new TextCharacters(ClassifiedRun.Text, ClassifiedRun.Offset + num, length, ClassifiedRun.Properties);
        }

        public bool IsTab(int tokenIndex)
        {
            if (ClassifiedRun != null)
                return ClassifiedRun.Text[ClassifiedRun.Offset + tokenIndex - TokenSpan.Start] == '\t';
            return false;
        }

        internal class TextEmbeddedSpace : TextEmbeddedObject
        {
            private readonly IAdornmentElement _element;
            private readonly Size _size;

            public override LineBreakCondition BreakAfter => _element.Affinity != PositionAffinity.Predecessor
                ? LineBreakCondition.BreakRestrained
                : LineBreakCondition.BreakPossible;

            public override LineBreakCondition BreakBefore => _element.Affinity != PositionAffinity.Successor
                ? LineBreakCondition.BreakRestrained
                : LineBreakCondition.BreakPossible;

            public override CharacterBufferReference CharacterBufferReference => new CharacterBufferReference(" ", 1);

            public override bool HasFixedSize => true;

            public override int Length => 1;

            public override TextRunProperties Properties { get; }

            public TextEmbeddedSpace(IAdornmentElement adornmentElement, TextFormattingRunProperties defaultProperties)
            {
                _element = adornmentElement;
                _size = new Size(adornmentElement.Width, adornmentElement.TextHeight);
                Properties = defaultProperties;
            }

            public override Rect ComputeBoundingBox(bool rightToLeft, bool sideways)
            {
                return new Rect(0.0, 0.0, _size.Width, _size.Height);
            }

            public override void Draw(DrawingContext drawingContext, Point origin, bool rightToLeft, bool sideways)
            {
            }

            public override TextEmbeddedObjectMetrics Format(double remainingParagraphWidth)
            {
                return new TextEmbeddedObjectMetrics(_size.Width, _size.Height, _element.Baseline);
            }
        }
    }
}