using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Formatting;

namespace ModernApplicationFramework.Modules.Editor.AdornmentLibrary.VisibleWhitespace
{
    internal class VisibleWhitespaceAdornment : UIElement
    {
        private static readonly Dictionary<int, FormattedText> IdeographicSpaceGlyphs =
            new Dictionary<int, FormattedText>();

        private static readonly Dictionary<int, FormattedText> SpaceGlyphs = new Dictionary<int, FormattedText>();
        private static readonly Typeface SymbolTypeface = new Typeface("Symbol");
        private static readonly Dictionary<int, FormattedText> TabGlyphs = new Dictionary<int, FormattedText>();
        private static readonly Typeface WingdingsTypeface = new Typeface("Wingdings");
        private readonly Brush _glyphBrush;
        private readonly IList<TextBounds> _ideographicSpaceBounds;
        private readonly IList<TextBounds> _spaceBounds;
        private readonly IList<TextBounds> _tabBounds;
        private readonly ITextView _view;

        internal delegate double GetWidthAvailableForGlyph(TextBounds bounds);

        public VisibleWhitespaceAdornment(ITextView view, IList<TextBounds> spaces, IList<TextBounds> tabs,
            IList<TextBounds> ideographicSpaces, Brush glyphBrush)
        {
            _view = view;
            _spaceBounds = spaces;
            _tabBounds = tabs;
            _ideographicSpaceBounds = ideographicSpaces;
            _glyphBrush = glyphBrush;
        }

        internal static FormattedText GetGlyph(TextBounds bounds, Dictionary<int, FormattedText> cachedGlyphs,
            GetWidthAvailableForGlyph getAvailableWidth, string glyphCharacter, Typeface glyphTypeface,
            double glyphSizeToWidthRatio, Brush glyphBrush)
        {
            var num = getAvailableWidth(bounds);
            if (num < 1.0)
                return null;
            var key = (int) num;
            if (!cachedGlyphs.TryGetValue(key, out var formattedText))
            {
                var emSize = Math.Ceiling(key * glyphSizeToWidthRatio);
                var formattedText1 = new FormattedText(glyphCharacter, CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight, glyphTypeface, emSize, glyphBrush);
                formattedText = formattedText1;
                while (formattedText.WidthIncludingTrailingWhitespace > num)
                {
                    --emSize;
                    formattedText.SetFontSize(emSize);
                }

                cachedGlyphs.Add(key, formattedText);
            }

            return formattedText;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            RenderGlyphs(drawingContext, _spaceBounds, SpaceGlyphs, b => b.Width * 0.4, "·", SymbolTypeface, true,
                2.175, _glyphBrush);
            RenderGlyphs(drawingContext, _tabBounds, TabGlyphs,
                b => Math.Min(b.Width, _view.FormattedLineSource.ColumnWidth), "à", WingdingsTypeface, false, 1.03,
                _glyphBrush);
            RenderGlyphs(drawingContext, _ideographicSpaceBounds, IdeographicSpaceGlyphs, b => b.Width * 0.4, "¨",
                SymbolTypeface, true, 1.33, _glyphBrush);
        }

        private static void DrawGlyph(DrawingContext drawingContext, TextBounds bounds, FormattedText glyph,
            bool isCentered, Brush glyphBrush)
        {
            var num1 = isCentered ? (bounds.Width - glyph.Width) * 0.5 : 1.0;
            var num2 = bounds.TextHeight * 0.5 - glyph.Height - glyph.OverhangAfter + glyph.Extent * 0.5;
            var origin = new Point(bounds.Left + num1, bounds.TextTop + num2);
            glyph.SetForegroundBrush(glyphBrush);
            drawingContext.DrawText(glyph, origin);
        }

        private static void RenderGlyphs(DrawingContext drawingContext, IList<TextBounds> bounds,
            Dictionary<int, FormattedText> cachedGlyphs, GetWidthAvailableForGlyph getAvailableWidth, string character,
            Typeface typeface, bool isCentered, double glyphSizeToWidthRatio, Brush glyphBrush)
        {
            FormattedText glyph = null;
            var num1 = double.MaxValue;
            for (var index = 0; index < bounds.Count; ++index)
            {
                var num2 = getAvailableWidth(bounds[index]);
                if (index <= 0 || bounds[index].Width != bounds[index - 1].Width || num1 != num2)
                {
                    num1 = num2;
                    glyph = GetGlyph(bounds[index], cachedGlyphs, getAvailableWidth, character, typeface,
                        glyphSizeToWidthRatio, glyphBrush);
                }

                if (glyph != null)
                    DrawGlyph(drawingContext, bounds[index], glyph, isCentered, glyphBrush);
            }
        }
    }
}