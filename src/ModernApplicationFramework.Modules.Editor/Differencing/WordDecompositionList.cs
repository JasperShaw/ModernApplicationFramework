using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Differencing;

namespace ModernApplicationFramework.Modules.Editor.Differencing
{
    internal sealed class WordDecompositionList : TokenizedStringList
    {
        public WordDecompositionList(string original, StringDifferenceOptions options): base(original)
        {
            CreateTokens(options, false);
        }

        public WordDecompositionList(SnapshotSpan original, StringDifferenceOptions options)
            : base(original)
        {
            CreateTokens(options, options.IgnoreTrimWhiteSpace);
        }

        private void CreateTokens(StringDifferenceOptions options, bool ignoreTrimWhiteSpace)
        {
            int originalLength = OriginalLength;
            int num1 = 0;
            int start = 0;
            TokenType tokenType1 = TokenType.WhiteSpace;
            bool flag1 = ignoreTrimWhiteSpace;
            while (num1 < originalLength)
            {
                int num2 = LengthOfLineBreak(num1, originalLength);
                TokenType tokenType2;
                bool flag2;
                if (num2 != 0)
                {
                    tokenType2 = ignoreTrimWhiteSpace ? TokenType.WhiteSpace : TokenType.LineBreak;
                    flag2 = ignoreTrimWhiteSpace;
                }
                else
                {
                    tokenType2 = GetTokenType(CharacterAt(num1), options.WordSplitBehavior);
                    flag2 = tokenType2 == TokenType.WhiteSpace && flag1;
                    num2 = 1;
                }
                if (tokenType2 != tokenType1 || tokenType2 == TokenType.Symbol)
                {
                    if (start < num1 && !flag1)
                        Tokens.Add(new Span(start, num1 - start));
                    tokenType1 = tokenType2;
                    start = num1;
                }
                flag1 = flag2;
                num1 += num2;
            }
            if (originalLength != 0 && ignoreTrimWhiteSpace && tokenType1 == TokenType.WhiteSpace)
                return;
            Tokens.Add(new Span(start, originalLength - start));
        }

        private static TokenType GetTokenType(char c, WordSplitBehavior splitBehavior)
        {
            if (char.IsWhiteSpace(c))
                return TokenType.WhiteSpace;
            if (splitBehavior == WordSplitBehavior.WhiteSpace)
                return TokenType.Other;
            if (char.IsPunctuation(c) || char.IsSymbol(c))
                return TokenType.Symbol;
            if (splitBehavior == WordSplitBehavior.WhiteSpaceAndPunctuation)
                return TokenType.Other;
            if (char.IsDigit(c) || char.IsNumber(c))
                return TokenType.Digit;
            return char.IsLetter(c) ? TokenType.Letter : TokenType.Other;
        }

        public int LengthOfLineBreak(int start, int end)
        {
            switch (CharacterAt(start))
            {
                case '\n':
                case '\x0085':
                case '\x2028':
                case '\x2029':
                    return 1;
                case '\r':
                    return ++start >= end || CharacterAt(start) != '\n' ? 1 : 2;
                default:
                    return 0;
            }
        }

        private enum TokenType
        {
            LineBreak,
            WhiteSpace,
            Symbol,
            Digit,
            Letter,
            Other,
        }
    }
}