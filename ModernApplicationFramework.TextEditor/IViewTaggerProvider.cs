namespace ModernApplicationFramework.TextEditor
{
    public interface IViewTaggerProvider
    {
        ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag;
    }
}