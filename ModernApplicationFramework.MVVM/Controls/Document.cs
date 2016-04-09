using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Win32;
using ModernApplicationFramework.Caliburn;
using ModernApplicationFramework.Commands;
using ModernApplicationFramework.MVVM.Core;
using ModernApplicationFramework.MVVM.Interfaces;
using ModernApplicationFramework.Utilities.UndoRedoManager;

namespace ModernApplicationFramework.MVVM.Controls
{
    public abstract class Document : LayoutItemBase, IDocument
    {
        private ICommand _closeCommand;

        private IUndoRedoManager _undoRedoManager;

        public override ICommand CloseCommand
        {
            get { return _closeCommand ?? (_closeCommand = new RelayCommand(p => TryClose(), p => true)); }
        }

        public ICommand RedoCommand => new Command(Redo, CanRedo);

        public ICommand SaveFileAsCommand => new Command(SaveFileAs, CanSaveFileAs);
        public ICommand SaveFileCommand => new Command(SaveFile, CanSaveFile);


        public ICommand UndoCommand => new Command(Undo, CanUndo);

        public IUndoRedoManager UndoRedoManager => _undoRedoManager ?? (_undoRedoManager = new UndoRedoManager());

        private static async Task DoSaveAs(IStorableDocument storableDocument)
        {
            // Show user dialog to choose filename.
            var dialog = new SaveFileDialog {FileName = storableDocument.FileName};
            var filter = string.Empty;

            var fileExtension = Path.GetExtension(storableDocument.FileName);
            var fileType =
                IoC.GetAll<IEditorProvider>()
                   .SelectMany(x => x.SupportedFileDefinitions)
                   .SingleOrDefault(x => x.FileType.FileExtension == fileExtension);
            if (fileType != null)
                filter = fileType.Name + "|*" + fileType.FileType.FileExtension + "|";

            filter += "All Files|*.*";
            dialog.Filter = filter;

            if (dialog.ShowDialog() != true)
                return;

            var filePath = dialog.FileName;

            // Save file.
            await storableDocument.Save(filePath);
        }

        private bool CanRedo()
        {
            return UndoRedoManager.RedoStack.Any();
        }

        private bool CanSaveFile()
        {
            return this is IStorableDocument;
        }

        private bool CanSaveFileAs()
        {
            return this is IStorableDocument;
        }

        private bool CanUndo()
        {
            return UndoRedoManager.UndoStack.Any();
        }

        private async void Redo()
        {
            await Task.Run(() => UndoRedoManager.Redo());
        }

        private async void SaveFile()
        {
            var storableDocument = this as IStorableDocument;
            if (storableDocument == null)
                return;

            // If file has never been saved, show Save As dialog.
            if (storableDocument.IsNew)
            {
                await DoSaveAs(storableDocument);
                return;
            }
            // Save file.
            var filePath = storableDocument.FilePath;
            await storableDocument.Save(filePath);
        }

        private async void SaveFileAs()
        {
            var storableDocument = this as IStorableDocument;
            if (storableDocument == null)
                return;

            await DoSaveAs(storableDocument);
        }


        private async void Undo()
        {
            await Task.Run(() => UndoRedoManager.Undo());
        }
    }
}