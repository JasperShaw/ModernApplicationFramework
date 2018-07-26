using System.IO;

namespace ModernApplicationFramework.TextEditor.Text
{
    public interface ITextImage
    {
        ITextImageVersion Version { get; }

        ITextImage GetSubText(Span span);

        int Length { get; }

        int LineCount { get; }

        string GetText(Span span);

        char[] ToCharArray(int startIndex, int length);

        void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count);

        char this[int position] { get; }

        TextImageLine GetLineFromLineNumber(int lineNumber);

        TextImageLine GetLineFromPosition(int position);

        int GetLineNumberFromPosition(int position);

        void Write(TextWriter writer, Span span);
    }
}