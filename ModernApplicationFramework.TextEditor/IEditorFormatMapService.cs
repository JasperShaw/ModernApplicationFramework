namespace ModernApplicationFramework.TextEditor
{
    public interface IEditorFormatMapService
    {
        IEditorFormatMap GetEditorFormatMap(ITextView view);

        IEditorFormatMap GetEditorFormatMap(string category);
    }
}