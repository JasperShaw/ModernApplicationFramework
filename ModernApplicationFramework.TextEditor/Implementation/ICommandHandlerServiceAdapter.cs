using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    public interface ICommandHandlerServiceAdapter : ICommandTarget
    {
        ITextView TextView { get; }

        ICommandTarget NextCommandTarget { get; }
    }
}