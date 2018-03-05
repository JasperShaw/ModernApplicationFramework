using System;
using System.IO;
using System.Threading.Tasks;
using ModernApplicationFramework.EditorBase.Interfaces.Editor;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;
using ModernApplicationFramework.Extended.Layout;

namespace ModernApplicationFramework.EditorBase.Editor
{
    public abstract class EditorBase : KeyBindingLayoutItem, IEditor
    {
        private bool _isReadOnly;

        public IFile Document { get; protected set; }

        public virtual bool IsReadOnly
        {
            get => _isReadOnly;
            protected set
            {
                if (value == _isReadOnly) return;
                _isReadOnly = value;
                NotifyOfPropertyChange();
            }
        }

        public abstract Guid EditorId { get; }

        public abstract string Name { get; }

        public abstract bool CanHandleFile(ISupportedFileDefinition fileDefinition);

        public async Task SaveFile()
        {
            var filePath = Path.GetFileName(Document.FilePath);
            if (Document is IStorableFile storableDocument)
                await storableDocument.Save(() => SaveFile(filePath));
        }

        public async Task LoadFile(IFile document, string name)
        {
            DisplayName = name;
            Document = document;
            await Document.Load(() => LoadFile(document));
        }

        protected virtual void UpdateDisplayName()
        {
            DisplayName = Document.FileName;
        }

        protected abstract void SaveFile(string filePath);

        protected virtual void LoadFile(IFile document)
        {
            Document = document;
            IsReadOnly = !(document is IStorableFile);
        }
    }
}