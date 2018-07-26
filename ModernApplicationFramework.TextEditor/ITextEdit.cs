namespace ModernApplicationFramework.TextEditor
{
    public interface ITextEdit : ITextBufferEdit
    {
        bool Insert(int position, string text);

        bool Insert(int position, char[] characterBuffer, int startIndex, int length);

        bool Delete(Span deleteSpan);

        bool Delete(int startPosition, int charsToDelete);

        bool Replace(Span replaceSpan, string replaceWith);

        bool Replace(int startPosition, int charsToReplace, string replaceWith);

        bool HasEffectiveChanges { get; }

        bool HasFailedChanges { get; }
    }
}