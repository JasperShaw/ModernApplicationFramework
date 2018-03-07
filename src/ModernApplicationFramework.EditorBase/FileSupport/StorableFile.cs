using System;
using System.IO;
using System.Threading.Tasks;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;

namespace ModernApplicationFramework.EditorBase.FileSupport
{
    //TODO: At some point make it possible to change file properties (name/path/extension) in inspector
    public sealed class StorableFile : MafFile, IStorableFile
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

        public bool IsNew { get; private set; }

        public StorableFile(string filePath, string fileName, bool isNew, bool isDirty) : base(filePath, fileName)
        {
            IsNew = isNew;
            IsDirty = isDirty;
        }

        public StorableFile(string fileName, bool isNew, bool isDirty) : this(string.Empty, fileName, isNew, isDirty)
        {
        }

        public override Task Load(Action loadAction)
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

        public static StorableFile CreateNew(string fileName)
        {
            return new StorableFile(fileName, true, false);
        }

        public static StorableFile OpenExisting(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                throw new ArgumentException("File was not found");
            var document = new StorableFile(filePath, System.IO.Path.GetFileName(filePath), false, false);
            return document;
        }
    }
}