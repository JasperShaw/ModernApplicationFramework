using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.EditorBase.FileSupport;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.EditorBase.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(NewFileCommandDefinition))]
    public class OpenFileCommandDefinition : CommandDefinition
    {
        public override string NameUnlocalized => "Open File";
        public override string Name => "OpenFile";
        public override string Text => "File...";
        public override string ToolTip => Text;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override CommandCategory Category => CommandCategories.FileCommandCategory;
        public override Guid Id => new Guid("{47E7AF89-3733-4FBF-A3FA-E8AD5D5C693E}");
        public override MultiKeyGesture DefaultKeyGesture { get; }
        public override GestureScope DefaultGestureScope { get; }

        public override UICommand Command { get; }

        public OpenFileCommandDefinition()
        {
            var command = new UICommand(OpenFile, CanOpenFile);
            Command = command;
            DefaultKeyGesture = new MultiKeyGesture(Key.O, ModifierKeys.Control);
            DefaultGestureScope = GestureScopes.GlobalGestureScope;
        }

        private bool CanOpenFile()
        {
            return true;
        }

        private void OpenFile()
        {
            OpenFileHelper.ShowOpenFilesDialog();
        }
    }
}
