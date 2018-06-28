using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.EditorBase.Interfaces.Editor;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.EditorBase.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    public class CopyFullPathCommandDefinition : CommandDefinition<ICopyFullPathCommand>
    {
        public override string NameUnlocalized => "Copy Full Path";
        public override string Text => CommandsResources.CopyFullPathCommandText;
        public override string ToolTip => Text;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override CommandCategory Category => CommandCategories.FileCommandCategory;
        public override Guid Id => new Guid("{1F2CAB1F-3624-4D2E-9855-4CD6F62F7B13}");
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures => null;
        public override GestureScope DefaultGestureScope => null;
    }

    public interface ICopyFullPathCommand : ICommandDefinitionCommand
    {
    }

    [Export(typeof(ICopyFullPathCommand))]
    internal class CopyFullPathCommand : CommandDefinitionCommand, ICopyFullPathCommand
    {
        private readonly IDockingHostViewModel _dockingHostViewModel;

        [ImportingConstructor]
        public CopyFullPathCommand(IDockingHostViewModel dockingHostViewModel)
        {
            _dockingHostViewModel = dockingHostViewModel;
        }

        protected override bool OnCanExecute(object parameter)
        {
            return _dockingHostViewModel.ActiveItem is IEditor editor && !string.IsNullOrEmpty(editor.Document.FileName);
        }

        protected override void OnExecute(object parameter)
        {
            Clipboard.SetText(((IEditor)_dockingHostViewModel.ActiveItem)?.Document.FullFilePath);
        }
    }
}
