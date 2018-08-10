using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Tagging;
using ModernApplicationFramework.Text.Ui.Tagging;

namespace ModernApplicationFramework.Text.Ui.Adornments
{
    public interface ITextMarkerProviderFactory
    {
        SimpleTagger<TextMarkerTag> GetTextMarkerTagger(ITextBuffer textBuffer);
    }
}