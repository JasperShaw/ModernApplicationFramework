namespace ModernApplicationFramework.Text.Data
{
    public interface ITextEdit : ITextBufferEdit
    {
        bool HasEffectiveChanges { get; }

        bool HasFailedChanges { get; }

        bool Delete(Span deleteSpan);

        bool Delete(int startPosition, int charsToDelete);
        bool Insert(int position, string text);

        bool Insert(int position, char[] characterBuffer, int startIndex, int length);

        bool Replace(Span replaceSpan, string replaceWith);

        bool Replace(int startPosition, int charsToReplace, string replaceWith);
    }
}