using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Extended.Clipboard;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Extended.Commands
{
    [Export(typeof(ICutCommand))]
    internal class CutCommand : CommandDefinitionCommand, ICutCommand
    {
        protected override bool OnCanExecute(object parameter)
        {
            return ApplicationCommands.Cut.CanExecute(parameter, null);
        }

        protected override void OnExecute(object parameter)
        {
            ApplicationCommands.Cut.Execute(parameter, null);
            CopyCutWatcher.PushClipboard(ClipboardPushOption.Cut);
        }
    }
}