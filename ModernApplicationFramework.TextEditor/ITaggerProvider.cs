namespace ModernApplicationFramework.TextEditor
{
    public interface ITaggerProvider
    {
        ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag;
    }
}