using System.IO;

namespace ModernApplicationFramework.Text.Data
{
    public interface ITextImage
    {
        int Length { get; }

        int LineCount { get; }
        ITextImageVersion Version { get; }

        char this[int position] { get; }

        void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count);

        TextImageLine GetLineFromLineNumber(int lineNumber);

        TextImageLine GetLineFromPosition(int position);

        int GetLineNumberFromPosition(int position);

        ITextImage GetSubText(Span span);

        string GetText(Span span);

        char[] ToCharArray(int startIndex, int length);

        void Write(TextWriter writer, Span span);
    }
}