using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Threading;
using ModernApplicationFramework.EditorBase.Core.OpenSaveDialogFilters;
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

        protected abstract string FallbackSaveExtension { get; }

        public abstract bool CanHandleFile(ISupportedFileDefinition fileDefinition);

        public virtual async Task Reload()
        {
            await LoadFile(Document, Document.FileName);
        }

        public async Task SaveFile(bool saveAs)
        {
            if (!(Document is IStorableFile storableDocument))
                return;
            var fdm = IoC.Get<IFileDefinitionManager>();
            SaveFileArguments args;
            if (!saveAs || storableDocument.IsNew)
            {
                var options = new SaveFileDialogOptions
                {
                    FileName = Document.FileName,
                    Filter = BuildSaveAsFilter().Filter,
                    FilterIndex = 1,
                    InitialDirectory = DefaultSaveAsDirectory,
                    Title = "Save file as",
                    Options = SaveFileDialogFlags.OverwritePrompt,
                    DefaultExtension = FallbackSaveExtension
                };
                args = FileService.Instance.ShowSaveFilesDialog(options);
                if (args == null)
                    return;
            }
            else
                args = new SaveFileArguments(Document.FullFilePath, Document.FileName,
                    fdm.GetDefinitionByFilePath(Document.FileName));

            await MafTaskHelper.Run(IoC.Get<IEnvironmentVariables>().ApplicationName, "Saving File...", async () =>
            {
                FileChangeService.Instance.UnadviseFileChange(Document);
                await storableDocument.Save(args, SaveFile);
                await Task.Delay(TimeSpan.FromSeconds(0.5)).ConfigureAwait(false);
                FileChangeService.Instance.AdviseFileChange(Document);
            });
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

        protected virtual void DirtyDisplayName(bool isDirty)
        {
            if (isDirty)
                DisplayName = Document.FileName + "*";
            else
                DisplayName = Document.FileName;
        }

        protected virtual FilterData BuildSaveAsFilter()
        {
            var def = IoC.Get<IFileDefinitionManager>().GetDefinitionByFilePath(Document.FileName);
            var fd = new FilterData();

            foreach (var definition in IoC.Get<IFileDefinitionManager>().SupportedFileDefinitions)
            {
                fd.AddFilter(new FilterDataEntry(definition.Name, definition.FileExtension));
            }

            //if (def != null)
            //    fd.AddFilter(new FilterDataEntry(def.Name, def.FileExtension));
            fd.AddFilterAnyFile();
            return fd;
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

        private void StorableFile_DirtyChanged(object sender, EventArgs e)
        {
            DirtyDisplayName(((IStorableFile)Document).IsDirty);
        }
    }
}