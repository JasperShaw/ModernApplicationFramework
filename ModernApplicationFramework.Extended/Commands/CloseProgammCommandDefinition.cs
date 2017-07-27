using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.CommandBase.Input;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Extended.Properties;

namespace ModernApplicationFramework.Extended.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(CloseProgammCommandDefinition))]
    public sealed class CloseProgammCommandDefinition : CommandDefinition
    {
#pragma warning disable 649
        [Import] private IDockingMainWindowViewModel _shell;
#pragma warning restore 649

        public override UICommand Command { get; }

        public override MultiKeyGesture DefaultKeyGesture { get; }
        public override CommandGestureCategory DefaultGestureCategory { get; }

        public override string IconId => "CloseProgrammIcon";

        public override Uri IconSource
            =>
                new Uri("/ModernApplicationFramework.Extended;component/Resources/Icons/CloseProgramm_16x.xaml",
                    UriKind.RelativeOrAbsolute);

        public override string Text => Commands_Resources.CloseProgammCommandDefinition_Text;
        public override string ToolTip => null;

        public override CommandCategory Category => CommandCategories.FileCommandCategory;

        public CloseProgammCommandDefinition()
        {
            var command = new UICommand(Close, CanClose);
            Command = command;

            DefaultKeyGesture = new MultiKeyGesture(new[] { Key.F4 }, ModifierKeys.Alt);
            DefaultGestureCategory = CommandGestureCategories.GlobalGestureCategory;
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