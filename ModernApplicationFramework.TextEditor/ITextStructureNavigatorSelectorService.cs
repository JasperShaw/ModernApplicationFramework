namespace ModernApplicationFramework.TextEditor
{
    public interface ITextStructureNavigatorSelectorService
    {
        ITextStructureNavigator GetTextStructureNavigator(ITextBuffer textBuffer);

        ITextStructureNavigator CreateTextStructureNavigator(ITextBuffer textBuffer, IContentType contentType);
    }
}