using System;
using System.IO;
using System.Threading.Tasks;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Threading;
using ModernApplicationFramework.EditorBase.Interfaces.Editor;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;
using ModernApplicationFramework.Extended.Layout;
using ModernApplicationFramework.Utilities.Interfaces;

namespace ModernApplicationFramework.EditorBase.Editor
{
    public abstract class EditorBase : KeyBindingLayoutItem, IEditor
    {
        private bool _isReadOnly;
        public abstract string Name { get; }
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

        public abstract string LocalizedName { get; }

        public abstract bool CanHandleFile(ISupportedFileDefinition fileDefinition);

        public virtual async Task Reload()
        {
            await LoadFile(Document, Document.FileName);
        }

        public async Task SaveFile()
        {
            var filePath = Path.GetFileName(Document.FullFilePath);
            if (Document is IStorableFile storableDocument)
                await storableDocument.Save(() => SaveFile(filePath));
        }

        public async Task LoadFile(IFile document, string name)
        {
            DisplayName = name;
            Document = document;

            await MafTaskHelper.Run(IoC.Get<IEnvironmentVariables>().ApplicationName, "Opening File...", async () =>
            {
                await Document.Load(() => LoadFile(document));
            });
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

        protected override void OnDeactivate(bool close)
        {
            if (close)
            {
                Document.Unload();
                Document = null;
            }
            base.OnDeactivate(close);
        }
    }
}