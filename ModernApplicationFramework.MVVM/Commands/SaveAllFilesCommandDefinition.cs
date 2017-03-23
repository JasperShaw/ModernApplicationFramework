using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.MVVM.Controls;
using ModernApplicationFramework.MVVM.Interfaces;

namespace ModernApplicationFramework.MVVM.Commands
{
    [Export(typeof(DefinitionBase))]
    public class SaveAllFilesCommandDefinition : CommandDefinition
    {
#pragma warning disable 649
        [Import] private IDockingMainWindowViewModel _shell;
#pragma warning restore 649


        public SaveAllFilesCommandDefinition()
        {
            Command = new GestureCommandWrapper(SaveFile, CanSaveFile, new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Shift));
        }

        public override bool CanShowInMenu => true;
        public override bool CanShowInToolbar => true;

        public override ICommand Command { get; }
        public override string IconId => "SaveAllIcon";

        public override Uri IconSource
            =>
                new Uri("/ModernApplicationFramework.MVVM;component/Resources/Icons/SaveAll_16x.xaml",
                    UriKind.RelativeOrAbsolute);

        public override string Name => "Save all";
        public override string Text => Name;
        public override string ToolTip => "Saves all activ files";

        private bool CanSaveFile()
        {
            return _shell.DockingHost.Documents.Any(x => x is StorableDocument);
        }

        private async void SaveFile()
        {
            foreach (var document in _shell.DockingHost.Documents)
            {
                var storableFile = document as StorableDocument;
                if (storableFile != null && storableFile.IsDirty)
                    await Task.Run(() => storableFile.SaveFileCommand.Execute(null));
            }
        }
    }
}