using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Text.Ui.Classification
{
    public interface IEditorFormatMapService
    {
        IEditorFormatMap GetEditorFormatMap(ITextView view);

        IEditorFormatMap GetEditorFormatMap(string category);
    }
}