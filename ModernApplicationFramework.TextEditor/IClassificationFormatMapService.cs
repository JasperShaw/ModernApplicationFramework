namespace ModernApplicationFramework.TextEditor
{
    public interface IClassificationFormatMapService
    {
        IClassificationFormatMap GetClassificationFormatMap(ITextView textView);

        IClassificationFormatMap GetClassificationFormatMap(string category);
    }
}