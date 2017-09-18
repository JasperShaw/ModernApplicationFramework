using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.MVVM.Interfaces;

namespace ModernApplicationFramework.MVVM.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    public sealed class CloseProgramCommandDefinition : CommandDefinition
    {
#pragma warning disable 649
        [Import] private IDockingMainWindowViewModel _shell;
#pragma warning restore 649

        public CloseProgramCommandDefinition()
        {
            Command = new GestureCommandWrapper(Close, CanClose, new KeyGesture(Key.F4, ModifierKeys.Alt));
        }

        public override bool CanShowInMenu => true;
        public override bool CanShowInToolbar => true;
        public override ICommand Command { get; }

        public override string IconId => "CloseProgramIcon";

        public override Uri IconSource
            =>
                new Uri("/ModernApplicationFramework.MVVM;component/Resources/Icons/CloseProgram_16x.xaml",
                    UriKind.RelativeOrAbsolute);

        public override string Name => "Close";
        public override string Text => Name;
        public override string ToolTip => "Closes the Program";

        private bool CanClose()
        {
            return _shell != null;
        }

        private void Close()
        {
            _shell.CloseCommand.Execute(null);
        }
    }
}