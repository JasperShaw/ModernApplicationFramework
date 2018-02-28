using System;
using System.IO;
using System.Threading.Tasks;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;

namespace ModernApplicationFramework.EditorBase.FileSupport
{
    //TODO: At some point make it possible to change file properties (name/path/extension) in inspector
    public sealed class StorableDocument : Document, IStorableDocument
    {
        private bool _isDirty;

        public bool IsDirty
        {
            get => _isDirty;
            set
            {
                if (value == _isDirty)
                    return;
                _isDirty = value;
                OnPropertyChanged();
            }
        }

        public bool AskForClose => true;

        public string FileName { get; }
        public string FilePath { get; }

        public bool IsNew { get; private set; }

        private StorableDocument(string filePath, string fileName, bool isNew, bool isDirty)
        {
            FilePath = filePath;
            FileName = fileName;
            IsNew = isNew;
            IsDirty = isDirty;
        }

        private StorableDocument(string fileName, bool isNew, bool isDirty) : this(null, fileName, isNew, isDirty)
        {
        }

        public Task Load(Action loadAction)
        {
            loadAction();
            IsDirty = false;
            return Task.CompletedTask;
        }

        public Task Save(Action saveAction)
        {
            saveAction();
            IsDirty = false;
            IsNew = false;
            return Task.CompletedTask;
        }

        public static StorableDocument CreateNew(string fileName)
        {
            return new StorableDocument(fileName, true, false);
        }

        public static StorableDocument OpenExisting(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                throw new ArgumentException("File was not found");
            var document = new StorableDocument(filePath, Path.GetFileName(filePath), false, false);
            return document;
        }
    }
}