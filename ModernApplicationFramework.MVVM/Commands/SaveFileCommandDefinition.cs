using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.MVVM.Interfaces;

namespace ModernApplicationFramework.MVVM.Commands
{
    [Export(typeof(DefinitionBase))]
    public class SaveFileCommandDefinition : CommandDefinition
    {
#pragma warning disable 649
        [Import] private IDockingMainWindowViewModel _shell;
#pragma warning restore 649


        public SaveFileCommandDefinition()
        {
            Command = new GestureCommandWrapper(SaveFile, CanSaveFile, new KeyGesture(Key.S, ModifierKeys.Control));
        }

        public override bool CanShowInMenu => true;
        public override bool CanShowInToolbar => true;

        public override ICommand Command { get; }
        public override string IconId => "SaveIcon";

        public override Uri IconSource
            =>
                new Uri("/ModernApplicationFramework.MVVM;component/Resources/Icons/Save_16x.xaml",
                    UriKind.RelativeOrAbsolute);

        public override string Name => "Save";
        public override string Text => Name;
        public override string ToolTip => "Saves the active file";

        private bool CanSaveFile()
        {
            return _shell.DockingHost.ActiveItem is IStorableDocument;
        }

        private void SaveFile()
        {
            _shell.DockingHost.ActiveItem.SaveFileCommand.Execute(null);
        }
    }
}