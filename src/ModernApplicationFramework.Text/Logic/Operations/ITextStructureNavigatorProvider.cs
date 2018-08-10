using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Logic.Operations
{
    public interface ITextStructureNavigatorProvider
    {
        ITextStructureNavigator CreateTextStructureNavigator(ITextBuffer textBuffer);
    }
}