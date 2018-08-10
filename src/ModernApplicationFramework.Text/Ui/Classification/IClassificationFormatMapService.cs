using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Text.Ui.Classification
{
    public interface IClassificationFormatMapService
    {
        IClassificationFormatMap GetClassificationFormatMap(ITextView textView);

        IClassificationFormatMap GetClassificationFormatMap(string category);
    }
}