using ModernApplicationFramework.Text.Logic;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public interface ITextViewModelProvider
    {
        ITextViewModel CreateTextViewModel(ITextDataModel dataModel, ITextViewRoleSet roles);
    }
}