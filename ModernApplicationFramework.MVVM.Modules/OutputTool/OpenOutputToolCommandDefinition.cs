using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.CommandBase.Input;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Extended.Modules.OutputTool
{
    [Export(typeof(CommandDefinitionBase))]
    public sealed class OpenOutputToolCommandDefinition : CommandDefinition
    {
#pragma warning disable 649
        [Import] private IDockingMainWindowViewModel _shell;
#pragma warning restore 649
        public override UICommand Command { get; }

        public override MultiKeyGesture DefaultKeyGesture { get; }
        public override CommandGestureCategory DefaultGestureCategory { get; }

        public override string IconId => "OutputIcon";

        public override Uri IconSource
            =>
                new Uri("/ModernApplicationFramework.Extended.Modules;component/Resources/Icons/Output_16x.xaml",
                    UriKind.RelativeOrAbsolute);

        public override string Name => "View.Output";
        public override string Text => "Output";
        public override string ToolTip => "Output";

        public override CommandCategory Category => CommandCategories.ViewCommandCategory;

        public string MyText { get; set; }

        public OpenOutputToolCommandDefinition()
        {
            var command = new UICommand(Open, CanOpen);
            Command = command;

            DefaultKeyGesture = new MultiKeyGesture(new[] { Key.O }, ModifierKeys.Control | ModifierKeys.Alt);
            DefaultGestureCategory = CommandGestureCategories.GlobalGestureCategory;
        }

        private bool CanOpen()
        {
            return _shell != null;
        }

        private void Open()
        {
            _shell.DockingHost.ShowTool<IOutput>();
        }
    }
}