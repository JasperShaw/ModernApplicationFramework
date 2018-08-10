using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Tagging;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Text.Ui.Tagging
{
    public interface IViewTaggerProvider
    {
        ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag;
    }
}