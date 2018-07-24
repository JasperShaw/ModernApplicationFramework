namespace ModernApplicationFramework.TextEditor
{
    public interface ITextDataModel
    {
        ITextBuffer DocumentBuffer { get; }

        ITextBuffer DataBuffer { get; }
    }
}