using System.ComponentModel.Composition;
using ModernApplicationFramework.Editor.Implementation;
using ModernApplicationFramework.Editor.Interop;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Editor.Commanding
{
    [Export(typeof(ICommandHandlerServiceAdapterFactory))]
    internal class CommandHandlerServiceAdapterFactory : ICommandHandlerServiceAdapterFactory
    {
        public ICommandHandlerServiceAdapter Create(ITextView textView, ICommandTarget nextCmdTarget)
        {
            return new CommandHandlerServiceAdapter(textView, nextCmdTarget);
        }

        public ICommandHandlerServiceAdapter Create(ITextView textView, ITextBuffer subjectBuffer, ICommandTarget nextCmdTarget)
        {
            return new CommandHandlerServiceAdapter(textView, subjectBuffer, nextCmdTarget);
        }
    }
}
