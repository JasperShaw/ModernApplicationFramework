using System.Windows.Media.TextFormatting;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Modules.Editor.Formatting
{
    internal class ClassifiedRun
    {
        public Span Span { get; private set; }

        public string Text { get; }

        public TextRunProperties Properties { get; private set; }

        public int Offset { get; }

        public bool ContainsBidi { get; }

        public bool MayContainMultiCharacterGlyphs { get; private set; }

        public bool ContainsNonWhiteSpace { get; private set; }

        public ClassifiedRun(Span span, TextRunProperties properties, string rawText, int offset)
        {
            Span = span;
            Text = rawText;
            Offset = offset;
            Properties = properties;
            for (int index = 0; index < span.Length; ++index)
            {
                int num = rawText[offset + index];
                if (num > 32 || (1 << num & 9729) == 0)
                {
                    ContainsNonWhiteSpace = true;
                    break;
                }
            }
            for (int index = 0; index < span.Length; ++index)
            {
                char ch = rawText[offset + index];
                if (ch >= '̀')
                {
                    MayContainMultiCharacterGlyphs = true;
                    if (ch >= '\x0590' && (ch <= 'ࣿ' || ch >= '\x200F' && ch <= '\x202E' || ch >= 'יִ'))
                    {
                        ContainsBidi = true;
                        break;
                    }
                }
            }
        }

        internal bool Merge(ClassifiedRun other)
        {
            if (ContainsBidi == other.ContainsBidi && Text == other.Text)
            {
                if (Properties == other.Properties)
                {
                    Span span = Span;
                    int start = span.Start;
                    span = other.Span;
                    int end = span.End;
                    Span = Span.FromBounds(start, end);
                    MayContainMultiCharacterGlyphs = MayContainMultiCharacterGlyphs || other.MayContainMultiCharacterGlyphs;
                    ContainsNonWhiteSpace = ContainsNonWhiteSpace || other.ContainsNonWhiteSpace;
                    return true;
                }
                if ((!ContainsNonWhiteSpace || !other.ContainsNonWhiteSpace) && (Properties.Typeface == other.Properties.Typeface && Properties.FontRenderingEmSize == other.Properties.FontRenderingEmSize) && (Properties.BackgroundBrush == other.Properties.BackgroundBrush && Properties.TextEffects == other.Properties.TextEffects && Properties.TextDecorations == other.Properties.TextDecorations))
                {
                    if (other.ContainsNonWhiteSpace)
                        Properties = other.Properties;
                    Span span = Span;
                    int start = span.Start;
                    span = other.Span;
                    int end = span.End;
                    Span = Span.FromBounds(start, end);
                    MayContainMultiCharacterGlyphs = MayContainMultiCharacterGlyphs || other.MayContainMultiCharacterGlyphs;
                    ContainsNonWhiteSpace = ContainsNonWhiteSpace || other.ContainsNonWhiteSpace;
                    return true;
                }
            }
            return false;
        }
    }
}