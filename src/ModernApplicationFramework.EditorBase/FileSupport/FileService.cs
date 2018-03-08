using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.EditorBase.Controls.OpenWithFileDialog;
using ModernApplicationFramework.EditorBase.Core.OpenSaveDialogFilters;
using ModernApplicationFramework.EditorBase.Interfaces.Editor;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.EditorBase.FileSupport
{
    [Export(typeof(IFileService))]
    public class FileService : IFileService
    {
        private readonly IDockingHostViewModel _shell;
        private static IFileService _instance;

        public static IFileService Instance => _instance ?? (_instance = IoC.Get<IFileService>());

        [ImportingConstructor]
        private FileService(IDockingHostViewModel shell)
        {
            _shell = shell;
        }

        public IReadOnlyList<IFile> OpenedFiles => _shell.LayoutItems.OfType<IEditor>().Select(x => x.Document).ToList();

        public IFile GetOpenedFile(string fileName)
        {
            throw new System.NotImplementedException();
        }

        public IFile CreateFile(NewFileArguments arguments)
        {
            throw new System.NotImplementedException();
        }

        public IFile OpenExistingFile(OpenFileArguments arguments)
        {
            throw new System.NotImplementedException();
        }

        public bool IsFileOpen(string filePath, out IEditor editor)
        {
            editor = default(IEditor);
            if (string.IsNullOrEmpty(filePath))
                return false;
            var editors = _shell.LayoutItems.OfType<IEditor>();
            editor = editors.FirstOrDefault(x => x.Document.FullFilePath.Equals(filePath));
            return editor != null;
        }

        public IReadOnlyCollection<OpenFileArguments> ShowOpenFilesDialog()
        {
            var fdm = IoC.Get<IFileDefinitionManager>();
            var supportedFileDefinitons =
                fdm.SupportedFileDefinitions.Where(x => x.SupportedFileOperation.HasFlag(SupportedFileOperation.Open)).ToList();
            var filterData = BuildFilter(supportedFileDefinitons);
            var dialog = new OpenWithFileDialog
            {
                IsCustom = true,
                Multiselect = true,
                Filter = filterData.Filter,
                FilterIndex = filterData.MaxIndex
            };
            return dialog.ShowDialog() != true
                ? new List<OpenFileArguments>()
                : CreateOpenFileArguments(dialog.FileNames, dialog.CustomResultData as IEditor);
        }

        private static IReadOnlyCollection<OpenFileArguments> CreateOpenFileArguments(IReadOnlyCollection<string> files, IEditor preferrEditor)
        {
            var arguments = new List<OpenFileArguments>();
            if (!files.Any())
                return arguments;

            var fdm = IoC.Get<IFileDefinitionManager>();
            foreach (var file in files)
            {
                var fileDefinition = fdm.GetDefinitionByExtension(Path.GetExtension(file));
                var editorId = Guid.Empty;
                if (preferrEditor != null)
                    editorId = preferrEditor.EditorId;
                var argument = new OpenFileArguments(fileDefinition, file, editorId);
                arguments.Add(argument);
            }
            return arguments;
        }


        internal static FilterData BuildFilter(IReadOnlyCollection<ISupportedFileDefinition> fileDefinitions)
        {
            var filter = new FilterData();
            var availableContexts = IoC.Get<IFileDefinitionContextManager>().GetRegisteredFileDefinitionContexts;
            foreach (var context in availableContexts)
            {
                var t = fileDefinitions.Where(x => x.FileContexts.Contains(context)).Select(x => x.FileExtension).ToList();
                filter.AddFilter(new FilterDataEntry(context.Context, t));
            }
            filter.AddFilterAnyFileAtEnd = true;
            filter.AddFilterAnyFile(FileSupportResources.OpenSaveFileFilterAnyText);
            return filter;
        }
    }

    public interface IFileService
    {
        IReadOnlyList<IFile> OpenedFiles { get; }

        IFile GetOpenedFile(string fileName);

        IFile CreateFile(NewFileArguments arguments);

        IFile OpenExistingFile(OpenFileArguments arguments);

        bool IsFileOpen(string filePath, out IEditor editor);

        IReadOnlyCollection<OpenFileArguments> ShowOpenFilesDialog();
    }
}
