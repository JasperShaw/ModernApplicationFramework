using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Extended.Properties;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Extended.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(CloseProgramCommandDefinition))]
    public sealed class CloseProgramCommandDefinition : CommandDefinition
    {
#pragma warning disable 649
        [Import] private IDockingMainWindowViewModel _shell;
#pragma warning restore 649

        public override UICommand Command { get; }

        public override MultiKeyGesture DefaultKeyGesture { get; }
        public override GestureScope DefaultGestureScope { get; }

        public override string IconId => "CloseProgramIcon";

        public override Uri IconSource
            =>
                new Uri("/ModernApplicationFramework.Extended;component/Resources/Icons/CloseProgram_16x.xaml",
                    UriKind.RelativeOrAbsolute);

        public override string Text => Commands_Resources.CloseProgramCommandDefinition_Text;

        public override string NameUnlocalized =>
            Commands_Resources.ResourceManager.GetString("CloseProgramCommandDefinition_Text",
                CultureInfo.InvariantCulture);

        public override string ToolTip => null;

        public override CommandCategory Category => CommandCategories.FileCommandCategory;

        public CloseProgramCommandDefinition()
        {
            var command = new UICommand(Close, CanClose);
            Command = command;

            DefaultKeyGesture = new MultiKeyGesture(Key.F4, ModifierKeys.Alt);
            DefaultGestureScope = GestureScopes.GlobalGestureScope;
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