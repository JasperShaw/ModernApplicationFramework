using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Text.Data.Differencing
{
    public interface ITextDifferencingSelectorService
    {
        ITextDifferencingService DefaultTextDifferencingService { get; }
        ITextDifferencingService GetTextDifferencingService(IContentType contentType);
    }
}