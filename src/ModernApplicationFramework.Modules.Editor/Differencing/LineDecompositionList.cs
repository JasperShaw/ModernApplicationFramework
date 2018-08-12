using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Modules.Editor.Differencing
{
    internal sealed class LineDecompositionList : TokenizedStringList
    {
        public LineDecompositionList(string original, bool ignoreTrimWhiteSpace)
            : base(original)
        {
            if (original.Length == 0)
            {
                Tokens.Add(new Span(0, 0));
            }
            else
            {
                var start1 = -1;
                var num1 = -1;
                var start2 = 0;
                var index = 0;
                while (index < original.Length)
                {
                    var num2 = TextUtilities.LengthOfLineBreak(original, index, original.Length);
                    if (num2 > 0)
                    {
                        index += num2;
                        if (ignoreTrimWhiteSpace)
                            Tokens.Add(start1 == -1 ? new Span(index - num2, 0) : Span.FromBounds(start1, num1 + 1));
                        else
                            Tokens.Add(Span.FromBounds(start2, index));
                        start2 = index;
                        start1 = -1;
                        num1 = -1;
                    }
                    else
                    {
                        if (!char.IsWhiteSpace(original[index]))
                        {
                            if (start1 == -1)
                                start1 = index;
                            num1 = index;
                        }
                        ++index;
                    }
                }
                if (ignoreTrimWhiteSpace)
                    Tokens.Add(start1 == -1 ? new Span(original.Length, 0) : Span.FromBounds(start1, num1 + 1));
                else
                    Tokens.Add(Span.FromBounds(start2, original.Length));
            }
        }
    }
}