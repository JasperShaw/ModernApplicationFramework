using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

namespace ModernApplicationFramework.Modules.Editor.Formatting
{
    internal class TextInfoCache
    {
        private readonly IDictionary<TextRunProperties, FontInfo> _textInfoTable = new Dictionary<TextRunProperties, FontInfo>();

        public TextInfoCache(bool useDisplayMode)
        {
            UseDisplayMode = useDisplayMode;
            TextFormatter = TextFormatter.Create(UseDisplayMode ? TextFormattingMode.Display : TextFormattingMode.Ideal);
        }

        public TextFormatter TextFormatter { get; }

        public bool UseDisplayMode { get; }

        public double GetTextBaseline(TextRunProperties properties)
        {
            return GetTextInfo(properties).Baseline;
        }

        public double GetTextHeight(TextRunProperties properties)
        {
            return GetTextInfo(properties).TextHeight;
        }

        public double GetSpaceWidth(TextRunProperties properties)
        {
            return GetTextInfo(properties).SpaceWidth;
        }

        public FontInfo GetTextInfo(TextRunProperties key)
        {
            if (!_textInfoTable.TryGetValue(key, out var fontInfo))
            {
                var formattedText = new FormattedText("Xg ", key.CultureInfo, FlowDirection.LeftToRight,
                    key.Typeface, key.FontRenderingEmSize, Brushes.Black, null,
                    UseDisplayMode ? TextFormattingMode.Display : TextFormattingMode.Ideal);
                fontInfo = new FontInfo(formattedText.Height, formattedText.Baseline, formattedText.WidthIncludingTrailingWhitespace - formattedText.Width);
                _textInfoTable.Add(key, fontInfo);
            }
            return fontInfo;
        }

        internal class FontInfo
        {
            public readonly double TextHeight;
            public readonly double Baseline;
            public readonly double SpaceWidth;

            public FontInfo(double textHeight, double baseline, double spaceWidth)
            {
                TextHeight = textHeight;
                Baseline = baseline;
                SpaceWidth = spaceWidth;
            }
        }
    }
}