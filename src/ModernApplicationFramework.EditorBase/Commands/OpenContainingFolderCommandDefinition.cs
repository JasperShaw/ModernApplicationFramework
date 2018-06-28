using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.EditorBase.Interfaces.Editor;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.EditorBase.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    public class OpenContainingFolderCommandDefinition : CommandDefinition<IOpenContainingFolderCommand>
    {
        public override string NameUnlocalized => "Open Containing Folder";
        public override string Text => CommandsResources.OpenContainingFolderCommandText;
        public override string ToolTip => Text;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override CommandCategory Category => CommandCategories.FileCommandCategory;
        public override Guid Id => new Guid("{1E11050B-F441-4C18-8A0E-B6C46D4265DE}");
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures => null;
        public override GestureScope DefaultGestureScope => null;
    }

    public interface IOpenContainingFolderCommand : ICommandDefinitionCommand
    {
    }

    [Export(typeof(IOpenContainingFolderCommand))]
    internal class OpenContainingFolderCommand : CommandDefinitionCommand, IOpenContainingFolderCommand
    {
        private readonly IDockingHostViewModel _dockingHostViewModel;

        [ImportingConstructor]
        public OpenContainingFolderCommand(IDockingHostViewModel dockingHostViewModel)
        {
            _dockingHostViewModel = dockingHostViewModel;
        }

        protected override bool OnCanExecute(object parameter)
        {
            return _dockingHostViewModel.ActiveItem is IEditor editor && !string.IsNullOrEmpty(editor.Document.Path);
        }

        protected override void OnExecute(object parameter)
        {
            Process.Start(((IEditor)_dockingHostViewModel.ActiveItem)?.Document.Path);
        }
    }
}
