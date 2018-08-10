using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Text.Data.Differencing
{
    public interface ITextDifferencingSelectorService
    {
        ITextDifferencingService GetTextDifferencingService(IContentType contentType);

        ITextDifferencingService DefaultTextDifferencingService { get; }
    }
}