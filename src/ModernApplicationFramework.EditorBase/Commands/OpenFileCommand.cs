using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.EditorBase.FileSupport;
using ModernApplicationFramework.EditorBase.Interfaces.Commands;
using ModernApplicationFramework.EditorBase.Interfaces.Services;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.EditorBase.Commands
{
    [Export(typeof(IOpenFileCommand))]
    internal class OpenFileCommand : CommandDefinitionCommand, IOpenFileCommand
    {
        protected override bool OnCanExecute(object parameter)
        {
            return true;
        }

        protected override void OnExecute(object parameter)
        {
            var arguments = FileService.Instance.ShowOpenFilesWithDialog();
            if (!arguments.Any())
                return;
            var service = IoC.Get<IOpenFileService>();
            foreach (var argument in arguments)
                service.OpenFile(argument);
        }
    }
}