using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Extended.Modules.InspectorTool.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    public sealed class OpenInstectorCommandDefinition : CommandDefinition
    {
#pragma warning disable 649
        [Import] private IDockingMainWindowViewModel _shell;
#pragma warning restore 649

        public override UICommand Command { get; }

        public override MultiKeyGesture DefaultKeyGesture { get; }
        public override CommandGestureCategory DefaultGestureCategory { get; }

        public override string IconId => "PropertyIcon";

        public override Uri IconSource =>
            new Uri("/ModernApplicationFramework.Extended.Modules;component/Resources/Icons/Property_16x.xaml",
                UriKind.RelativeOrAbsolute);

        public override string Name => "View.Inspector";
        public override string Text => "Inspector";
        public override string ToolTip => "Inspector";

        public override CommandCategory Category => CommandCategories.ViewCommandCategory;

        public string MyText { get; set; }

        public OpenInstectorCommandDefinition()
        {
            var command = new UICommand(Open, CanOpen);
            Command = command;

            DefaultKeyGesture = new MultiKeyGesture(Key.F4);
            DefaultGestureCategory = CommandGestureCategories.GlobalGestureCategory;
        }

        private bool CanOpen()
        {
            if (!AllowExecution)
                return false;
            return _shell != null;
        }

        private void Open()
        {
            _shell.DockingHost.ShowTool<IInspectorTool>();
        }
    }
}