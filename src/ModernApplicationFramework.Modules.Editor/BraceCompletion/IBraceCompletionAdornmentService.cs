using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Modules.Editor.BraceCompletion
{
    public interface IBraceCompletionAdornmentService
    {
        ITrackingPoint Point { get; set; }
    }
}