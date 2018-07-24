using System.Collections.Generic;

namespace ModernApplicationFramework.TextEditor.Text
{
    public interface ITokenizedStringList : IList<string>
    {
        string Original { get; }

        Span GetElementInOriginal(int index);

        Span GetSpanInOriginal(Span span);
    }
}