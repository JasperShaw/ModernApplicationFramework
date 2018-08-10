using ModernApplicationFramework.Text.Logic.Tagging;

namespace ModernApplicationFramework.Text.Ui.Tagging
{
    public interface IErrorTag : ITag
    {
        string ErrorType { get; }

        object ToolTipContent { get; }
    }
}