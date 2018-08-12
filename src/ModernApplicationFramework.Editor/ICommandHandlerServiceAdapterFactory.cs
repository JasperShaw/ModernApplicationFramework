using ModernApplicationFramework.Editor.Implementation;
using ModernApplicationFramework.Editor.Interop;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Editor
{
    public interface ICommandHandlerServiceAdapterFactory
    {
        ICommandHandlerServiceAdapter Create(ITextView textView, ICommandTarget nextCmdTarget);

        ICommandHandlerServiceAdapter Create(ITextView textView, ITextBuffer subjectBuffer, ICommandTarget nextCmdTarget);
    }
}