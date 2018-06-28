using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Extended.Clipboard;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Extended.Commands
{
    [Export(typeof(ICopyCommand))]
    internal class CopyCommand : CommandDefinitionCommand, ICopyCommand
    {
        protected override bool OnCanExecute(object parameter)
        {
            return ApplicationCommands.Copy.CanExecute(parameter, null);
        }

        protected override void OnExecute(object parameter)
        {
            ApplicationCommands.Copy.Execute(parameter, null);
            CopyCutWatcher.PushClipboard(ClipboardPushOption.Copy);
        }
    }
}