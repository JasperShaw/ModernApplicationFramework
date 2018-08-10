using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Text.Logic.Operations
{
    public interface ITextStructureNavigatorSelectorService
    {
        ITextStructureNavigator CreateTextStructureNavigator(ITextBuffer textBuffer, IContentType contentType);
        ITextStructureNavigator GetTextStructureNavigator(ITextBuffer textBuffer);
    }
}