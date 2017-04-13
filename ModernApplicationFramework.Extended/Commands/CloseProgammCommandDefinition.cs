using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Extended.Commands
{
    [Export(typeof(DefinitionBase))]
    public sealed class CloseProgammCommandDefinition : CommandDefinition
    {
#pragma warning disable 649
        [Import] private IDockingMainWindowViewModel _shell;
#pragma warning restore 649

        public override ICommand Command { get; }

        public override string IconId => "CloseProgrammIcon";

        public override Uri IconSource
            =>
                new Uri("/ModernApplicationFramework.Extended;component/Resources/Icons/CloseProgramm_16x.xaml",
                    UriKind.RelativeOrAbsolute);

        public override string Name => "File.Exit";
        public override string Text => "&Exit";
        public override string ToolTip => "Closes the Programm";

        public override CommandCategory Category => CommandCategories.FileCommandCategory;

        public CloseProgammCommandDefinition()
        {
            var command = new MultiKeyGestureCommandWrapper(Close, CanClose,
                new MultiKeyGesture(new[] {Key.F4}, ModifierKeys.Alt));
            Command = command;
            ShortcutText = command.GestureText;
        }

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