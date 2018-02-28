﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using JetBrains.Annotations;
using Microsoft.Win32;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.EditorBase.FileSupport
{
    public abstract class Document : IDocument
    {
        public ICommand SaveFileAsCommand => new Command(SaveFileAs, CanSaveFileAs);
        public ICommand SaveFileCommand => new Command(SaveFile, CanSaveFile);


        private static async Task DoSaveAs(IStorableDocument storableDocument)
        {
            // Show user dialog to choose filename.
            var dialog = new SaveFileDialog {FileName = storableDocument.FileName};
            var filter = string.Empty;


            //TODO: This is for the Editor Base Only
            //var fileExtension = Path.GetExtension(storableDocument.FileName);
            //var fileType =
            //    IoC.GetAll<IEditorProvider>()
            //       .SelectMany(x => x.SupportedFileDefinitions)
            //       .SingleOrDefault(x => x.FileType.FileExtension == fileExtension);
            //if (fileType != null)
            //    filter = fileType.Name + "|*" + fileType.FileType.FileExtension + "|";

            filter += "All Files|*.*";
            dialog.Filter = filter;

            if (dialog.ShowDialog() != true)
                return;

            var filePath = dialog.FileName;

            // Save file.
            //await storableDocument.Save(filePath);
        }

        private bool CanSaveFile()
        {
            return this is IStorableDocument;
        }

        private bool CanSaveFileAs()
        {
            return this is IStorableDocument;
        }


        private async void SaveFile()
        {
            if (!(this is IStorableDocument storableDocument))
                return;

            // If file has never been saved, show Save As dialog.
            if (storableDocument.IsNew)
            {
                await DoSaveAs(storableDocument);
                return;
            }
            // Save file.
            var filePath = storableDocument.FilePath;
            //await storableDocument.Save(filePath);
        }

        private async void SaveFileAs()
        {
            if (!(this is IStorableDocument storableDocument))
                return;

            await DoSaveAs(storableDocument);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}