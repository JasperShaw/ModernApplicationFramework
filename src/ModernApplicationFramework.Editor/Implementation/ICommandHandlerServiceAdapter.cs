using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Editor.Implementation
{
    public interface ICommandHandlerServiceAdapter : ICommandTarget
    {
        ITextView TextView { get; }

        ICommandTarget NextCommandTarget { get; }
    }
}