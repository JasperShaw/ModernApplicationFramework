using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.MVVM.Commands;

namespace ModernApplicationFramework.MVVM.Demo.Toolbars
{
    public class DemoToolbarModel
    {
        static DemoToolbarModel()
        {
            ViewLocator.AddNamespaceMapping(typeof(DemoToolbarModel).Namespace,
                typeof(DemoToolbarModel).Namespace);
        }

        public ICommand NewFileCommand
            => IoC.Get<ICommandService>().GetCommandDefinition(typeof(NewFileCommandDefinition)).Command;

        public ICommand OpenFileCommand
            => IoC.Get<ICommandService>().GetCommandDefinition(typeof(OpenFileCommandDefinition)).Command;

        public ICommand RedoCommand
            => IoC.Get<ICommandService>().GetCommandDefinition(typeof(RedoCommandDefinition)).Command;

        public ICommand SaveFileCommand
            => IoC.Get<ICommandService>().GetCommandDefinition(typeof(SaveFileCommandDefinition)).Command;

        public ICommand UndoCommand
            => IoC.Get<ICommandService>().GetCommandDefinition(typeof(UndoCommandDefinition)).Command;
    }
}