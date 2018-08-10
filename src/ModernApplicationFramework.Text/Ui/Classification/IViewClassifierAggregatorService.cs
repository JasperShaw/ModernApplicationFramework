using ModernApplicationFramework.Text.Logic.Classification;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Text.Ui.Classification
{
    public interface IViewClassifierAggregatorService
    {
        IClassifier GetClassifier(ITextView textView);
    }
}