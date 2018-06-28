using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Extended.Clipboard;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Extended.Commands
{
    [Export(typeof(IPasteCommand))]
    internal class PasteCommand : CommandDefinitionCommand, IPasteCommand
    {
        protected override bool OnCanExecute(object parameter)
        {
            return ApplicationCommands.Paste.CanExecute(parameter, null);
        }

        protected override void OnExecute(object parameter)
        {
            ApplicationCommands.Paste.Execute(parameter, null);
            CopyCutWatcher.PushClipboard(ClipboardPushOption.Paste);
        }
    }
}