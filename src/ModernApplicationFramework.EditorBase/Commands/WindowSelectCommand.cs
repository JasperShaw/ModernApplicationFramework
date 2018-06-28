using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.EditorBase.Dialogs.WindowSelectionDialog;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.EditorBase.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    public class WindowSelectCommand : CommandDefinition
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

        public override ICommand Command { get; }

        public WindowSelectCommand()
        {
            var command = new UICommand(OpenWindowSelectDialog, () => true);
            Command = command;
        }

        private static void OpenWindowSelectDialog()
        {
            IoC.Get<IWindowManager>().ShowDialog(IoC.Get<IWindowSelectionDialogViewModel>());
        }
    }
}
