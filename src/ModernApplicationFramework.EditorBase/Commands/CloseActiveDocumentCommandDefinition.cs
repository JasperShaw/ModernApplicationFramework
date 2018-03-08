using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.EditorBase.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    public class CloseActiveDocumentCommandDefinition : CommandDefinition
    {
#pragma warning disable 649
        [Import] private IDockingMainWindowViewModel _shell;
#pragma warning restore 649
        public override string IconId => null;
        public override CommandCategory Category => CommandCategories.FileCommandCategory;
        public override Guid Id => new Guid("{C085DAA2-3C18-4357-991D-70F6C65E4B78}");
        public override Uri IconSource => null;
        public override string Name => "Close";
        public override string NameUnlocalized => "Close";
        public override string Text => CommandsResources.CloseActiveDocumentCommandName;
        public override string ToolTip => Text;

        public override UICommand Command { get; }

        public override MultiKeyGesture DefaultKeyGesture { get; }
        public override GestureScope DefaultGestureScope { get; }

        public CloseActiveDocumentCommandDefinition()
        {
            var command = new UICommand(CloseActive, CanCloseActive);
            Command = command;
            DefaultKeyGesture = new MultiKeyGesture(Key.F4, ModifierKeys.Control);
            DefaultGestureScope = GestureScopes.GlobalGestureScope;
        }

        private bool CanCloseActive()
        {
            return _shell.DockingHost.ActiveItem != null;
        }

        private void CloseActive()
        {
            _shell.DockingHost.ActiveItem.TryClose(true);
        }
    }
}
