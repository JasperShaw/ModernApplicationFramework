namespace ModernApplicationFramework.TextEditor
{
    public interface ITextViewModelProvider
    {
        ITextViewModel CreateTextViewModel(ITextDataModel dataModel, ITextViewRoleSet roles);
    }
}