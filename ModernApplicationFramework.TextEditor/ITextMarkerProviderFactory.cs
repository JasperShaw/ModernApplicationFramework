namespace ModernApplicationFramework.TextEditor
{
    public interface ITextMarkerProviderFactory
    {
        SimpleTagger<TextMarkerTag> GetTextMarkerTagger(ITextBuffer textBuffer);
    }
}