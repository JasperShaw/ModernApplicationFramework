using System.ComponentModel.Composition;
using System.Windows;
using ModernApplicationFramework.Text.Ui.Commanding;
using ModernApplicationFramework.Text.Ui.Editor.Commanding.Commands;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Extended.Demo.Modules.EditorOverrideCommand
{
    [Export(typeof(ITextEditCommand))]
    [ContentType("Output")]
    [Name("Test Command")]
    internal class CustomCopyCommand : ITextEditCommand<CopyCommandArgs>
    {
        public CommandState GetCommandState(CopyCommandArgs args)
        {
            return CommandState.Available;
        }

        public bool ExecuteCommand(CopyCommandArgs args, CommandExecutionContext executionContext)
        {
            MessageBox.Show("Message First");
            return false;
        }
    }
}
