using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.EditorBase.Dialogs.WindowSelectionDialog;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.EditorBase.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    public class WindowSelectCommandDefinition : CommandDefinition<IWindowSelectCommand>
    {
        public override string NameUnlocalized => "Window";
        public override string Text => CommandsResources.WindowSelectCommandText;
        public override string ToolTip => Text;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override CommandCategory Category => CommandCategories.WindowCommandCategory;
        public override Guid Id => new Guid("{40A3EA90-739D-4569-AF50-3C69C7D44438}");
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures => null;
        public override GestureScope DefaultGestureScope => null;
    }

    public interface IWindowSelectCommand : ICommandDefinitionCommand
    {
    }

    [Export(typeof(IWindowSelectCommand))]
    internal class WindowSelectCommand : CommandDefinitionCommand, IWindowSelectCommand
    {
        protected override bool OnCanExecute(object parameter)
        {
            return true;
        }

        protected override void OnExecute(object parameter)
        {
            IoC.Get<IWindowManager>().ShowDialog(IoC.Get<IWindowSelectionDialogViewModel>());
        }
    }
}
