using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Logic.Tagging
{
    public interface ITaggerProvider
    {
        ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag;
    }
}