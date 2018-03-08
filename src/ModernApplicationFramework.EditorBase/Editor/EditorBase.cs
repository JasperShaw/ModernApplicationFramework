using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Threading;
using ModernApplicationFramework.EditorBase.FileSupport;
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

        protected virtual string DefaultSaveAsDirectory => string.Empty;

        public abstract bool CanHandleFile(ISupportedFileDefinition fileDefinition);

        public virtual async Task Reload()
        {
            await LoadFile(Document, Document.FileName);
        }

        public async Task SaveFile(bool saveAs)
        {
            if (Document is IStorableFile storableDocument)
            {
                SaveFileArguments args = null;
                if (saveAs || storableDocument.IsNew)
                {

                }
                else
                    args = new SaveFileArguments(Document.FullFilePath, Document.FileName);
                await MafTaskHelper.Run(IoC.Get<IEnvironmentVariables>().ApplicationName, "Saving File...", async () =>
                {
                    FileChangeService.Instance.UnadviseFileChange(Document);
                    await storableDocument.Save(args, SaveFile);
                    await Task.Delay(TimeSpan.FromSeconds(0.5)).ConfigureAwait(false);
                    FileChangeService.Instance.AdviseFileChange(Document);
                });
            }

        }

        public async Task LoadFile(IFile document, string name)
        {
            DisplayName = name;
            Document = document;
            if (document is IStorableFile storableFile)
                storableFile.DirtyChanged += StorableFile_DirtyChanged;
            IsReadOnly = !(document is IStorableFile);

            await MafTaskHelper.Run(IoC.Get<IEnvironmentVariables>().ApplicationName, "Opening File...", async () =>
            {
                await Document.Load(LoadFile);
            });
        }

        protected virtual void UpdateDisplayName()
        {
            DisplayName = Document.FileName;
        }

        protected virtual void SaveFile()
        {
        }

        protected virtual void LoadFile()
        {
        }

        protected override void OnDeactivate(bool close)
        {
            if (close)
            {
                Document.Unload();
                Document = null;
                if (Document is IStorableFile storableFile)
                    storableFile.DirtyChanged -= StorableFile_DirtyChanged;
            }
            base.OnDeactivate(close);
        }

        protected virtual void DirtyDisplayName(bool isDirty)
        {
            if (isDirty)
                DisplayName = Document.FileName + "*";
            else
                DisplayName = Document.FileName;
        }

        private void StorableFile_DirtyChanged(object sender, EventArgs e)
        {
            DirtyDisplayName(((IStorableFile)Document).IsDirty);
        }
    }
}