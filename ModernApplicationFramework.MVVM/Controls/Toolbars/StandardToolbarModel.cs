using System.Windows.Input;
using ModernApplicationFramework.Caliburn;
using ModernApplicationFramework.Caliburn.Platform.Xaml;
using ModernApplicationFramework.Commands.Service;
using ModernApplicationFramework.MVVM.Commands;

namespace ModernApplicationFramework.MVVM.Controls.Toolbars
{
    public class StandardToolbarModel
    {
        static StandardToolbarModel()
        {
            ViewLocator.AddNamespaceMapping(typeof (StandardToolbarModel).Namespace,
                typeof (StandardToolbarModel).Namespace);
        }

        public ICommand UndoCommand
            => IoC.Get<ICommandService>().GetCommandDefinition(typeof (UndoCommandDefinition)).Command;

        public ICommand RedoCommand
            => IoC.Get<ICommandService>().GetCommandDefinition(typeof(RedoCommandDefinition)).Command;

        public ICommand NewFileCommand
            => IoC.Get<ICommandService>().GetCommandDefinition(typeof(NewFileCommandDefinition)).Command;

        public ICommand OpenFileCommand
            => IoC.Get<ICommandService>().GetCommandDefinition(typeof(OpenFileCommandDefinition)).Command;

        public ICommand SaveFileCommand
            => IoC.Get<ICommandService>().GetCommandDefinition(typeof(SaveFileCommandDefinition)).Command;
    }
}