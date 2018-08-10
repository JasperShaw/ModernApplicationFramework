using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Text.Logic.Operations
{
    public interface ITextStructureNavigatorSelectorService
    {
        ITextStructureNavigator GetTextStructureNavigator(ITextBuffer textBuffer);

        ITextStructureNavigator CreateTextStructureNavigator(ITextBuffer textBuffer, IContentType contentType);
    }
}