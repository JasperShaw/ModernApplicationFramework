using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.EditorBase.FileSupport;
using ModernApplicationFramework.EditorBase.Interfaces;
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
            var arguments = OpenFileHelper.ShowOpenFilesDialog();
            if (!arguments.Any())
                return;
            var editorProvider = IoC.Get<IEditorProvider>();

            var supportedFiles = new List<OpenFileArguments>();
            var unsupportedFiles = new List<OpenFileArguments>();
            foreach (var argument in arguments)
            {
                if (editorProvider.Handles(argument.Path))
                    supportedFiles.Add(argument);
                else
                    unsupportedFiles.Add(argument);
            }

            OpenFileHelper.OpenSupportedFiles(supportedFiles);
            OpenFileHelper.OpenUnsupportedFiles(unsupportedFiles);
        }
    }
}
