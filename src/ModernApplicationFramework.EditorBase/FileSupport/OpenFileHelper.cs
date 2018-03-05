using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.EditorBase.Controls.Dialogs;
using ModernApplicationFramework.EditorBase.Core.OpenSaveDialogFilters;
using ModernApplicationFramework.EditorBase.FileSupport.Exceptions;
using ModernApplicationFramework.EditorBase.Interfaces;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;
using ModernApplicationFramework.Native.Platform.Enums;
using ModernApplicationFramework.Utilities.Interfaces;

namespace ModernApplicationFramework.EditorBase.FileSupport
{
    public static class OpenFileHelper
    {
        public static IReadOnlyCollection<OpenFileArguments> ShowOpenFilesDialog()
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
                : CreateOpenFileArguments(dialog.FileNames);
        }

        private static IReadOnlyCollection<OpenFileArguments> CreateOpenFileArguments(IReadOnlyCollection<string> files)
        {
            var arguments = new List<OpenFileArguments>();
            if (!files.Any())
                return arguments;

            var fdm = IoC.Get<IFileDefinitionManager>();
            foreach (var file in files)
            {
                var fileDefinition = fdm.GetDefinitionByExtension(Path.GetExtension(file));
                var argument = new OpenFileArguments(fileDefinition, file, Guid.Empty);
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

        internal static void OpenSupportedFiles(IReadOnlyCollection<OpenFileArguments> files)
        {
            var editorProvider = IoC.Get<IEditorProvider>();
            try
            {
                foreach (var file in files)
                    editorProvider.Open(file);
            }
            catch (FileNotSupportedException exception)
            {
                MessageBox.Show(exception.Message, IoC.Get<IEnvironmentVariables>().ApplicationName,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        internal static void OpenUnsupportedFiles(IReadOnlyCollection<OpenFileArguments> files)
        {
            var flag = false;
            foreach (var file in files)
            {
                var fileType = NativeMethods.NativeMethods.GetExeType(file.Path);
                if (fileType == ShellFileType.Unknown)
                    continue;
                if (fileType == ShellFileType.Windows || fileType == ShellFileType.Dos ||
                    fileType == ShellFileType.Console)
                {
                    flag = true;
                    continue;
                }
                if (NativeMethods.NativeMethods.GetExeType(file.Path) == ShellFileType.Unknown)
                    Process.Start(file.Path);
            }

            if (flag)
            {
                MessageBox.Show("Executables will not be opened", IoC.Get<IEnvironmentVariables>().ApplicationName,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
