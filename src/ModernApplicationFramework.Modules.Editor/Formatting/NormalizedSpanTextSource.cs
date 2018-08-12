using System.Collections.Generic;
using System.Globalization;
using System.Windows.Media.TextFormatting;

namespace ModernApplicationFramework.Modules.Editor.Formatting
{
    internal class NormalizedSpanTextSource : TextSource
    {
        public static TextEndOfLine LineBreak = new TextEndOfLine(1);
        private int _predictedNormalizedSpanIndex;

        public int ForcedLineBreakIndex { get; set; }

        public IList<NormalizedSpan> NormalizedSpans { get; }

        public int TextLineStartIndex { get; set; }

        public NormalizedSpanTextSource(IList<NormalizedSpan> normalizedSpans)
        {
            NormalizedSpans = normalizedSpans;
        }

        public NormalizedSpan FindNormalizedSpan(int tokenIndex)
        {
            var normalizedSpan = NormalizedSpans[_predictedNormalizedSpanIndex];
            if (tokenIndex >= normalizedSpan.TokenSpan.End)
            {
                if (_predictedNormalizedSpanIndex < NormalizedSpans.Count - 1)
                    do
                    {
                        normalizedSpan = NormalizedSpans[++_predictedNormalizedSpanIndex];
                    } while (!normalizedSpan.TokenSpan.Contains(tokenIndex));
            }
            else if (tokenIndex < normalizedSpan.TokenSpan.Start)
            {
                do
                {
                    normalizedSpan = NormalizedSpans[--_predictedNormalizedSpanIndex];
                } while (!normalizedSpan.TokenSpan.Contains(tokenIndex));
            }

            return normalizedSpan;
        }

        public override TextSpan<CultureSpecificCharacterBufferRange> GetPrecedingText(
            int textSourceCharacterIndexLimit)
        {
            return new TextSpan<CultureSpecificCharacterBufferRange>(0,
                new CultureSpecificCharacterBufferRange(CultureInfo.CurrentUICulture,
                    new CharacterBufferRange("", 0, 0)));
        }

        public override int GetTextEffectCharacterIndexFromTextSourceCharacterIndex(int textSourceCharacterIndex)
        {
            return textSourceCharacterIndex;
        }

        public override TextRun GetTextRun(int tokenIndex)
        {
            if (tokenIndex >= ForcedLineBreakIndex)
                return LineBreak;
            return FindNormalizedSpan(tokenIndex).GetTextRun(tokenIndex, TextLineStartIndex, ForcedLineBreakIndex);
        }

        public bool IsTab(int tokenIndex)
        {
            return FindNormalizedSpan(tokenIndex).IsTab(tokenIndex);
        }
    }
}