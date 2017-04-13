using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Extended.Modules.InspectorTool.Commands
{
    [Export(typeof(DefinitionBase))]
    public sealed class OpenInstectorCommandDefinition : CommandDefinition
    {
#pragma warning disable 649
        [Import] private IDockingMainWindowViewModel _shell;
#pragma warning restore 649

        public override ICommand Command { get; }

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
            var command = new MultiKeyGestureCommandWrapper(Open, CanOpen, new MultiKeyGesture(Key.F4));
            Command = command;
            ShortcutText = command.GestureText;
        }

        private bool CanOpen()
        {
            return _shell != null;
        }

        private void Open()
        {
            _shell.DockingHost.ShowTool<IInspectorTool>();
        }
    }
}